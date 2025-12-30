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

            // 只有当检测到请求时才打印，避免刷屏
            Debug.Log($"[ObjectSpawningSystem] 本帧处理 {entities.Length} 个生成请求...");

            // ========================================================================
            // [关键修改] 第一轮循环：优先处理岛屿 (Islands)
            // 目的：确保岛屿的网格数据(GridData)在这一帧立刻注册，哪怕资源还在加载。
            // 这样同一帧或下一帧的建筑请求就能检测到合法的地基。
            // ========================================================================
            for (int i = 0; i < entities.Length; i++)
            {
                if (requests[i].Type == PlacementType.Island)
                {
                    ProcessIslandRequest(entities[i], requests[i]);
                }
            }

            // ========================================================================
            // [关键修改] 第二轮循环：处理其他物体 (Buildings, Bridges)
            // 此时，如果是同一帧进来的岛屿，其网格数据已经在上面被注册了。
            // ========================================================================
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
            // 1. 数据层注册 (仅需执行一次)
            // 如果这个请求还没进行过逻辑注册，先注册网格数据
            if (!_processedRequests.Contains(entity))
            {
                Debug.Log($"[Spawn-Island] 处理岛屿逻辑注册 ID:{req.ObjectId} Pos:{req.Position}");

                // 检查位置
                if (!_gridSystem.CheckIslandPlacement(req.Position, req.Size, req.AirspaceHeight))
                {
                    Debug.LogError($"[Spawn-Island] ❌ 位置检测失败！坐标 {req.Position} 可能超出边界或重叠。");
                    EntityManager.DestroyEntity(entity);
                    return;
                }

                // 获取配置并注册网格 (即使没有模型，网格数据也必须先占位)
                var islandBaseData = ConfigManager.Instance.Tables.TbIsland.GetOrDefault(req.ObjectId);
                if (islandBaseData != null)
                {
                    _gridSystem.RegisterIsland(req.Position, islandBaseData, req.RotationIndex);
                    // 标记为已处理逻辑，防止下一帧重复注册网格
                    _processedRequests.Add(entity);
                }
                else
                {
                    Debug.LogError($"[Spawn-Island] ❌ 找不到配置 ID:{req.ObjectId}");
                    EntityManager.DestroyEntity(entity);
                    return;
                }
            }

            // 2. 表现层生成 (尝试加载资源并生成 Entity)
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

                // 传递状态机组件
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

                // 完成任务：销毁请求实体，清理缓存
                EntityManager.DestroyEntity(entity);
                _processedRequests.Remove(entity);
            }
            else
            {
                // 资源未就绪，保持请求Entity存在，下一帧继续尝试 TrySpawnObject
                // 但因为 _processedRequests 已经包含了它，不会再次触发 RegisterIsland
                Debug.LogWarning($"[Spawn-Island] ⏳ 资源加载中 ID:{req.ObjectId}...");
            }
        }

        private void ProcessOtherRequest(Entity entity, PlaceObjectRequest req)
        {
            if (req.Type == PlacementType.Building)
            {
                Debug.Log($"[Spawn-Building] 处理建筑请求 ID:{req.ObjectId} Pos:{req.Position}");

                int3 end = req.Position + req.Size - new int3(1, 1, 1);
                // 此时 _gridSystem 应该已经包含刚才注册的岛屿数据了
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
                        Name = new FixedString64Bytes(buildCfg.Name)
                    });

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
                return true; // 返回 true 表示处理“完成”（实际上是失败了，但要销毁请求）
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