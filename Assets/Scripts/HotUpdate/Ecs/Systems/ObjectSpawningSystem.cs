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

            // 只有当检测到请求时才打印，避免刷屏
            Debug.Log($"[ObjectSpawningSystem] 检测到 {entities.Length} 个生成请求...");

            for (int i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                var req = requests[i];
                bool processed = false;

                if (req.Type == PlacementType.Island)
                {
                    Debug.Log($"[Spawn-Island] 处理岛屿请求 ID:{req.ObjectId} Pos:{req.Position} Size:{req.Size}");

                    // 1. 检查位置是否合法
                    if (!_gridSystem.CheckIslandPlacement(req.Position, req.Size, req.AirspaceHeight))
                    {
                        Debug.LogError($"[Spawn-Island] ❌ 位置检测失败！坐标 {req.Position} 可能超出边界或重叠。");
                        EntityManager.DestroyEntity(entity);
                        continue;
                    }

                    // 2. 尝试生成物体 (包含资源加载)
                    if (TrySpawnObject(req, out Entity spawned))
                    {
                        Debug.Log($"[Spawn-Island] ✅ 实体生成成功！Entity: {spawned}");
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

                            // [新增] 传递状态机组件 (从 Request Entity -> Spawned Entity)
                            if (EntityManager.HasComponent<IslandStatusComponent>(entity))
                            {
                                var status = EntityManager.GetComponentData<IslandStatusComponent>(entity);
                                EntityManager.AddComponentData(spawned, status);
                            }
                            else
                            {
                                // 如果没有状态组件(例如本地放置)，给一个默认值
                                EntityManager.AddComponentData(spawned, new IslandStatusComponent
                                {
                                    State = 1,
                                    CreateTime = (long)System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1)).TotalSeconds
                                });
                            }

                            processed = true;
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"[Spawn-Island] ⏳ 资源未就绪，正在异步加载 ID:{req.ObjectId}...");
                    }
                }
                else if (req.Type == PlacementType.Building)
                {
                    // ========================================================================
                    // [关键修改] 打印详细日志，排查建筑位置错误
                    // ========================================================================
                    Debug.Log($"[Spawn-Building] 处理建筑请求 ID:{req.ObjectId} Pos:{req.Position} Size:{req.Size}");

                    int3 end = req.Position + req.Size - new int3(1, 1, 1);
                    if (!_gridSystem.IsBuildingBuildable(req.Position, end))
                    {
                        Debug.LogError($"[Spawn-Building] ❌ 建筑位置检测失败: Start:{req.Position} End:{end}");
                        EntityManager.DestroyEntity(entity);
                        continue;
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
                            Name = new FixedString64Bytes(buildCfg.Name)
                        });

                        processed = true;
                    }
                }
                else if (req.Type == PlacementType.Bridge)
                {
                    Debug.Log($"[Spawn-Bridge] 处理桥梁请求 ID:{req.ObjectId} Pos:{req.Position}");

                    if (!_gridSystem.IsBridgeBuildable(req.Position))
                    {
                        Debug.LogError($"[Spawn-Bridge] ❌ 桥梁位置检测失败: {req.Position}");
                        EntityManager.DestroyEntity(entity);
                        continue;
                    }

                    if (TrySpawnObject(req, out Entity spawned))
                    {
                        _gridSystem.RegisterBridge(req.Position, new FixedString64Bytes(req.ObjectId.ToString()));
                        processed = true;
                    }
                }

                if (processed)
                {
                    EntityManager.DestroyEntity(entity);
                }
            }
            entities.Dispose();
            requests.Dispose();
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

            // [Visual Offset] 岛屿视觉偏移 +1
            if (req.Type == PlacementType.Island)
            {
                worldPos.y += 1f;
            }

            // 检查 EntityFactory 是否已经缓存了该 Prefab 的 Archetype
            if (_entityFactory.HasEntity(req.ObjectId))
            {
                float s = _gridConfig.CellSize;
                float3 size = new float3(req.Size.x, req.Size.y, req.Size.z) * s;

                // [Collider Offset] 计算碰撞体中心偏移
                // 如果是岛屿，实体在 Y+1，为了让碰撞体贴合网格平面，碰撞体中心需要下移 1 (即 -1)
                float3 colliderCenter = float3.zero;
                if (req.Type == PlacementType.Island)
                {
                    colliderCenter = new float3(0, -1f, 0);
                }

                var box = new BoxGeometry { Center = colliderCenter, Orientation = quaternion.identity, Size = new float3(size.x, 0.5f, size.z) };

                // 真正生成 Entity
                spawned = _entityFactory.SpawnColliderEntity(req.ObjectId, worldPos, req.Rotation, box);
            }

            // 如果还没有缓存，或者生成失败
            if (spawned == Entity.Null)
            {
                if (!_loadingAssets.Contains(req.ObjectId))
                {
                    Debug.Log($"[Spawn] 📥 触发资源加载: [{req.ObjectId}] {path}");
                    _loadingAssets.Add(req.ObjectId);
                    LoadAsset(req.ObjectId, path).Forget();
                }
                return false; // 返回 false 表示本次未完成，下一帧继续尝试
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