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

                        int bType = (int)ConfigManager.Instance.Tables.TbBuild.GetOrDefault(req.ObjectId).BuildingType;
                        string nameStr = ConfigManager.Instance.Tables.TbBuild.GetOrDefault(req.ObjectId).Name;
                        int subtype = (int)ConfigManager.Instance.Tables.TbBuild.GetOrDefault(req.ObjectId).BuildingSubtype;

                        EntityManager.AddComponentData(spawned, new BuildingComponent
                        {
                            ConfigId = req.ObjectId,
                            Size = req.Size,
                            BuildingType = bType,
                            BuildingSubtype = subtype,
                            Name = new FixedString64Bytes(nameStr)
                        });

                        int prosperity = ConfigManager.Instance.Tables.TbBuildingLevel.GetOrDefault(req.ObjectId).Prosperity;
                        if (prosperity > 0)
                        {
                            EntityManager.AddComponentData(spawned, new ProsperityComponent { Value = prosperity });
                        }

                        int powerCost = ConfigManager.Instance.Tables.TbBuildingLevel.GetOrDefault(req.ObjectId).PowerConsumption;
                        if (powerCost > 0)
                        {
                            EntityManager.AddComponentData(spawned, new ElectricityComponent { PowerConsumption = powerCost, IsPowered = true });
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

                                var factoryData = GameConfigBridge.GetFactoryConfig(req.ObjectId);
                                if (factoryData.IsValid)
                                {
                                    EntityManager.AddComponentData(spawned, new ProductionComponent
                                    {
                                        ProductionInterval = factoryData.ProductionInterval,
                                        MaxReserves = factoryData.MaxReserves,
                                        JobSlots = factoryData.JobSlots,
                                        DemandOccupation = factoryData.DemandOccupation,
                                        IsActive = true,
                                        Timer = 0f
                                    });

                                    var affinityBuffer = EntityManager.AddBuffer<IslandAffinityElement>(spawned);
                                    foreach (var affinity in factoryData.IslandAffinity) affinityBuffer.Add(new IslandAffinityElement { IslandType = affinity });
                                    var inputBuffer = EntityManager.AddBuffer<ProductionInputElement>(spawned);
                                    foreach (var input in factoryData.Inputs) inputBuffer.Add(new ProductionInputElement { ItemId = input.x, Count = input.y });
                                    var outputBuffer = EntityManager.AddBuffer<ProductionOutputElement>(spawned);
                                    foreach (var output in factoryData.Outputs) outputBuffer.Add(new ProductionOutputElement { ItemId = output.x, CountPerCycle = output.y, CurrentStorage = 0 });

                                    // 【已移除】此处不再调用 UI 生成
                                }
                                else
                                {
                                    Debug.LogWarning($"[Spawn] 建筑 {req.ObjectId} 产出配置无效。");
                                }
                                break;

                            case 4: // Service
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
                                    var inBuf = EntityManager.AddBuffer<ProductionInputElement>(spawned);
                                    foreach (var v in prodData.Inputs) inBuf.Add(new ProductionInputElement { ItemId = v.x, Count = v.y });
                                    var outBuf = EntityManager.AddBuffer<ProductionOutputElement>(spawned);
                                    foreach (var v in prodData.Outputs) outBuf.Add(new ProductionOutputElement { ItemId = v.x, CountPerCycle = v.y, CurrentStorage = 0 });
                                    var affBuf = EntityManager.AddBuffer<IslandAffinityElement>(spawned);
                                    foreach (var v in prodData.IslandAffinity) affBuf.Add(new IslandAffinityElement { IslandType = v });
                                }

                                var srvCfg = GameConfigBridge.GetServiceConfig(req.ObjectId);
                                if (srvCfg.Found)
                                {
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
                                    EntityManager.AddBuffer<ServiceQueueElement>(spawned);
                                }

                                // 【已移除】此处不再调用 UI 生成
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