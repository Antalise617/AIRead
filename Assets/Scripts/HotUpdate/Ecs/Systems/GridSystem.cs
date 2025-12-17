using cfg;
using System.Collections.Generic;
using GameFramework.ECS.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace GameFramework.ECS.Systems
{
    /// <summary>
    /// 网格核心系统 (ECS版 - 集成配置表与状态系统)
    /// 负责：全局网格数据维护、注册/注销逻辑、寻路算法、坐标转换
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    public partial class GridSystem : SystemBase
    {
        // [核心数据] 全局网格数据容器
        public NativeParallelHashMap<int3, GridCellData> WorldGrid;
        public NativeParallelHashMap<int3, Entity> BridgeEntityMap;
        private Random _random;
        private bool _isInitialized = false;

        protected override void OnCreate()
        {
            BridgeEntityMap = new NativeParallelHashMap<int3, Entity>(1000, Allocator.Persistent);
            // 初始化 NativeHashMap (容量预估 100x100x15 = 150,000)
            WorldGrid = new NativeParallelHashMap<int3, GridCellData>(150000, Allocator.Persistent);
            _random = new Random(1234); // 随机种子

            // 自动创建默认配置 (防止空引用)
            if (!SystemAPI.HasSingleton<GridConfigComponent>())
            {
                EntityManager.CreateSingleton(new GridConfigComponent
                {
                    Width = 100,
                    Length = 100,
                    Height = 15,
                    CellSize = 2.0f
                });
                Debug.Log("[GridSystem] 创建默认配置 (100x100x15)");
            }
        }

        protected override void OnDestroy()
        {
            if (WorldGrid.IsCreated) WorldGrid.Dispose();
            if (BridgeEntityMap.IsCreated) BridgeEntityMap.Dispose();
            base.OnDestroy();
        }

        protected override void OnUpdate()
        {
            // 延迟初始化：等待配置组件就绪
            if (!_isInitialized && SystemAPI.HasSingleton<GridConfigComponent>())
            {
                InitializeGridData();
                _isInitialized = true;
            }
        }

        // ===========================================================================================
        // 1. 初始化
        // ===========================================================================================
        private void InitializeGridData()
        {
            var config = SystemAPI.GetSingleton<GridConfigComponent>();
            int totalCells = config.Width * config.Length * config.Height;
            if (WorldGrid.Capacity < totalCells) WorldGrid.Capacity = totalCells;

            for (int x = 0; x < config.Width; x++)
            {
                for (int z = 0; z < config.Length; z++)
                {
                    for (int h = 0; h < config.Height; h++)
                    {
                        int3 gridKey = new int3(x, h, z);
                        float3 worldPos = new float3(x * config.CellSize, h * config.CellSize, z * config.CellSize);

                        GridCellData data = new GridCellData
                        {
                            Position = gridKey,
                            WorldPosition = worldPos,
                            Type = GridType.Space,
                            IslandID = "",
                            BuildingID = "",
                            IsMovable = false,
                            IsBuildable = false,
                            IsBridgeable = false // 默认为不可造桥
                        };
                        WorldGrid.TryAdd(gridKey, data);
                    }
                }
            }
            Debug.Log($"[GridSystem] 网格数据初始化完成，共 {WorldGrid.Count()} 个格子");
        }

        // ===========================================================================================
        // 2. 核心注册/注销逻辑 (支持配置表)
        // ===========================================================================================

        /// <summary>
        /// 注册岛屿占用 (支持配置表数据和旋转)
        /// </summary>
        /// <param name="anchorPos">锚点位置</param>
        /// <param name="config">岛屿配置数据</param>
        /// <param name="rotationIndex">旋转角度索引 (0=0°, 1=90°, 2=180°, 3=270°)</param>
        public bool RegisterIsland(int3 anchorPos, Island_Config config, int rotationIndex)
        {
            if (!WorldGrid.IsCreated) return false;

            // 1. 计算旋转后的尺寸
            int3 originalSize = new int3(config.Length, config.Height, config.Width);
            // 如果旋转 90 或 270 度，交换 长(X) 和 宽(Z)
            int3 finalSize = (rotationIndex % 2 == 1)
                ? new int3(originalSize.z, originalSize.y, originalSize.x)
                : originalSize;

            // 2. 严格检测：确保位置合法且未被占用
            if (!CheckIslandPlacement(anchorPos, finalSize, config.AirHeight))
            {
                Debug.LogWarning($"[GridSystem] 岛屿注册失败: 位置 {anchorPos} 无效或已被占用。");
                return false;
            }

            FixedString64Bytes islandId = new FixedString64Bytes(config.Id.ToString());

            // 3. 基础占用设置 (设置岛屿本体和空域类型)
            // 实体底部 = 锚点Y - 高度 + 1
            int3 startPos = new int3(anchorPos.x, anchorPos.y - finalSize.y + 1, anchorPos.z);

            // A. 设置岛屿本体 (GridType.Island)
            for (int x = 0; x < finalSize.x; x++)
            {
                for (int z = 0; z < finalSize.z; z++)
                {
                    for (int h = 0; h < finalSize.y; h++)
                    {
                        int3 currentPos = new int3(startPos.x + x, startPos.y + h, startPos.z + z);
                        UpdateCell(currentPos, (ref GridCellData data) => {
                            data.Type = GridType.Island;
                            data.IslandID = islandId;
                            // 默认先全部重置，稍后根据配置开启
                            data.IsMovable = false;
                            data.IsBuildable = false;
                            data.IsBridgeable = false;
                        });
                    }
                }
            }

            // B. 设置空域 (GridType.IslandAirspace)
            for (int x = 0; x < finalSize.x; x++)
                for (int z = 0; z < finalSize.z; z++)
                    for (int h = 1; h <= config.AirHeight; h++)
                        UpdateCell(anchorPos + new int3(x, h, z), (ref GridCellData data) => {
                            data.Type = GridType.IslandAirspace;
                            data.IslandID = islandId;
                        });

            // 4. 应用配置表中的特殊区域 (BuildArea 和 AttachmentPoint)
            // 假设表面层位于锚点上方第一层 (anchor.y + 1)
            // 注意：这里我们只处理表面那一层的数据
            int surfaceY = anchorPos.y + 1; // 表面高度

            // C. 处理可建造区域 (BuildArea) -> 可移动 + 可建造
            if (config.BuildArea != null)
            {
                foreach (var coord in config.BuildArea)
                {
                    if (coord.Count < 2) continue;
                    // 读取局部坐标 (原始未旋转)
                    int localX = coord[0];
                    int localZ = coord[1];

                    // 计算旋转后的偏移
                    int2 rotatedOffset = RotateCoordinate(localX, localZ, rotationIndex, config.Length, config.Width);

                    // 计算世界网格坐标
                    int3 targetPos = anchorPos + new int3(rotatedOffset.x, 1, rotatedOffset.y); // 注意：这里相对锚点的偏移是 (x, 1, z)

                    UpdateCell(targetPos, (ref GridCellData data) => {
                        data.IsMovable = true;
                        data.IsBuildable = true;
                    });
                }
            }

            // D. 处理附着点 (AttachmentPoint) -> 可造桥
            if (config.AttachmentPoint != null)
            {
                foreach (var coord in config.AttachmentPoint)
                {
                    if (coord.Count < 2) continue;
                    int localX = coord[0];
                    int localZ = coord[1];

                    int2 rotatedOffset = RotateCoordinate(localX, localZ, rotationIndex, config.Length, config.Width);
                    int3 targetPos = anchorPos + new int3(rotatedOffset.x, 1, rotatedOffset.y);

                    UpdateCell(targetPos, (ref GridCellData data) => {
                        data.IsBridgeable = true;
                        // 附着点通常是连接点，所以也应该允许 NPC 移动上去
                    });
                }
            }

            Debug.Log($"[GridSystem] 岛屿 {config.Id} 注册成功 @ {anchorPos}，方向 {rotationIndex}");
            return true;
        }

        public bool UnregisterIsland(int3 anchorPos, int3 size, int airspace, FixedString64Bytes islandId)
        {
            if (!WorldGrid.IsCreated) return false;
            int3 startPos = new int3(anchorPos.x, anchorPos.y - size.y + 1, anchorPos.z);

            // 清理本体
            for (int x = 0; x < size.x; x++)
                for (int z = 0; z < size.z; z++)
                    for (int h = 0; h < size.y; h++)
                        ResetCellIfMatch(startPos + new int3(x, h, z), islandId);

            // 清理空域
            for (int x = 0; x < size.x; x++)
                for (int z = 0; z < size.z; z++)
                    for (int h = 1; h <= airspace; h++)
                        ResetCellIfMatch(anchorPos + new int3(x, h, z), islandId);

            // 清理表面属性 (简单暴力重置，不再细分BuildArea)
            // 实际项目中可能需要更精细的还原
            for (int x = 0; x < size.x; x++)
                for (int z = 0; z < size.z; z++)
                {
                    int3 surfacePos = anchorPos + new int3(x, 1, z);
                    UpdateCell(surfacePos, (ref GridCellData data) => {
                        data.IsMovable = false;
                        data.IsBuildable = false;
                        data.IsBridgeable = false;
                    });
                }
            return true;
        }

        /// <summary>
        /// 注册建筑
        /// </summary>
        public bool RegisterBuilding(int3 pos, int3 size, FixedString64Bytes buildingId)
        {
            int3 endPos = pos + size - new int3(1, 1, 1);
            if (!IsBuildingBuildable(pos, endPos)) return false;

            for (int x = 0; x < size.x; x++)
                for (int z = 0; z < size.z; z++)
                    for (int y = 0; y < size.y; y++)
                    {
                        UpdateCell(pos + new int3(x, y, z), (ref GridCellData data) => {
                            data.Type = GridType.Building;
                            data.BuildingID = buildingId;
                            data.IsMovable = true;
                            data.IsBuildable = false;
                            // 建筑占据后，通常不再作为造桥点，除非有特殊设计
                        });
                    }
            return true;
        }

        public bool UnregisterBuilding(int3 pos, int3 size, FixedString64Bytes buildingId)
        {
            for (int x = 0; x < size.x; x++)
                for (int z = 0; z < size.z; z++)
                    for (int y = 0; y < size.y; y++)
                    {
                        int3 current = pos + new int3(x, y, z);
                        if (WorldGrid.TryGetValue(current, out GridCellData cell) && cell.BuildingID == buildingId)
                        {
                            UpdateCell(current, (ref GridCellData data) => {
                                // 恢复为 Space (或根据层级恢复为 Island 表面)
                                // 这是一个简化处理，理想情况应该知道底下是什么
                                data.Type = GridType.Space;
                                data.BuildingID = "";
                                data.IsMovable = true;
                                data.IsBuildable = true;
                            });
                        }
                    }
            return true;
        }

        /// <summary>
        /// [复刻项目1] 根据锚点和尺寸计算物体的世界中心坐标
        /// 逻辑：(锚点世界坐标 + 对角点世界坐标) / 2
        /// </summary>
        /// <param name="anchorGridPos">左下角锚点逻辑坐标</param>
        /// <param name="size">物体尺寸 (已处理旋转)</param>
        /// <returns>物体中心的世界坐标</returns>
        public float3 CalculateObjectCenterWorldPosition(int3 anchorGridPos, int3 size)
        {
            if (!SystemAPI.HasSingleton<GridConfigComponent>()) return float3.zero;
            float cellSize = SystemAPI.GetSingleton<GridConfigComponent>().CellSize;

            // 1. 获取锚点格子中心的世界坐标 (GridToWorldPosition 默认返回格子中心)
            // 这里我们手动算一下基础坐标，假设 gridPos.x * cellSize 是格子的原点
            // 但为了与 GridEntityVisualizationSystem 对齐，通常 0,0 格子的中心在 (0,0,0) 或者 (0.5, 0, 0.5) * cellSize
            // 这里使用纯数学坐标： x * cellSize

            float3 anchorWorldPos = new float3(
                anchorGridPos.x * cellSize,
                anchorGridPos.y * cellSize,
                anchorGridPos.z * cellSize
            );

            // 2. 获取对角格子(右上角)的世界坐标
            // 对角格子的逻辑坐标 = 锚点 + 尺寸 - 1
            int3 diagonalGridPos = anchorGridPos + new int3(size.x - 1, 0, size.z - 1);

            float3 diagonalWorldPos = new float3(
                diagonalGridPos.x * cellSize,
                diagonalGridPos.y * cellSize,
                diagonalGridPos.z * cellSize
            );

            // 3. 计算中点
            return (anchorWorldPos + diagonalWorldPos) * 0.5f;
        }

        // ===========================================================================================
        // 3. 检测与查询 API
        // ===========================================================================================

        /// <summary>
        /// 检查岛屿放置是否合法
        /// </summary>
        public bool CheckIslandPlacement(int3 anchorPos, int3 size, int airspace)
        {
            if (!WorldGrid.IsCreated) return false;

            // 实体底部
            int3 bodyBottom = new int3(anchorPos.x, anchorPos.y - size.y + 1, anchorPos.z);
            // 整体最高点
            int3 totalTop = new int3(anchorPos.x + size.x - 1, anchorPos.y + airspace, anchorPos.z + size.z - 1);

            if (!IsInGridRange(bodyBottom, totalTop)) return false;

            return CheckAreaType(bodyBottom, totalTop, GridType.Space);
        }

        public bool IsBuildingBuildable(int3 start, int3 end)
        {
            if (!IsInGridRange(start, end)) return false;

            for (int x = start.x; x <= end.x; x++)
                for (int y = start.y; y <= end.y; y++)
                    for (int z = start.z; z <= end.z; z++)
                    {
                        if (!WorldGrid.TryGetValue(new int3(x, y, z), out GridCellData cell) || !cell.IsBuildable)
                            return false;
                    }
            return true;
        }

        /// <summary>
        /// 判断是否可造桥
        /// </summary>
        public bool IsBridgeBuildable(int3 pos)
        {
            if (!IsValidPosition(pos)) return false;

            if (WorldGrid.TryGetValue(pos, out GridCellData cell))
            {
                // 1. 如果已被占用且不是 Space，则不能造桥 (除非它是特殊的附着点且未造过东西，但通常占用后即不可造)
                // 这里逻辑：如果已经被建筑占了，肯定不能造
                if (cell.BuildingID != "") return false;

                // 2. 如果是附着点，允许
                if (cell.IsBridgeable) return true;

                // 3. 如果是完全空的 Space (水面/空中)，也允许
                if (cell.Type == GridType.Space) return true;
            }
            return false;
        }

        public NativeList<GridCellData> GetBridgeablePositions(Allocator allocator)
        {
            NativeList<GridCellData> results = new NativeList<GridCellData>(allocator);
            foreach (var kvp in WorldGrid)
            {
                var pos = kvp.Key;
                var cell = kvp.Value;
                // 寻找潜在造桥点：
                // A. 显式附着点且未被占用
                // B. 或者是空的 Space 且旁边有可移动区域 (桥梁延伸)
                if (cell.BuildingID == "")
                {
                    if (cell.IsBridgeable)
                    {
                        results.Add(cell);
                    }
                    else if (cell.Type == GridType.Space && HasMovableNeighbor(pos))
                    {
                        results.Add(cell);
                    }
                }
            }
            return results;
        }

        /// <summary>
        /// 注册桥梁：标记自身占用，并将周围的空位标记为可造桥锚点
        /// </summary>
        public bool RegisterBridge(int3 pos, FixedString64Bytes bridgeId)
        {
            // 1. 再次检查是否可造
            if (!IsBridgeBuildable(pos)) return false;

            // 2. 更新当前格子状态
            UpdateCell(pos, (ref GridCellData data) => {
                data.Type = GridType.PublicBridge;
                data.BuildingID = bridgeId;
                data.IsMovable = true;      // 桥梁是可行走区域
                data.IsBuildable = false;   // 桥梁上不能建房子
                data.IsBridgeable = false;  // 自身已被占用，不能再造桥（防止重叠）
            });

            // 3. [新增] 激活周围邻居为“可造桥区域” (延伸机制)
            int3[] neighborOffsets = {
                new int3(1, 0, 0), new int3(-1, 0, 0),
                new int3(0, 0, 1), new int3(0, 0, -1)
            };

            foreach (var offset in neighborOffsets)
            {
                int3 neighborPos = pos + offset;

                // 只有当邻居在网格范围内，且是空的空气/水面时，才激活锚点
                if (IsValidPosition(neighborPos))
                {
                    UpdateCell(neighborPos, (ref GridCellData data) => {
                        if (data.Type == GridType.Space)
                        {
                            data.IsBridgeable = true; // 标记为可造桥，虚影系统会识别这个标记
                        }
                    });
                }
            }

            return true;
        }

        // ===========================================================================================
        // 4. 辅助工具 API
        // ===========================================================================================

        public bool IsValidPosition(int3 pos)
        {
            if (!SystemAPI.HasSingleton<GridConfigComponent>()) return false;
            var config = SystemAPI.GetSingleton<GridConfigComponent>();
            return pos.x >= 0 && pos.x < config.Width &&
                   pos.y >= 0 && pos.y < config.Height &&
                   pos.z >= 0 && pos.z < config.Length;
        }

        private bool IsInGridRange(int3 start, int3 end)
        {
            return IsValidPosition(start) && IsValidPosition(end);
        }

        /// <summary>
        /// 辅助：计算旋转后的局部坐标
        /// </summary>
        /// <param name="x">原始X</param>
        /// <param name="z">原始Z</param>
        /// <param name="rotation">旋转次数 (0,1,2,3)</param>
        /// <param name="originalXSize">原始长度</param>
        /// <param name="originalZSize">原始宽度</param>
        private int2 RotateCoordinate(int x, int z, int rotation, int originalXSize, int originalZSize)
        {
            int w = originalXSize;
            int h = originalZSize;

            switch (rotation % 4)
            {
                case 0: return new int2(x, z);                 // 0度
                case 1: return new int2(z, w - 1 - x);         // 90度
                case 2: return new int2(w - 1 - x, h - 1 - z); // 180度
                case 3: return new int2(h - 1 - z, x);         // 270度
                default: return new int2(x, z);
            }
        }

        public float3 GridToWorldPosition(int3 gridPos, int3 size)
        {
            if (!SystemAPI.HasSingleton<GridConfigComponent>()) return float3.zero;
            float cellSize = SystemAPI.GetSingleton<GridConfigComponent>().CellSize;

            float x = gridPos.x * cellSize + (size.x * cellSize * 0.5f) - (cellSize * 0.5f);
            float y = gridPos.y * cellSize;
            float z = gridPos.z * cellSize + (size.z * cellSize * 0.5f) - (cellSize * 0.5f);

            return new float3(x, y, z);
        }

        public float3 GridToWorldPosition(int3 gridPos)
        {
            return GridToWorldPosition(gridPos, new int3(1, 1, 1));
        }

        public int3 WorldToGridPosition(float3 worldPos)
        {
            if (!SystemAPI.HasSingleton<GridConfigComponent>()) return int3.zero;
            float cellSize = SystemAPI.GetSingleton<GridConfigComponent>().CellSize;
            return new int3((int)math.round(worldPos.x / cellSize), (int)math.round(worldPos.y / cellSize), (int)math.round(worldPos.z / cellSize));
        }

        // ===========================================================================================
        // 5. A* 寻路
        // ===========================================================================================
        public List<int3> FindPath(int3 start, int3 end)
        {
            if (!IsValidPosition(start) || !IsValidPosition(end)) return null;

            if (!WorldGrid.TryGetValue(start, out GridCellData startCell) || !startCell.IsMovable) return null;
            if (!WorldGrid.TryGetValue(end, out GridCellData endCell) || !endCell.IsMovable) return null;

            var openSet = new List<int3> { start };
            var cameFrom = new Dictionary<int3, int3>();
            var gScore = new Dictionary<int3, int> { { start, 0 } };
            var fScore = new Dictionary<int3, int> { { start, ManhattanDistance(start, end) } };

            while (openSet.Count > 0)
            {
                int3 current = GetLowestF(openSet, fScore);

                if (current.Equals(end)) return ReconstructPath(cameFrom, current);

                openSet.Remove(current);

                foreach (int3 neighbor in GetNeighbors(current))
                {
                    int tentativeG = gScore[current] + 1;
                    if (tentativeG < gScore.GetValueOrDefault(neighbor, int.MaxValue))
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentativeG;
                        fScore[neighbor] = tentativeG + ManhattanDistance(neighbor, end);
                        if (!openSet.Contains(neighbor)) openSet.Add(neighbor);
                    }
                }
            }
            return null;
        }

        // --- 私有辅助方法 ---

        private delegate void CellUpdateDelegate(ref GridCellData data);
        private void UpdateCell(int3 pos, CellUpdateDelegate action)
        {
            if (WorldGrid.TryGetValue(pos, out GridCellData data))
            {
                action(ref data);
                WorldGrid[pos] = data;
            }
        }

        private void ResetCellIfMatch(int3 pos, FixedString64Bytes id)
        {
            if (WorldGrid.TryGetValue(pos, out GridCellData cell) && cell.IslandID == id)
            {
                UpdateCell(pos, (ref GridCellData data) => {
                    data.Type = GridType.Space;
                    data.IslandID = "";
                    data.IsMovable = false;
                    data.IsBuildable = false;
                    data.IsBridgeable = false;
                });
            }
        }

        private bool CheckAreaType(int3 start, int3 end, GridType targetType)
        {
            for (int x = start.x; x <= end.x; x++)
                for (int y = start.y; y <= end.y; y++)
                    for (int z = start.z; z <= end.z; z++)
                    {
                        if (!WorldGrid.TryGetValue(new int3(x, y, z), out GridCellData cell) || cell.Type != targetType)
                            return false;
                    }
            return true;
        }

        private bool HasMovableNeighbor(int3 pos)
        {
            int3[] dirs = { new int3(1, 0, 0), new int3(-1, 0, 0), new int3(0, 0, 1), new int3(0, 0, -1) };
            foreach (var dir in dirs)
            {
                if (WorldGrid.TryGetValue(pos + dir, out GridCellData n) && n.IsMovable) return true;
            }
            return false;
        }

        private List<int3> GetNeighbors(int3 pos)
        {
            List<int3> list = new List<int3>();
            int3[] dirs = { new int3(1, 0, 0), new int3(-1, 0, 0), new int3(0, 0, 1), new int3(0, 0, -1) };

            foreach (var dir in dirs)
            {
                int3 next = pos + dir;
                if (IsValidPosition(next) && WorldGrid[next].IsMovable) list.Add(next);
            }
            return list;
        }

        private int ManhattanDistance(int3 a, int3 b) => math.abs(a.x - b.x) + math.abs(a.y - b.y) + math.abs(a.z - b.z);

        private int3 GetLowestF(List<int3> openSet, Dictionary<int3, int> fScore)
        {
            int3 lowest = openSet[0];
            int minVal = fScore.GetValueOrDefault(lowest, int.MaxValue);
            for (int i = 1; i < openSet.Count; i++)
            {
                int val = fScore.GetValueOrDefault(openSet[i], int.MaxValue);
                if (val < minVal) { minVal = val; lowest = openSet[i]; }
            }
            return lowest;
        }

        private List<int3> ReconstructPath(Dictionary<int3, int3> cameFrom, int3 current)
        {
            var path = new List<int3> { current };
            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                path.Add(current);
            }
            path.Reverse();
            return path;
        }
    }

    public struct GridCellData
    {
        public int3 Position;
        public float3 WorldPosition;
        public GridType Type;
        public FixedString64Bytes IslandID;
        public FixedString64Bytes BuildingID;

        // 状态标志位
        public bool IsMovable;    // 是否可行走
        public bool IsBuildable;  // 是否可建造建筑
        public bool IsBridgeable; // [新增] 是否可建造桥梁 (岛屿附着点)
    }

    public enum GridType : byte
    {
        Space,
        Island,
        IslandAirspace,
        Building,
        PublicBridge,
        PrivateBridge
    }
}