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
                        var islandBaseData = GameConfigBridge.GetIslandData(req.ObjectId);
                        if (islandBaseData != null)
                        {
                            _gridSystem.RegisterIsland(req.Position, islandBaseData, req.RotationIndex);

                            // === 核心逻辑：随机初始化岛屿的等级与地表逻辑属性 ===
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
                        _gridSystem.RegisterBuilding(req.Position, req.Size, new FixedString64Bytes(req.ObjectId.ToString()));

                        int fType = GameConfigBridge.GetBuildingFunctionType(req.ObjectId);
                        switch (fType)
                        {
                            case 1:
                                var cfgVisitor = GameConfigBridge.GetVisitorCenterConfig(req.ObjectId);
                                EntityManager.AddComponentData(spawned, new VisitorCenterComponent
                                {
                                    UnspawnedVisitorCount = (int)cfgVisitor.x,
                                    SpawnInterval = cfgVisitor.y
                                });
                                break;
                            case 5:
                                var srvCfg = GameConfigBridge.GetServiceConfig(req.ObjectId);
                                if (srvCfg.Found)
                                {
                                    EntityManager.AddComponentData(spawned, new ServiceComponent
                                    {
                                        ServiceConfigId = req.ObjectId,
                                        ServiceTime = srvCfg.ServiceTime,
                                        QueueCapacity = srvCfg.QueueCapacity,
                                        MaxConcurrentNum = srvCfg.MaxConcurrentNum,
                                        OutputItemId = srvCfg.OutputItemId,
                                        OutputItemCount = srvCfg.OutputItemCount,
                                        IsActive = true
                                    });
                                    EntityManager.AddBuffer<ServiceQueueElement>(spawned);
                                    var slots = EntityManager.AddBuffer<ServiceSlotElement>(spawned);
                                    for (int k = 0; k < srvCfg.MaxConcurrentNum; k++)
                                    {
                                        slots.Add(new ServiceSlotElement { VisitorEntity = Entity.Null, Timer = 0f, IsOccupied = false });
                                    }
                                }
                                break;
                            case 6:
                                if (GameConfigBridge.TryGetFactoryConfig(req.ObjectId, out ProductionComponent prodConfig))
                                {
                                    prodConfig.Timer = 0f;
                                    prodConfig.CurrentReserves = 0;
                                    prodConfig.IsActive = true;
                                    EntityManager.AddComponentData(spawned, prodConfig);
                                }
                                break;
                        }

                        EntityManager.AddComponentData(spawned, new BuildingComponent { ConfigId = req.ObjectId, Size = req.Size, FuncType = fType });
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
            // 1. 拿到的是 object 类型的配置对象（避开 cfg 命名空间报错）
            object rawCfg = GameConfigBridge.GetRandomFirstLevelIslandConfig();

            if (rawCfg != null)
            {
                // 2. 通过 Bridge 定义的提取委托来取值（这些委托在热更侧被指向真正的 cfg 字段）
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

                // 3. 处理 Buffer 数据
                var buffer = EntityManager.AddBuffer<BuildableStructureElement>(islandEntity);
                var structures = GameConfigBridge.GetIslandLevelStructures(rawCfg);

                if (structures != null)
                {
                    foreach (var subtype in structures)
                    {
                        buffer.Add(new BuildableStructureElement { StructureType = subtype });
                    }
                }

                Debug.Log($"[IslandSpawn] 岛屿随机初始化属性成功！类型ID: {islandType}, 加成值: {bonusValue}");
            }
            else
            {
                Debug.LogWarning("[IslandSpawn] 未能获取随机岛屿配置，请检查 GameConfigBridge 委托是否注入！");
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