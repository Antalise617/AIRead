using Cysharp.Threading.Tasks;
using GameFramework.ECS.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace GameFramework.ECS.Systems
{
    public struct VisualGridTag : IComponentData { }

    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial class GridEntityVisualizationSystem : SystemBase
    {
        private GridSystem _gridSystem;
        private EntityFactory _entityFactory;

        private NativeList<Entity> _currentVisualEntities;
        private bool _isResourceLoaded = false;

        // [新增] 可视化模式枚举，用于状态管理
        private enum VisMode { None, LayerRange, BuildableOnly, BridgeableOnly }
        private VisMode _currentMode = VisMode.None;
        private int2 _currentRange = new int2(-1, -1);

        private const int GRID_VIEW_ID = 9001;
        private const string PREFAB_PATH = "Assets/Resources_moved/GridCell.prefab";

        protected override void OnCreate()
        {
            RequireForUpdate<GridConfigComponent>();
            _entityFactory = new EntityFactory(EntityManager);
            _currentVisualEntities = new NativeList<Entity>(Allocator.Persistent);
            LoadGridResource().Forget();
        }

        protected override void OnStartRunning()
        {
            _gridSystem = World.GetExistingSystemManaged<GridSystem>();
        }

        private async UniTaskVoid LoadGridResource()
        {
            var entity = await _entityFactory.LoadEntityArchetypeAsync(GRID_VIEW_ID, PREFAB_PATH);
            if (entity != Entity.Null)
            {
                _isResourceLoaded = true;
                // Debug.Log("[GridVis] 网格资源加载就绪");
            }
        }

        protected override void OnDestroy()
        {
            ClearCurrentGrid();
            if (_currentVisualEntities.IsCreated) _currentVisualEntities.Dispose();
            _entityFactory.Dispose();
        }

        protected override void OnUpdate() { }

        /// <summary>
        /// 模式A: 设置显示的层级范围 (用于放置岛屿)
        /// </summary>
        public void SetVisualizationRange(int yMin, int yMax)
        {
            if (!CheckPrerequisites()) return;

            // 如果是隐藏指令
            if (yMin < 0 || yMax < 0)
            {
                ClearCurrentGrid();
                _currentMode = VisMode.None;
                _currentRange = new int2(-1, -1);
                return;
            }

            // 状态去重
            if (_currentMode == VisMode.LayerRange && _currentRange.x == yMin && _currentRange.y == yMax) return;

            ClearCurrentGrid();
            _currentMode = VisMode.LayerRange;
            _currentRange = new int2(yMin, yMax);

            GenerateGridInRange(yMin, yMax);
        }

        /// <summary>
        /// 模式B: [新增] 显示所有可建造区域 (用于放置建筑)
        /// 复刻项目1逻辑：不考虑高度层，显示所有 IsBuildable 的格子
        /// </summary>
        public void ShowBuildableGrids()
        {
            if (!CheckPrerequisites()) return;

            // 状态去重
            if (_currentMode == VisMode.BuildableOnly) return;

            ClearCurrentGrid();
            _currentMode = VisMode.BuildableOnly;
            _currentRange = new int2(-1, -1); // 重置层级记录

            GenerateBuildableGrids();
        }

        // [新增]：显示所有可造桥区域（岛屿连接点）
        public void ShowBridgeableGrids()
        {
            if (!CheckPrerequisites()) return;

            // 状态去重
            if (_currentMode == VisMode.BridgeableOnly) return;

            ClearCurrentGrid();
            _currentMode = VisMode.BridgeableOnly;
            _currentRange = new int2(-1, -1); // 重置层级记录

            GenerateBridgeableGrids();
        }

        // [新增]：生成可造桥网格的具体逻辑
        private void GenerateBridgeableGrids()
        {
            var config = SystemAPI.GetSingleton<GridConfigComponent>();
            var boxGeometry = GetBoxGeometry(config);

            // 遍历全局网格，寻找 IsBridgeable = true 的格子
            foreach (var kvp in _gridSystem.WorldGrid)
            {
                var cellData = kvp.Value;

                // 核心筛选：只显示标记为“可造桥”且尚未被完全阻挡的格子
                // 注意：如果该格子已经被建筑占据(BuildingID不为空)，通常就不显示了，除非你的设计允许重叠
                if (cellData.IsBridgeable)
                {
                    SpawnSingleGridCell(cellData.WorldPosition, cellData.Position, boxGeometry);
                }

                // 扩展逻辑（可选）：如果你希望在此基础上，也显示已经造了桥的位置（方便连接），
                // 可以加上 || cellData.Type == GridType.PublicBridge
            }
            Debug.Log($"[GridVis] 显示桥梁锚点，数量: {_currentVisualEntities.Length}");
        }

        private bool CheckPrerequisites()
        {
            return _isResourceLoaded && _gridSystem != null && _gridSystem.WorldGrid.IsCreated;
        }

        // --- 生成逻辑 ---

        private void GenerateGridInRange(int yMin, int yMax)
        {
            var config = SystemAPI.GetSingleton<GridConfigComponent>();
            var boxGeometry = GetBoxGeometry(config);

            for (int y = yMin; y <= yMax; y++)
            {
                if (y >= config.Height) continue;
                for (int x = 0; x < config.Width; x++)
                {
                    for (int z = 0; z < config.Length; z++)
                    {
                        int3 gridPos = new int3(x, y, z);
                        if (_gridSystem.WorldGrid.TryGetValue(gridPos, out GridCellData cellData))
                        {
                            SpawnSingleGridCell(cellData.WorldPosition, gridPos, boxGeometry);
                        }
                    }
                }
            }
        }

        private void GenerateBuildableGrids()
        {
            var config = SystemAPI.GetSingleton<GridConfigComponent>();
            var boxGeometry = GetBoxGeometry(config);
            
            // 遍历整个哈希表，寻找 IsBuildable = true 的格子
            // 注意：这比遍历层级稍慢，但在 15万数据量级下 NativeHashMap 迭代非常快
            foreach (var kvp in _gridSystem.WorldGrid)
            {
                var cellData = kvp.Value;
                // 核心筛选逻辑：只显示可建造的
                if (cellData.IsBuildable)
                {
                    SpawnSingleGridCell(cellData.WorldPosition, cellData.Position, boxGeometry);
                }
            }
            Debug.Log($"[GridVis] 显示可建造区域，数量: {_currentVisualEntities.Length}");
        }

        private BoxGeometry GetBoxGeometry(GridConfigComponent config)
        {
            return new BoxGeometry
            {
                Size = new float3(2, 2, 0.1f),
                Center = float3.zero,
                Orientation = quaternion.identity
            };
        }

        private void SpawnSingleGridCell(float3 worldPos, int3 logicalPos, BoxGeometry colliderParams)
        {
            Entity e = _entityFactory.SpawnColliderEntity(
                GRID_VIEW_ID,
                worldPos += new float3(0, -1, 0),
                quaternion.RotateX(math.radians(90)),
                colliderParams,
                1.0f
            );

            if (e != Entity.Null)
            {
                EntityManager.AddComponentData(e, new GridPositionComponent { Value = logicalPos });
                EntityManager.AddComponent<VisualGridTag>(e);
                _currentVisualEntities.Add(e);
            }
        }

        private void ClearCurrentGrid()
        {
            if (!_currentVisualEntities.IsEmpty)
            {
                EntityManager.DestroyEntity(_currentVisualEntities.AsArray());
                _currentVisualEntities.Clear();
            }
        }
    }
}