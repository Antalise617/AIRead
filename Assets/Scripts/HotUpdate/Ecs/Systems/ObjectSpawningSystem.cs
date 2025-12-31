using Cysharp.Threading.Tasks;
using GameFramework.Core;
using GameFramework.ECS.Components;
using GameFramework.Managers;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using cfg;
using System.Linq;

namespace GameFramework.ECS.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class ObjectSpawningSystem : SystemBase
    {
        private GridSystem _gridSystem;
        private EntityFactory _entityFactory;
        private GridConfigComponent _gridConfig;
        private GridEntityVisualizationSystem _visSystem;
        private HashSet<int> _loadingAssets = new HashSet<int>();

        // [新增] 用于记录已经完成“数据注册”但还在等待“资源加载”的请求实体
        private HashSet<Entity> _processedRequests = new HashSet<Entity>();

        protected override void OnCreate()
        {
            base.OnCreate();
            _entityFactory = new EntityFactory(EntityManager);
            RequireForUpdate<GridConfigComponent>();
        }

        protected override void OnDestroy()
        {
            _entityFactory.Dispose();
            base.OnDestroy();
        }

        protected override void OnStartRunning()
        {
            _gridSystem = World.GetExistingSystemManaged<GridSystem>();
            _visSystem = World.GetExistingSystemManaged<GridEntityVisualizationSystem>();
            _gridConfig = SystemAPI.GetSingleton<GridConfigComponent>();
            Debug.Log($"[ObjectSpawningSystem] 系统启动运行。GridSystem={(_gridSystem != null ? "OK" : "NULL")}");
        }

        protected override void OnUpdate()
        {
            var query = SystemAPI.QueryBuilder().WithAll<PlaceObjectRequest>().Build();
            if (query.IsEmpty) return;

            var entities = query.ToEntityArray(Allocator.Temp);
            var requests = query.ToComponentDataArray<PlaceObjectRequest>(Allocator.Temp);

            Debug.Log($"[ObjectSpawningSystem] 本帧处理 {entities.Length} 个生成请求...");

            // 第一轮循环：优先处理岛屿 (Islands)
            for (int i = 0; i < entities.Length; i++)
            {
                if (requests[i].Type == PlacementType.Island)
                {
                    ProcessIslandRequest(entities[i], requests[i]);
                }
            }

            // 第二轮循环：处理其他物体 (Buildings, Bridges)
            for (int i = 0; i < entities.Length; i++)
            {
                if (requests[i].Type != PlacementType.Island)
                {
                    ProcessOtherRequest(entities[i], requests[i]);
                }
            }

            entities.Dispose();
            requests.Dispose();
        }

        private void ProcessIslandRequest(Entity entity, PlaceObjectRequest req)
        {
            if (!_processedRequests.Contains(entity))
            {
                Debug.Log($"[Spawn-Island] 处理岛屿逻辑注册 ID:{req.ObjectId} Pos:{req.Position}");

                if (!_gridSystem.CheckIslandPlacement(req.Position, req.Size, req.AirspaceHeight))
                {
                    Debug.LogError($"[Spawn-Island] ❌ 位置检测失败！坐标 {req.Position} 可能超出边界或重叠。");
                    EntityManager.DestroyEntity(entity);
                    return;
                }

                var islandBaseData = ConfigManager.Instance.Tables.TbIsland.GetOrDefault(req.ObjectId);
                if (islandBaseData != null)
                {
                    _gridSystem.RegisterIsland(req.Position, islandBaseData, req.RotationIndex);
                    _processedRequests.Add(entity);
                }
                else
                {
                    Debug.LogError($"[Spawn-Island] ❌ 找不到配置 ID:{req.ObjectId}");
                    EntityManager.DestroyEntity(entity);
                    return;
                }
            }

            if (TrySpawnObject(req, out Entity spawned))
            {
                Debug.Log($"[Spawn-Island] ✅ 实体生成成功！Entity: {spawned}");

                InitializeIslandLogicAttributes(spawned);
                EntityManager.AddComponentData(spawned, new IslandComponent
                {
                    ConfigId = req.ObjectId,
                    Size = req.Size,
                    AirSpace = req.AirspaceHeight
                });

                if (EntityManager.HasComponent<IslandStatusComponent>(entity))
                {
                    var status = EntityManager.GetComponentData<IslandStatusComponent>(entity);
                    EntityManager.AddComponentData(spawned, status);
                }
                else
                {
                    EntityManager.AddComponentData(spawned, new IslandStatusComponent
                    {
                        State = 1,
                        CreateTime = (long)System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1)).TotalSeconds
                    });
                }

                EntityManager.DestroyEntity(entity);
                _processedRequests.Remove(entity);
            }
            else
            {
                Debug.LogWarning($"[Spawn-Island] ⏳ 资源加载中 ID:{req.ObjectId}...");
            }
        }

        private void ProcessOtherRequest(Entity entity, PlaceObjectRequest req)
        {
            if (req.Type == PlacementType.Building)
            {
                Debug.Log($"[Spawn-Building] 处理建筑请求 ID:{req.ObjectId} Pos:{req.Position}");

                int3 end = req.Position + req.Size - new int3(1, 1, 1);
                if (!_gridSystem.IsBuildingBuildable(req.Position, end))
                {
                    Debug.LogError($"[Spawn-Building] ❌ 建筑位置检测失败: Start:{req.Position} End:{end} (请检查脚下是否有岛屿)");
                    EntityManager.DestroyEntity(entity);
                    return;
                }

                if (TrySpawnObject(req, out Entity spawned))
                {
                    _gridSystem.RegisterBuilding(req.Position, req.Size, new FixedString64Bytes(req.ObjectId.ToString()));

                    var buildCfg = ConfigManager.Instance.Tables.TbBuild.GetOrDefault(req.ObjectId);

                    EntityManager.AddComponentData(spawned, new BuildingComponent
                    {
                        ConfigId = req.ObjectId,
                        Size = req.Size,
                        BuildingType = (int)buildCfg.BuildingType,
                        BuildingSubtype = (int)buildCfg.BuildingSubtype,
                        Name = new FixedString64Bytes(buildCfg.Name)
                    });

                    var levelCfg = ConfigManager.Instance.Tables.TbBuildingLevel.GetOrDefault(req.ObjectId);
                    if (levelCfg != null)
                    {
                        if (levelCfg.Prosperity > 0)
                            EntityManager.AddComponentData(spawned, new ProsperityComponent { Value = levelCfg.Prosperity });

                        if (levelCfg.PowerConsumption > 0)
                            EntityManager.AddComponentData(spawned, new ElectricityComponent { PowerConsumption = levelCfg.PowerConsumption, IsPowered = true });
                    }

                    int bType = (int)buildCfg.BuildingType;
                    int subtype = (int)buildCfg.BuildingSubtype;

                    switch (bType)
                    {
                        case 1: // Core
                            if (subtype == 2)
                            {
                                var cfgVisitor = new float2(5, 2.0f);
                                EntityManager.AddComponentData(spawned, new VisitorCenterComponent
                                {
                                    UnspawnedVisitorCount = (int)cfgVisitor.x,
                                    SpawnInterval = cfgVisitor.y
                                });
                            }
                            break;

                        case 2: // Supply
                            break;

                        case 3: // Factory
                            if (levelCfg != null)
                            {
                                int totalStorage = 0;
                                foreach (var store in levelCfg.OutputStorage) { if (store.Count >= 2) totalStorage += store[1]; }
                                if (totalStorage <= 0) totalStorage = 50;

                                EntityManager.AddComponentData(spawned, new ProductionComponent
                                {
                                    ProductionInterval = (float)levelCfg.OutputCycle,
                                    MaxReserves = levelCfg.OutputStorage.Count > 0 && levelCfg.OutputStorage[0].Count > 0 ? levelCfg.OutputStorage[0][0] : 0,
                                    JobSlots = levelCfg.JobSlots,
                                    DemandOccupation = (int)levelCfg.DemandOccupation,
                                    IsActive = true,
                                    Timer = 0f
                                });

                                if (levelCfg.IslandAffinity.Count > 0)
                                {
                                    var affinityBuffer = EntityManager.AddBuffer<IslandAffinityElement>(spawned);
                                    foreach (var affinity in levelCfg.IslandAffinity)
                                        affinityBuffer.Add(new IslandAffinityElement { IslandType = (int)affinity });
                                }

                                if (levelCfg.ItemCost.Count > 0)
                                {
                                    var inputBuffer = EntityManager.AddBuffer<ProductionInputElement>(spawned);
                                    foreach (var input in levelCfg.ItemCost)
                                    {
                                        if (input.Count >= 2)
                                            inputBuffer.Add(new ProductionInputElement { ItemId = input[0], Count = input[1] });
                                    }
                                }

                                if (levelCfg.OutPut.Count > 0)
                                {
                                    var outputBuffer = EntityManager.AddBuffer<ProductionOutputElement>(spawned);
                                    foreach (var output in levelCfg.OutPut)
                                    {
                                        if (output.Count >= 2)
                                            outputBuffer.Add(new ProductionOutputElement { ItemId = output[0], CountPerCycle = output[1], CurrentStorage = 0 });
                                    }
                                }
                            }
                            break;

                        case 4: // Service
                            if (levelCfg != null)
                            {
                                int totalStorage = 0;
                                foreach (var store in levelCfg.OutputStorage) { if (store.Count >= 2) totalStorage += store[1]; }
                                if (totalStorage <= 0) totalStorage = 20;

                                EntityManager.AddComponentData(spawned, new ProductionComponent
                                {
                                    ProductionInterval = (float)levelCfg.OutputCycle,
                                    MaxReserves = totalStorage,
                                    JobSlots = levelCfg.JobSlots,
                                    DemandOccupation = (int)levelCfg.DemandOccupation,
                                    IsActive = true,
                                    Timer = 0f
                                });

                                if (levelCfg.ItemCost.Count > 0)
                                {
                                    var inBuf = EntityManager.AddBuffer<ProductionInputElement>(spawned);
                                    foreach (var v in levelCfg.ItemCost)
                                        if (v.Count >= 2) inBuf.Add(new ProductionInputElement { ItemId = v[0], Count = v[1] });
                                }

                                if (levelCfg.OutPut.Count > 0)
                                {
                                    var outBuf = EntityManager.AddBuffer<ProductionOutputElement>(spawned);
                                    foreach (var v in levelCfg.OutPut)
                                        if (v.Count >= 2) outBuf.Add(new ProductionOutputElement { ItemId = v[0], CountPerCycle = v[1], CurrentStorage = 0 });
                                }

                                if (levelCfg.IslandAffinity.Count > 0)
                                {
                                    var affBuf = EntityManager.AddBuffer<IslandAffinityElement>(spawned);
                                    foreach (var v in levelCfg.IslandAffinity) affBuf.Add(new IslandAffinityElement { IslandType = (int)v });
                                }

                                int outId = 0;
                                int outCount = 0;
                                if (levelCfg.OutPut.Count > 0 && levelCfg.OutPut[0].Count >= 2)
                                {
                                    outId = levelCfg.OutPut[0][0];
                                    outCount = levelCfg.OutPut[0][1];
                                }

                                EntityManager.AddComponentData(spawned, new ServiceComponent
                                {
                                    ServiceConfigId = req.ObjectId,
                                    ServiceTime = (float)levelCfg.DwellTime,
                                    MaxVisitorCapacity = levelCfg.VisitorCapacity,
                                    OutputItemId = outId,
                                    OutputItemCount = outCount,
                                    IsActive = true,
                                    IsServing = false,
                                    ServiceTimer = 0f
                                });
                                EntityManager.AddBuffer<ServiceQueueElement>(spawned);
                            }
                            break;
                    }

                    EntityManager.DestroyEntity(entity);
                }
            }
            else if (req.Type == PlacementType.Bridge)
            {
                Debug.Log($"[Spawn-Bridge] 处理桥梁请求 ID:{req.ObjectId} Pos:{req.Position}");

                if (!_gridSystem.IsBridgeBuildable(req.Position))
                {
                    Debug.LogError($"[Spawn-Bridge] ❌ 桥梁位置检测失败: {req.Position}");
                    EntityManager.DestroyEntity(entity);
                    return;
                }

                if (TrySpawnObject(req, out Entity spawned))
                {
                    _gridSystem.RegisterBridge(req.Position, new FixedString64Bytes(req.ObjectId.ToString()));

                    // [修复] 调用正确的刷新方法，且不需要传递 int2 参数
                    if (_visSystem != null)
                    {
                        _visSystem.ShowBridgeableGrids(true);
                    }

                    EntityManager.AddComponentData(spawned, new BridgeComponent { ConfigId = req.ObjectId });
                    EntityManager.DestroyEntity(entity);
                }
            }
        }

        private void InitializeIslandLogicAttributes(Entity islandEntity)
        {
            var allLevels = ConfigManager.Instance.Tables.TbIslandLevel.DataList;
            if (allLevels.Count > 0)
            {
                var randCfg = allLevels[UnityEngine.Random.Range(0, allLevels.Count)];
                EntityManager.AddComponentData(islandEntity, new IslandDataComponent
                {
                    Level = 1,
                    IslandType = (int)randCfg.IslandType,
                    BonusType = (int)randCfg.BonusBuildingTypes,
                    BonusValue = randCfg.Value
                });
                EntityManager.AddBuffer<BuildableStructureElement>(islandEntity);
            }
        }

        private bool TrySpawnObject(PlaceObjectRequest req, out Entity spawned)
        {
            spawned = Entity.Null;
            string path = "";

            if (req.Type == PlacementType.Building)
                path = ConfigManager.Instance.Tables.TbBuild.GetOrDefault(req.ObjectId)?.ResourceName;
            else if (req.Type == PlacementType.Island)
                path = ConfigManager.Instance.Tables.TbIsland.GetOrDefault(req.ObjectId)?.ResourceName;
            else if (req.Type == PlacementType.Bridge)
                path = ConfigManager.Instance.Tables.TbBridgeConfig.GetOrDefault(req.ObjectId)?.ResourceName ?? $"bridge_{req.ObjectId}";

            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError($"[Spawn] ❌ 配置表中找不到资源路径！Type:{req.Type} ID:{req.ObjectId}");
                return true;
            }

            float3 worldPos = _gridSystem.CalculateObjectCenterWorldPosition(req.Position, req.Size);

            if (req.Type == PlacementType.Island)
            {
                worldPos.y += 1f;
            }

            if (_entityFactory.HasEntity(req.ObjectId))
            {
                float s = _gridConfig.CellSize;
                float3 size = new float3(req.Size.x, req.Size.y, req.Size.z) * s;

                float3 colliderCenter = float3.zero;
                if (req.Type == PlacementType.Island)
                {
                    colliderCenter = new float3(0, -1f, 0);
                }

                var box = new BoxGeometry { Center = colliderCenter, Orientation = quaternion.identity, Size = new float3(size.x, 0.5f, size.z) };

                spawned = _entityFactory.SpawnColliderEntity(req.ObjectId, worldPos, req.Rotation, box);
            }

            if (spawned == Entity.Null)
            {
                if (!_loadingAssets.Contains(req.ObjectId))
                {
                    Debug.Log($"[Spawn] 📥 触发资源加载: [{req.ObjectId}] {path}");
                    _loadingAssets.Add(req.ObjectId);
                    LoadAsset(req.ObjectId, path).Forget();
                }
                return false;
            }

            EntityManager.AddComponentData(spawned, new GridPositionComponent { Value = req.Position });
            return true;
        }

        private async UniTaskVoid LoadAsset(int id, string path)
        {
            try
            {
                await _entityFactory.LoadEntityArchetypeAsync(id, path);
                Debug.Log($"[Spawn] ✅ 资源加载完成: {path}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[Spawn] ❌ 资源加载异常: {e.Message}");
            }
            finally
            {
                _loadingAssets.Remove(id);
            }
        }
    }
}