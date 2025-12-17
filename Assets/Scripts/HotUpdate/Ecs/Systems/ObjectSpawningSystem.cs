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
using cfg.building;

namespace GameFramework.ECS.Systems
{
    /// <summary>
    /// 对象生成系统 (扩展版：支持岛屿和建筑)
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class ObjectSpawningSystem : SystemBase // 建议类名改为 ObjectSpawningSystem
    {
        private GridSystem _gridSystem;
        private EntityFactory _entityFactory;
        private GridConfigComponent _gridConfig;

        // [新增] 引用可视化系统，用于刷新
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
            // [新增] 获取可视化系统
            _visSystem = World.GetExistingSystemManaged<GridEntityVisualizationSystem>();
            _gridConfig = SystemAPI.GetSingleton<GridConfigComponent>();
        }

        protected override void OnUpdate()
        {
            var query = SystemAPI.QueryBuilder().WithAll<PlaceObjectRequest>().Build();
            if (query.IsEmpty) return;

            var requestEntities = query.ToEntityArray(Allocator.Temp);
            var requests = query.ToComponentDataArray<PlaceObjectRequest>(Allocator.Temp);

            for (int i = 0; i < requestEntities.Length; i++)
            {
                var entity = requestEntities[i];
                var req = requests[i];

                bool isProcessed = false;

                // ---------------- 岛屿生成逻辑 ----------------
                if (req.Type == PlacementType.Island)
                {
                    // 1. 再次校验
                    if (!_gridSystem.CheckIslandPlacement(req.Position, req.Size, req.AirspaceHeight))
                    {
                        Debug.LogWarning($"[Spawning] 拒绝生成岛屿：位置 {req.Position} 无效");
                        EntityManager.DestroyEntity(entity);
                        continue;
                    }

                    // 2. 尝试生成
                    if (TrySpawnObject(req, out Entity spawnedEntity))
                    {
                        var config = ConfigManager.Instance.Tables.IslandCfg.Get(req.ObjectId);
                        if (config != null)
                        {
                            // 3. 注册网格 (岛屿特有逻辑)
                            _gridSystem.RegisterIsland(req.Position, config, req.RotationIndex);

                            // 4. 添加组件
                            EntityManager.AddComponentData(spawnedEntity, new IslandComponent
                            {
                                ConfigId = req.ObjectId,
                                Size = req.Size,
                                AirSpace = req.AirspaceHeight
                            });

                            isProcessed = true;
                        }
                    }
                }
                // ---------------- 建筑生成逻辑 (新增) ----------------
                else if (req.Type == PlacementType.Building)
                {
                    int3 endPos = req.Position + req.Size - new int3(1, 1, 1);

                    // 1. 再次校验
                    if (!_gridSystem.IsBuildingBuildable(req.Position, endPos))
                    {
                        Debug.LogWarning($"[Spawning] 拒绝生成建筑：位置 {req.Position} 不可建造");
                        EntityManager.DestroyEntity(entity);
                        continue;
                    }

                    // 2. 尝试生成
                    if (TrySpawnObject(req, out Entity spawnedEntity))
                    {
                        // 3. 注册网格
                        FixedString64Bytes buildingId = new FixedString64Bytes(req.ObjectId.ToString());
                        _gridSystem.RegisterBuilding(req.Position, req.Size, buildingId);

                        // [修改] 获取配置并填充组件
                        FunctionType fType = 0; // 默认为0或None
                        if (ConfigManager.Instance.Tables != null)
                        {
                            var config = ConfigManager.Instance.Tables.BuildingCfg.Get(req.ObjectId);
                            if (config != null)
                            {
                                fType = config.FunctionType;

                                // 根据类型挂载特定的功能组件
                                switch (config.FunctionType)
                                {
                                    case FunctionType.VisitorCenter:
                                        EntityManager.AddComponentData(spawnedEntity, new VisitorCenterComponent
                                        {
                                            UnspawnedVisitorCount = 5,
                                            SpawnInterval = 2.0f,
                                            SpawnTimer = 0f
                                        });
                                        Debug.Log($"[Spawning] 建筑 {req.ObjectId} 挂载 '游客中心' 组件");
                                        break;

                                    case FunctionType.Airport:
                                        Debug.Log($"[Spawning] 建筑 {req.ObjectId} 是航站楼");
                                        break;

                                    case FunctionType.Shop:
                                        Debug.Log($"[Spawning] 建筑 {req.ObjectId} 是商店");
                                        break;
                                }
                            }
                        }

                        // [修改] 添加 BuildingComponent，包含 FuncType
                        EntityManager.AddComponentData(spawnedEntity, new BuildingComponent
                        {
                            ConfigId = req.ObjectId,
                            Size = req.Size,
                            FuncType = fType // 记录枚举值
                        });

                        Debug.Log($"[Spawning] 建筑生成成功 ID:{req.ObjectId} 类型:{fType} @ {req.Position}");
                        isProcessed = true;
                    }
                }
                // ---------------- 桥梁生成逻辑 ----------------
                else if (req.Type == PlacementType.Bridge)
                {
                    // 1. 校验
                    if (!_gridSystem.IsBridgeBuildable(req.Position))
                    {
                        Debug.LogWarning($"[Spawning] 拒绝生成桥梁：位置 {req.Position} 无效");
                        EntityManager.DestroyEntity(entity);
                        continue;
                    }

                    // 2. 尝试生成
                    if (TrySpawnObject(req, out Entity spawnedEntity))
                    {
                        FixedString64Bytes bridgeId = new FixedString64Bytes(req.ObjectId.ToString());

                        // 3. 注册数据 (GridSystem 会自动处理邻居扩充)
                        _gridSystem.RegisterBridge(req.Position, bridgeId);

                        // [新增] 4. 立即刷新可视化网格
                        // 这样玩家点下鼠标后，立刻看到新造的桥周围亮起了绿格子
                        if (_visSystem != null)
                        {
                            _visSystem.ShowBridgeableGrids();
                        }

                        // 5. 添加组件
                        EntityManager.AddComponentData(spawnedEntity, new BridgeComponent
                        {
                            ConfigId = req.ObjectId
                        });

                        isProcessed = true;
                    }
                }

                // 如果处理成功（非加载等待中），则销毁请求实体
                if (isProcessed)
                {
                    EntityManager.DestroyEntity(entity);
                }
                // 如果 isProcessed 为 false，说明可能正在异步加载资源，保留 entity 下一帧重试
            }

            requestEntities.Dispose();
            requests.Dispose();
        }

