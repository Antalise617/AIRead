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
                    // ... (岛屿生成逻辑保持不变) ...
                    if (!_gridSystem.CheckIslandPlacement(req.Position, req.Size, req.AirspaceHeight))
                    {
                        EntityManager.DestroyEntity(entity);
                        continue;
                    }

                    if (TrySpawnObject(req, out Entity spawned))
                    {
                        var islandBaseData = GameConfigBridge.GetIslandData(req.ObjectId);
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
                        // 1. 注册格子占用
                        _gridSystem.RegisterBuilding(req.Position, req.Size, new FixedString64Bytes(req.ObjectId.ToString()));

                        // 2. 获取基础配置
                        int bType = GameConfigBridge.GetBuildingFunctionType(req.ObjectId);
                        string nameStr = GameConfigBridge.GetBuildingName(req.ObjectId);
                        int subtype = GameConfigBridge.GetBuildingSubtype(req.ObjectId);

                        // 3. 添加通用建筑组件
                        EntityManager.AddComponentData(spawned, new BuildingComponent
                        {
                            ConfigId = req.ObjectId,
                            Size = req.Size,
                            BuildingType = bType,
                            BuildingSubtype = subtype,
                            Name = new FixedString64Bytes(nameStr)
                        });

                        // 1. 繁荣度
                        int prosperity = GameConfigBridge.GetBuildingProsperity(req.ObjectId);
                        if (prosperity > 0)
                        {
                            EntityManager.AddComponentData(spawned, new ProsperityComponent
                            {
                                Value = prosperity
                            });
                        }

                        // 2. 电力
                        // 即使耗电量为0，只要是功能建筑最好也挂上，方便统一管理，这里视耗电量>0挂载
                        int powerCost = GameConfigBridge.GetBuildingPowerConsumption(req.ObjectId);
                        if (powerCost > 0)
                        {
                            EntityManager.AddComponentData(spawned, new ElectricityComponent
                            {
                                PowerConsumption = powerCost,
                                IsPowered = true // 【需求】默认开启，假装有电
                            });
                        }

                        // 4. 根据 BuildingType 挂载功能组件 [引用: zsEnum.BuildingType]
                        switch (bType)
                        {
                            case 1: // Core (核心类)
                                // 仅针对 VisitorCenter (Subtype 1) 挂载刷怪组件
                                if (subtype == 1)
                                {
                                    var cfgVisitor = GameConfigBridge.GetVisitorCenterConfig(req.ObjectId);
                                    EntityManager.AddComponentData(spawned, new VisitorCenterComponent
                                    {
                                        UnspawnedVisitorCount = (int)cfgVisitor.x,
                                        SpawnInterval = cfgVisitor.y
                                    });
                                }
                                break;

                            case 2: // Supply (供给类)
                                // TODO: 未来实现供给类逻辑 (如发电站)
                                break;

                            case 3: // Factory / Production
                                var factoryData = GameConfigBridge.GetFactoryConfig(req.ObjectId);

                                if (factoryData.IsValid)
                                {
                                    // 1. 填充生产组件 (新增字段)
                                    EntityManager.AddComponentData(spawned, new ProductionComponent
                                    {
                                        ProductionInterval = factoryData.ProductionInterval,
                                        MaxReserves = factoryData.MaxReserves,
                                        JobSlots = factoryData.JobSlots,             // [新增]
                                        DemandOccupation = factoryData.DemandOccupation, // [新增]
                                        IsActive = true,
                                        Timer = 0f
                                    });

                                    // 2. 填充岛屿亲和 Buffer [新增]
                                    var affinityBuffer = EntityManager.AddBuffer<IslandAffinityElement>(spawned);
                                    foreach (var affinity in factoryData.IslandAffinity)
                                    {
                                        affinityBuffer.Add(new IslandAffinityElement { IslandType = affinity });
                                    }

                                    // 3. 填充原料 Buffer
                                    var inputBuffer = EntityManager.AddBuffer<ProductionInputElement>(spawned);
                                    foreach (var input in factoryData.Inputs)
                                    {
                                        inputBuffer.Add(new ProductionInputElement { ItemId = input.x, Count = input.y });
                                    }

                                    // 4. 填充产出 Buffer (注意结构变化)
                                    var outputBuffer = EntityManager.AddBuffer<ProductionOutputElement>(spawned);
                                    foreach (var output in factoryData.Outputs)
                                    {
                                        outputBuffer.Add(new ProductionOutputElement
                                        {
                                            ItemId = output.x,
                                            CountPerCycle = output.y, // 配置产出量
                                            CurrentStorage = 0        // 初始库存为0
                                        });
                                    }
                                }
                                else
                                {
                                    Debug.LogWarning($"[Spawn] 建筑 {req.ObjectId} 产出配置无效。");
                                }
                                break;

                            case 4: // Service (Type 4)
                                // A. 挂载生产功能 (后厂 - 保持不变)
                                var prodData = GameConfigBridge.GetFactoryConfig(req.ObjectId);
                                if (prodData.IsValid)
                                {
                                    EntityManager.AddComponentData(spawned, new ProductionComponent
                                    {
                                        ProductionInterval = prodData.ProductionInterval,
                                        MaxReserves = prodData.MaxReserves,
                                        JobSlots = prodData.JobSlots,
                                        DemandOccupation = prodData.DemandOccupation,
                                        IsActive = true,
                                        Timer = 0f
                                    });
                                    // ... (挂载 Production Buffer 代码保持不变) ...
                                    var inBuf = EntityManager.AddBuffer<ProductionInputElement>(spawned);
                                    foreach (var v in prodData.Inputs) inBuf.Add(new ProductionInputElement { ItemId = v.x, Count = v.y });
                                    var outBuf = EntityManager.AddBuffer<ProductionOutputElement>(spawned);
                                    foreach (var v in prodData.Outputs) outBuf.Add(new ProductionOutputElement { ItemId = v.x, CountPerCycle = v.y, CurrentStorage = 0 });
                                    var affBuf = EntityManager.AddBuffer<IslandAffinityElement>(spawned);
                                    foreach (var v in prodData.IslandAffinity) affBuf.Add(new IslandAffinityElement { IslandType = v });
                                }

                                // B. 挂载服务功能 (前店 - 【修改】)
                                var srvCfg = GameConfigBridge.GetServiceConfig(req.ObjectId);
                                if (srvCfg.Found)
                                {
                                    // 1. 挂载简化后的服务组件
                                    EntityManager.AddComponentData(spawned, new ServiceComponent
                                    {
                                        ServiceConfigId = req.ObjectId,
                                        ServiceTime = srvCfg.ServiceTime,
                                        MaxVisitorCapacity = srvCfg.MaxVisitorCapacity,
                                        OutputItemId = srvCfg.OutputItemId,
                                        OutputItemCount = srvCfg.OutputItemCount,
                                        IsActive = true,
                                        IsServing = false,
                                        ServiceTimer = 0f
                                    });

                                    // 2. 挂载游客队列 (仅此一个 Buffer)
                                    EntityManager.AddBuffer<ServiceQueueElement>(spawned);

                                    // 【移除】不再需要 ServiceSlotElement Buffer
                                }
                                break;

                            case 5: // Experience (体验类)
                                // TODO: 未来实现体验类逻辑 (如游乐设施)
                                break;

                                // case 6 (PublicBridge) 和 7 (StaffBridge) 通常由 PlacementType.Bridge 处理，
                                // 但如果通过 PlacementType.Building 放置，可在此处扩展。
                        }

                        processed = true;
                    }
                }
                else if (req.Type == PlacementType.Bridge)
                {
                    // ... (桥梁逻辑保持不变) ...
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

        // ... (其余辅助方法保持不变) ...
        private void InitializeIslandLogicAttributes(Entity islandEntity)
        {
            object rawCfg = GameConfigBridge.GetRandomFirstLevelIslandConfig();
            if (rawCfg != null)
            {
                int islandType = GameConfigBridge.GetIslandLevelType(rawCfg);
                int bonusType = GameConfigBridge.GetIslandLevelBonusType(rawCfg);
                int bonusValue = GameConfigBridge.GetIslandLevelValue(rawCfg);

                EntityManager.AddComponentData(islandEntity, new IslandDataComponent
                {
                    Level = 1,
                    IslandType = islandType,
                    BonusType = bonusType,
                    BonusValue = bonusValue
                });

                var buffer = EntityManager.AddBuffer<BuildableStructureElement>(islandEntity);
                var structures = GameConfigBridge.GetIslandLevelStructures(rawCfg);
                if (structures != null)
                {
                    foreach (var subtype in structures)
                    {
                        buffer.Add(new BuildableStructureElement { StructureType = subtype });
                    }
                }
            }
        }

        private bool TrySpawnObject(PlaceObjectRequest req, out Entity spawned)
        {
            spawned = Entity.Null;
            string path = GameConfigBridge.GetResourceName(req.ObjectId, (int)req.Type);
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