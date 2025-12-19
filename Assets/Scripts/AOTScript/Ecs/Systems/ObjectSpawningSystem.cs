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

        // 定义常量对应 FunctionType (需与热更保持一致)
        private const int FUNC_VISITOR_CENTER = 1;

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
                        // ★ 通过 Bridge 获取 IslandData
                        var islandData = GameConfigBridge.GetIslandData(req.ObjectId);
                        if (islandData != null)
                        {
                            _gridSystem.RegisterIsland(req.Position, islandData, req.RotationIndex);
                            EntityManager.AddComponentData(spawned, new IslandComponent { ConfigId = req.ObjectId, Size = req.Size, AirSpace = req.AirspaceHeight });
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
                        if (fType == FUNC_VISITOR_CENTER)
                        {
                            var cfg = GameConfigBridge.GetVisitorCenterConfig(req.ObjectId);
                            EntityManager.AddComponentData(spawned, new VisitorCenterComponent { UnspawnedVisitorCount = (int)cfg.x, SpawnInterval = cfg.y });
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

        private bool TrySpawnObject(PlaceObjectRequest req, out Entity spawned)
        {
            spawned = Entity.Null;
            string path = GameConfigBridge.GetResourceName(req.ObjectId, (int)req.Type);

            if (string.IsNullOrEmpty(path)) return true;

            float3 pos = _gridSystem.CalculateObjectCenterWorldPosition(req.Position, req.Size);

            if (_entityFactory.HasEntity(req.ObjectId))
            {
                float s = _gridConfig.CellSize;
                float3 size = new float3(req.Size.x, req.Size.y, req.Size.z) * s;
                var box = new BoxGeometry { Center = float3.zero, Orientation = quaternion.identity, Size = new float3(size.x, 0.5f, size.z) };
                spawned = _entityFactory.SpawnColliderEntity(req.ObjectId, pos, req.Rotation, box);
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