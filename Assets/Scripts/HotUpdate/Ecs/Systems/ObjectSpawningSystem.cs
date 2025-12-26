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
using cfg; // [新增]
using System.Linq; // [新增]

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
        }

        protected override void OnUpdate()
        {
            var query = SystemAPI.QueryBuilder().WithAll<PlaceObjectRequest>().Build();
            if (query.IsEmpty) return;

            var entities = query.ToEntityArray(Allocator.Temp);
            var requests = query.ToComponentDataArray<PlaceObjectRequest>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                var req = requests[i];
                bool processed = false;

                if (req.Type == PlacementType.Island)
                {
                    if (!_gridSystem.CheckIslandPlacement(req.Position, req.Size, req.AirspaceHeight))
                    {
                        EntityManager.DestroyEntity(entity);
                        continue;
                    }

                    if (TrySpawnObject(req, out Entity spawned))
                    {
                        var islandBaseData = ConfigManager.Instance.Tables.TbIsland.GetOrDefault(req.ObjectId);
                        if (islandBaseData != null)
                        {
                            _gridSystem.RegisterIsland(req.Position, islandBaseData, req.RotationIndex);
                            InitializeIslandLogicAttributes(spawned);
                            EntityManager.AddComponentData(spawned, new IslandComponent
                            {
                                ConfigId = req.ObjectId,
                                Size = req.Size,
                                AirSpace = req.AirspaceHeight
                            });
                            processed = true;
                        }
                    }
                }
                else if (req.Type == PlacementType.Building)
                {
                    int3 end = req.Position + req.Size - new int3(1, 1, 1);
                    if (!_gridSystem.IsBuildingBuildable(req.Position, end))
                    {
                        EntityManager.DestroyEntity(entity);
                        continue;
                    }

                    if (TrySpawnObject(req, out Entity spawned))
                    {
                        // 1. 注册与基础组件
                        _gridSystem.RegisterBuilding(req.Position, req.Size, new FixedString64Bytes(req.ObjectId.ToString()));

                        // [修复] 直接读取 TbBuild 表
                        var buildCfg = ConfigManager.Instance.Tables.TbBuild.GetOrDefault(req.ObjectId);
                        int bType = (int)buildCfg.BuildingType;
                        string nameStr = buildCfg.Name;
                        int subtype = (int)buildCfg.BuildingSubtype;

                        EntityManager.AddComponentData(spawned, new BuildingComponent
                        {
                            ConfigId = req.ObjectId,
                            Size = req.Size,
                            BuildingType = bType,
                            BuildingSubtype = subtype,
                            Name = new FixedString64Bytes(nameStr)
                        });

                        // [修复] 读取 TbBuildingLevel 表
                        var levelCfg = ConfigManager.Instance.Tables.TbBuildingLevel.GetOrDefault(req.ObjectId);
                        if (levelCfg != null)
                        {
                            if (levelCfg.Prosperity > 0)
                                EntityManager.AddComponentData(spawned, new ProsperityComponent { Value = levelCfg.Prosperity });

                            if (levelCfg.PowerConsumption > 0)
                                EntityManager.AddComponentData(spawned, new ElectricityComponent { PowerConsumption = levelCfg.PowerConsumption, IsPowered = true });
                        }

                        // 4. 根据 BuildingType 挂载功能组件
                        switch (bType)
                        {
                            case 1: // Core
                                if (subtype == 1)
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
                                var factoryData = ConfigManager.Instance.Tables.TbBuildingLevel.GetOrDefault(req.ObjectId);
                                if (factoryData != null)
                                {
                                    // 1. 计算总存储量
                                    int totalStorage = 0;
                                    foreach (var store in factoryData.OutputStorage) { if (store.Count >= 2) totalStorage += store[1]; }
                                    if (totalStorage <= 0) totalStorage = 50; // 默认值防止为0

                                    // 2. 挂载生产组件
                                    EntityManager.AddComponentData(spawned, new ProductionComponent
                                    {
                                        ProductionInterval = (float)factoryData.OutputCycle,
                                        MaxReserves = factoryData.OutputStorage[0][0],
                                        JobSlots = factoryData.JobSlots,
                                        DemandOccupation = (int)factoryData.DemandOccupation, // 假设枚举类型兼容，否则需强转 (int)
                                        IsActive = true,
                                        Timer = 0f
                                    });

                                    // 3. 岛屿亲和 Buffer
                                    if (factoryData.IslandAffinity.Count > 0)
                                    {
                                        var affinityBuffer = EntityManager.AddBuffer<IslandAffinityElement>(spawned);
                                        // 这里的 IslandAffinity 是 List<IslandType>，如果你的 Buffer 存的是 int，需要强转
                                        foreach (var affinity in factoryData.IslandAffinity)
                                            affinityBuffer.Add(new IslandAffinityElement { IslandType = (int)affinity });
                                    }

                                    // 4. 原料输入 Buffer
                                    if (factoryData.ItemCost.Count > 0)
                                    {
                                        var inputBuffer = EntityManager.AddBuffer<ProductionInputElement>(spawned);
                                        foreach (var input in factoryData.ItemCost)
                                        {
                                            if (input.Count >= 2)
                                                inputBuffer.Add(new ProductionInputElement { ItemId = input[0], Count = input[1] });
                                        }
                                    }

                                    // 5. 产品输出 Buffer
                                    if (factoryData.OutPut.Count > 0)
                                    {
                                        var outputBuffer = EntityManager.AddBuffer<ProductionOutputElement>(spawned);
                                        foreach (var output in factoryData.OutPut)
                                        {
                                            if (output.Count >= 2)
                                                outputBuffer.Add(new ProductionOutputElement { ItemId = output[0], CountPerCycle = output[1], CurrentStorage = 0 });
                                        }
                                    }
                                }
                                else
                                {
                                    Debug.LogWarning($"[Spawn] 建筑 {req.ObjectId} (Factory) 产出配置无效。");
                                }
                                break;

                            case 4: // Service
                                var prodData = ConfigManager.Instance.Tables.TbBuildingLevel.GetOrDefault(req.ObjectId);

                                // --- A. 复用生产逻辑 (Case 4 也有生产组件) ---
                                if (prodData != null)
                                {
                                    // 计算存储
                                    int totalStorage = 0;
                                    foreach (var store in prodData.OutputStorage) { if (store.Count >= 2) totalStorage += store[1]; }
                                    if (totalStorage <= 0) totalStorage = 20;

                                    EntityManager.AddComponentData(spawned, new ProductionComponent
                                    {
                                        ProductionInterval = (float)prodData.OutputCycle,
                                        MaxReserves = totalStorage,
                                        JobSlots = prodData.JobSlots,
                                        DemandOccupation = (int)prodData.DemandOccupation,
                                        IsActive = true,
                                        Timer = 0f
                                    });

                                    // Buffer: 原料 (ItemCost)
                                    if (prodData.ItemCost.Count > 0)
                                    {
                                        var inBuf = EntityManager.AddBuffer<ProductionInputElement>(spawned);
                                        foreach (var v in prodData.ItemCost)
                                            if (v.Count >= 2) inBuf.Add(new ProductionInputElement { ItemId = v[0], Count = v[1] });
                                    }

                                    // Buffer: 产出 (OutPut)
                                    // 注意：服务类建筑的 OutPut 通常是给 ProductionSystem 用的产出（如有），
                                    // 或者是服务完成后的奖励。这里照搬原逻辑挂载到 ProductionOutputElement。
                                    if (prodData.OutPut.Count > 0)
                                    {
                                        var outBuf = EntityManager.AddBuffer<ProductionOutputElement>(spawned);
                                        foreach (var v in prodData.OutPut)
                                            if (v.Count >= 2) outBuf.Add(new ProductionOutputElement { ItemId = v[0], CountPerCycle = v[1], CurrentStorage = 0 });
                                    }

                                    // Buffer: 亲和
                                    if (prodData.IslandAffinity.Count > 0)
                                    {
                                        var affBuf = EntityManager.AddBuffer<IslandAffinityElement>(spawned);
                                        foreach (var v in prodData.IslandAffinity) affBuf.Add(new IslandAffinityElement { IslandType = (int)v });
                                    }
                                }

                                // --- B. 服务逻辑 ---
                                // 既然原版是 GetFactoryConfig 和 GetServiceConfig 分开的，
                                // 现在都在 TbBuildingLevel 里。
                                if (prodData != null)
                                {
                                    // 解析服务奖励：假设 OutPut 的第一个元素是服务奖励 (ItemId, Amount)
                                    // 如果没有配 Output，则默认为 0
                                    int outId = 0;
                                    int outCount = 0;
                                    if (prodData.OutPut.Count > 0 && prodData.OutPut[0].Count >= 2)
                                    {
                                        outId = prodData.OutPut[0][0];
                                        outCount = prodData.OutPut[0][1];
                                    }

                                    EntityManager.AddComponentData(spawned, new ServiceComponent
                                    {
                                        ServiceConfigId = req.ObjectId,
                                        ServiceTime = (float)prodData.DwellTime,      // 对应 ServiceTime
                                        MaxVisitorCapacity = prodData.VisitorCapacity, // 对应 MaxVisitorCapacity
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

                        processed = true;
                    }
                }
                else if (req.Type == PlacementType.Bridge)
                {
                    if (!_gridSystem.IsBridgeBuildable(req.Position))
                    {
                        EntityManager.DestroyEntity(entity);
                        continue;
                    }

                    if (TrySpawnObject(req, out Entity spawned))
                    {
                        _gridSystem.RegisterBridge(req.Position, new FixedString64Bytes(req.ObjectId.ToString()));
                        if (_visSystem != null) _visSystem.ShowBridgeableGrids();
                        EntityManager.AddComponentData(spawned, new BridgeComponent { ConfigId = req.ObjectId });
                        processed = true;
                    }
                }

                if (processed) EntityManager.DestroyEntity(entity);
            }
            entities.Dispose();
            requests.Dispose();
        }

        private void InitializeIslandLogicAttributes(Entity islandEntity)
        {
            // [修复] 随机获取一个 Level 1 岛屿配置
            // 假设 TbIslandLevel 表包含岛屿等级数据，这里简单取第一个或者随机取
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

                var buffer = EntityManager.AddBuffer<BuildableStructureElement>(islandEntity);
                foreach (var structureId in randCfg.BuildableStructures)
                {
                    buffer.Add(new BuildableStructureElement { StructureType = structureId });
                }
            }
        }

        private bool TrySpawnObject(PlaceObjectRequest req, out Entity spawned)
        {
            spawned = Entity.Null;
            // [修复] 手动获取资源路径
            string path = "";
            if (req.Type == PlacementType.Building)
                path = ConfigManager.Instance.Tables.TbBuild.GetOrDefault(req.ObjectId)?.ResourceName;
            else if (req.Type == PlacementType.Island)
                path = ConfigManager.Instance.Tables.TbIsland.GetOrDefault(req.ObjectId)?.ResourceName;
            else if (req.Type == PlacementType.Bridge)
            {
                // [修复] 查表获取桥梁资源名
                var cfg = ConfigManager.Instance.Tables.TbBridgeConfig.GetOrDefault(req.ObjectId);
                path = cfg != null ? cfg.ResourceName : $"bridge_{req.ObjectId}";
            }
            if (string.IsNullOrEmpty(path)) return true;

            float3 worldPos = _gridSystem.CalculateObjectCenterWorldPosition(req.Position, req.Size);

            if (_entityFactory.HasEntity(req.ObjectId))
            {
                float s = _gridConfig.CellSize;
                float3 size = new float3(req.Size.x, req.Size.y, req.Size.z) * s;
                var box = new BoxGeometry { Center = float3.zero, Orientation = quaternion.identity, Size = new float3(size.x, 0.5f, size.z) };
                spawned = _entityFactory.SpawnColliderEntity(req.ObjectId, worldPos, req.Rotation, box);
            }

            if (spawned == Entity.Null)
            {
                if (!_loadingAssets.Contains(req.ObjectId))
                {
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
            await _entityFactory.LoadEntityArchetypeAsync(id, path);
            _loadingAssets.Remove(id);
        }
    }
}