        private bool TrySpawnObject(PlaceObjectRequest req, out Entity spawnedEntity)
        {
            spawnedEntity = Entity.Null;
            string resourcePath = "";

            if (ConfigManager.Instance.Tables != null)
            {
                switch (req.Type)
                {
                    case PlacementType.Island:
                        resourcePath = ConfigManager.Instance.Tables.IslandCfg.Get(req.ObjectId)?.ResourceName;
                        break;
                    case PlacementType.Building:
                        resourcePath = ConfigManager.Instance.Tables.BuildingCfg.Get(req.ObjectId)?.ResourceName;
                        break;
                    case PlacementType.Bridge:
                        string basePath = ConfigManager.Instance.Tables.BridgeCfg.Get(req.ObjectId)?.ResourceName;
                        if (!string.IsNullOrEmpty(basePath))
                        {
                            resourcePath = basePath;
                        }
                        break;
                }
            }

            if (string.IsNullOrEmpty(resourcePath))
            {
                Debug.LogError($"配置无效或资源路径缺失 ID: {req.ObjectId}");
                return true;
            }

            // B. 计算世界坐标
            float3 worldPos = _gridSystem.CalculateObjectCenterWorldPosition(req.Position, req.Size);
            quaternion rotation = req.Rotation;

            if (_entityFactory.HasEntity(req.ObjectId))
            {
                // 简化的碰撞体逻辑
                float cellSize = _gridConfig.CellSize > 0 ? _gridConfig.CellSize : 1f;
                float3 baseSize = new float3(req.Size.x, req.Size.y, req.Size.z) * cellSize;

                BoxGeometry boxGeometry = default;

                switch (req.Type)
                {
                    case PlacementType.Island:
                        // 岛屿：稍微厚一点，中心下沉，作为地面承载物体
                        boxGeometry = new BoxGeometry
                        {
                            Center = new float3(0, -2.1f, 0),
                            Orientation = quaternion.identity,
                            Size = new float3(baseSize.x, baseSize.y, baseSize.z),
                            BevelRadius = 0f
                        };
                        break;

                    case PlacementType.Bridge:
                        // 桥梁：很薄，铺在表面
                        boxGeometry = new BoxGeometry
                        {
                            Center = new float3(0, -1, 0),
                            Orientation = quaternion.identity,
                            Size = new float3(baseSize.x, baseSize.y, baseSize.z),
                            BevelRadius = 0f
                        };
                        break;

                    case PlacementType.Building:
                        // 建筑：完整包围盒，中心上移立在地面上
                        boxGeometry = new BoxGeometry
                        {
                            Center = new float3(0, 2, 0),
                            Orientation = quaternion.identity,
                            Size = baseSize,
                            BevelRadius = 0f
                        };
                        break;

                    default:
                        boxGeometry = new BoxGeometry
                        {
                            Center = float3.zero,
                            Orientation = quaternion.identity,
                            Size = baseSize,
                            BevelRadius = 0f
                        };
                        break;
                }

                spawnedEntity = _entityFactory.SpawnColliderEntity(
                    req.ObjectId,
                    worldPos,
                    rotation,
                    boxGeometry
                );
            }
            else
            {
                spawnedEntity = Entity.Null;
            }

            // D. 触发异步加载
            if (spawnedEntity == Entity.Null)
            {
                if (!_loadingAssets.Contains(req.ObjectId))
                {
                    _loadingAssets.Add(req.ObjectId);
                    // 传入修改后的 resourcePath (带 _zhidao 后缀的)
                    LoadAssetAndCleanState(req.ObjectId, resourcePath).Forget();
                }
                return false; // 等待加载
            }

            // E. 添加位置组件
            EntityManager.AddComponentData(spawnedEntity, new GridPositionComponent { Value = req.Position });

            return true;
        }

        private async UniTaskVoid LoadAssetAndCleanState(int id, string path)
        {
            await _entityFactory.LoadEntityArchetypeAsync(id, path);
            _loadingAssets.Remove(id);
        }
    }
}