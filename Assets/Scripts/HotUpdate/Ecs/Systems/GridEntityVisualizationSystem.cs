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
    //[UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial class GridEntityVisualizationSystem : SystemBase
    {
        private GridSystem _gridSystem;
        private EntityFactory _entityFactory;

        private NativeList<Entity> _currentVisualEntities;
        private bool _isResourceLoaded = false;

        // [修改] 可视化模式枚举，增加 WalkableOnly
        private enum VisMode { None, LayerRange, BuildableOnly, BridgeableOnly, WalkableOnly }
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
            }
        }

        protected override void OnDestroy()
        {
            ClearCurrentGrid();
            if (_currentVisualEntities.IsCreated) _currentVisualEntities.Dispose();
            _entityFactory.Dispose();
        }

        // [新增] Update 中检测 P 键输入
        protected override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                // 如果当前已经是显示可移动网格模式，则关闭
                if (_currentMode == VisMode.WalkableOnly)
                {
                    Debug.Log("[GridVis] 隐藏可移动网格");
                    SetVisualizationRange(-1, -1); // 复用此方法来清理和重置状态
                }
                else
                {
                    // 否则切换到显示可移动网格
                    Debug.Log("[GridVis] 显示可移动网格");
                    ShowWalkableGrids();
                }
            }
        }

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
        /// 模式B: 显示所有可建造区域 (用于放置建筑)
        /// </summary>
        public void ShowBuildableGrids()
        {
            if (!CheckPrerequisites()) return;

            if (_currentMode == VisMode.BuildableOnly) return;

            ClearCurrentGrid();
            _currentMode = VisMode.BuildableOnly;
            _currentRange = new int2(-1, -1);

            GenerateBuildableGrids();
        }

        /// <summary>
        /// 模式C: 显示所有可造桥区域（岛屿连接点）
        /// </summary>
        public void ShowBridgeableGrids(bool forceRefresh = false)
        {
            if (!CheckPrerequisites()) return;

            if (!forceRefresh && _currentMode == VisMode.BridgeableOnly) return;

            ClearCurrentGrid();
            _currentMode = VisMode.BridgeableOnly;
            _currentRange = new int2(-1, -1);

            GenerateBridgeableGrids();
        }

        // [新增] 模式D: 显示所有可移动（行走）区域
        public void ShowWalkableGrids()
        {
            if (!CheckPrerequisites()) return;

            if (_currentMode == VisMode.WalkableOnly) return;

            ClearCurrentGrid();
            _currentMode = VisMode.WalkableOnly;
            _currentRange = new int2(-1, -1);

            GenerateWalkableGrids();
        }

        // --- 生成逻辑 ---

        // [新增] 生成可移动网格的具体逻辑
        private void GenerateWalkableGrids()
        {
            var config = SystemAPI.GetSingleton<GridConfigComponent>();
            var boxGeometry = GetBoxGeometry(config);

            foreach (var kvp in _gridSystem.WorldGrid)
            {
                var cellData = kvp.Value;
                // 筛选 IsWalkable 为 true 的格子
                if (cellData.IsMovable)
                {
                    SpawnSingleGridCell(cellData.WorldPosition, cellData.Position, boxGeometry);
                }
            }
            Debug.Log($"[GridVis] 显示可移动区域，数量: {_currentVisualEntities.Length}");
        }

        private void GenerateBridgeableGrids()
        {
            var config = SystemAPI.GetSingleton<GridConfigComponent>();
            var boxGeometry = GetBoxGeometry(config);

            foreach (var kvp in _gridSystem.WorldGrid)
            {
                var cellData = kvp.Value;
                if (cellData.IsBridgeable)
                {
                    SpawnSingleGridCell(cellData.WorldPosition, cellData.Position, boxGeometry);
                }
            }
            Debug.Log($"[GridVis] 显示桥梁锚点，数量: {_currentVisualEntities.Length}");
        }

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

            foreach (var kvp in _gridSystem.WorldGrid)
            {
                var cellData = kvp.Value;
                if (cellData.IsBuildable)
                {
                    SpawnSingleGridCell(cellData.WorldPosition, cellData.Position, boxGeometry);
                }
            }
            Debug.Log($"[GridVis] 显示可建造区域，数量: {_currentVisualEntities.Length}");
        }

        private bool CheckPrerequisites()
        {
            return _isResourceLoaded && _gridSystem != null && _gridSystem.WorldGrid.IsCreated;
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