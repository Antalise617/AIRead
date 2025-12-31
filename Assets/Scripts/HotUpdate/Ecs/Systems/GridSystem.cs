using System.Collections.Generic;
using cfg;
using GameFramework.ECS.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace GameFramework.ECS.Systems
{
    public partial class GridSystem : SystemBase
    {
        public NativeParallelHashMap<int3, GridCellData> WorldGrid;
        public NativeParallelHashMap<int3, Entity> BridgeEntityMap;
        private Random _random;
        private bool _isInitialized = false;

        // [新增] 用于 A* 寻路的邻居偏移量 (上下左右 + 上下层)
        private readonly int3[] _neighborOffsets = new int3[]
        {
            new int3(1, 0, 0), new int3(-1, 0, 0),
            new int3(0, 0, 1), new int3(0, 0, -1),
            // 如果您的桥梁或地形涉及高度变化（例如坡道），请保留以下两行；如果是纯平面移动，可以注释掉
            new int3(0, 1, 0), new int3(0, -1, 0)
        };

        protected override void OnCreate()
        {
            BridgeEntityMap = new NativeParallelHashMap<int3, Entity>(1000, Allocator.Persistent);
            WorldGrid = new NativeParallelHashMap<int3, GridCellData>(150000, Allocator.Persistent);
            _random = new Random(1234);

            if (!SystemAPI.HasSingleton<GridConfigComponent>())
            {
                EntityManager.CreateSingleton(new GridConfigComponent { Width = 50, Length = 50, Height = 15, CellSize = 2.0f });
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
            if (!_isInitialized && SystemAPI.HasSingleton<GridConfigComponent>())
            {
                InitializeGridData();
                _isInitialized = true;
            }
        }

        private void InitializeGridData()
        {
            var config = SystemAPI.GetSingleton<GridConfigComponent>();
            Debug.Log($"[GridSystem] 初始化网格数据: {config.Width}x{config.Height}x{config.Length}, CellSize:{config.CellSize}");
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
                        // 默认 IsMovable 为 false
                        WorldGrid.TryAdd(gridKey, new GridCellData { Position = gridKey, WorldPosition = worldPos, Type = GridType.Space, IsMovable = false });
                    }
                }
            }
        }

        public bool RegisterIsland(int3 anchorPos, Island data, int rotationIndex)
        {
            if (!WorldGrid.IsCreated || data == null) return false;

            int3 originalSize = new int3(data.Length, data.Height, data.Width);
            int3 finalSize = (rotationIndex % 2 == 1) ? new int3(originalSize.z, originalSize.y, originalSize.x) : originalSize;

            if (!CheckIslandPlacement(anchorPos, finalSize, data.AirHeight)) return false;

            FixedString64Bytes islandId = new FixedString64Bytes(data.Id.ToString());
            int3 startPos = new int3(anchorPos.x, anchorPos.y - finalSize.y + 1, anchorPos.z);

            // 本体
            for (int x = 0; x < finalSize.x; x++)
                for (int z = 0; z < finalSize.z; z++)
                    for (int h = 0; h < finalSize.y; h++)
                        UpdateCell(startPos + new int3(x, h, z), (ref GridCellData d) => { d.Type = GridType.Island; d.IslandID = islandId; });

            // 空域
            for (int x = 0; x < finalSize.x; x++)
                for (int z = 0; z < finalSize.z; z++)
                    for (int h = 1; h <= data.AirHeight; h++)
                        UpdateCell(anchorPos + new int3(x, h, z), (ref GridCellData d) => { d.Type = GridType.IslandAirspace; d.IslandID = islandId; });

            // 建造区 (允许移动)
            if (data.BuildArea != null)
            {
                foreach (var coord in data.BuildArea)
                {
                    int2 offset = RotateCoordinate(coord[0], coord[1], rotationIndex, data.Length, data.Width);
                    UpdateCell(anchorPos + new int3(offset.x, 1, offset.y), (ref GridCellData d) => { d.IsMovable = true; d.IsBuildable = true; });
                }
            }

            // 附着点
            if (data.AttachmentPoint != null)
            {
                foreach (var coord in data.AttachmentPoint)
                {
                    int2 offset = RotateCoordinate(coord[0], coord[1], rotationIndex, data.Length, data.Width);
                    UpdateCell(anchorPos + new int3(offset.x, 1, offset.y), (ref GridCellData d) => { d.IsBridgeable = true; });
                }
            }

            return true;
        }

        public bool CheckIslandPlacement(int3 anchorPos, int3 size, int airspace)
        {
            if (!WorldGrid.IsCreated) return false;
            int3 bottom = new int3(anchorPos.x, anchorPos.y - size.y + 1, anchorPos.z);
            int3 top = new int3(anchorPos.x + size.x - 1, anchorPos.y + airspace, anchorPos.z + size.z - 1);

            if (!IsInGridRange(bottom, top)) return false;
            return CheckAreaType(bottom, top, GridType.Space);
        }

        public bool RegisterBuilding(int3 pos, int3 size, FixedString64Bytes id)
        {
            int3 end = pos + size - new int3(1, 1, 1);
            if (!IsBuildingBuildable(pos, end)) return false;
            for (int x = 0; x < size.x; x++)
                for (int z = 0; z < size.z; z++)
                    for (int y = 0; y < size.y; y++)
                        UpdateCell(pos + new int3(x, y, z), (ref GridCellData d) =>
                        {
                            d.Type = GridType.Building;
                            d.BuildingID = id;
                            d.IsMovable = true; // 建筑占据的格子允许进入(例如排队)
                            d.IsBuildable = false;
                        });
            return true;
        }

        public bool RegisterBridge(int3 pos, FixedString64Bytes id)
        {
            if (!IsBridgeBuildable(pos)) return false;

            // 注册桥梁本体：设置为可移动
            UpdateCell(pos, (ref GridCellData d) =>
            {
                d.Type = GridType.PublicBridge;
                d.BuildingID = id;
                d.IsMovable = true; // 关键：桥必须可移动
                d.IsBuildable = false;
                d.IsBridgeable = false;
            });

            // 更新周围连接点
            int3[] offsets = { new int3(1, 0, 0), new int3(-1, 0, 0), new int3(0, 0, 1), new int3(0, 0, -1) };
            foreach (var off in offsets)
            {
                int3 n = pos + off;
                if (IsValidPosition(n)) UpdateCell(n, (ref GridCellData d) => { if (d.Type == GridType.Space) d.IsBridgeable = true; });
            }
            return true;
        }

        // ====================================================================================
        // [核心修复] 实现 A* 寻路算法
        // ====================================================================================
        public List<int3> FindPath(int3 start, int3 end)
        {
            if (!WorldGrid.ContainsKey(start) || !WorldGrid.ContainsKey(end))
            {
                Debug.LogWarning($"[GridSystem] 寻路失败：起点或终点不在网格内。{start} -> {end}");
                return null;
            }

            // 检查终点是否可达 (可选：如果终点是建筑中心，IsMovable 应该是 true)
            if (!WorldGrid[end].IsMovable)
            {
                // 如果终点不可移动（比如点击了墙壁），可以尝试寻找终点附近最近的可移动格子
                // 这里暂且直接返回失败
                // Debug.LogWarning($"[GridSystem] 终点不可移动: {end}");
                // return null; 
            }

            // A* 基础数据结构
            var openSet = new List<int3> { start };
            var cameFrom = new Dictionary<int3, int3>();
            var gScore = new Dictionary<int3, float> { { start, 0 } };
            var fScore = new Dictionary<int3, float> { { start, CalculateHeuristic(start, end) } };

            // 防止无限循环的安全阈值
            int maxSteps = 5000;
            int steps = 0;

            while (openSet.Count > 0)
            {
                if (steps++ > maxSteps)
                {
                    Debug.LogWarning("[GridSystem] 寻路步数过多，强行中止");
                    return null;
                }

                // 1. 获取 fScore 最小的节点
                int3 current = openSet[0];
                float minF = fScore.ContainsKey(current) ? fScore[current] : float.MaxValue;
                int minIndex = 0;

                for (int i = 1; i < openSet.Count; i++)
                {
                    float f = fScore.ContainsKey(openSet[i]) ? fScore[openSet[i]] : float.MaxValue;
                    if (f < minF)
                    {
                        current = openSet[i];
                        minF = f;
                        minIndex = i;
                    }
                }

                // 2. 到达终点
                if (current.Equals(end))
                {
                    return ReconstructPath(cameFrom, current);
                }

                openSet.RemoveAt(minIndex);

                // 3. 遍历邻居
                foreach (var neighbor in GetNeighbors(current))
                {
                    // 假设相邻格子的移动代价为 1
                    float tentativeG = gScore[current] + 1;

                    if (tentativeG < (gScore.ContainsKey(neighbor) ? gScore[neighbor] : float.MaxValue))
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentativeG;
                        fScore[neighbor] = tentativeG + CalculateHeuristic(neighbor, end);

                        if (!openSet.Contains(neighbor))
                        {
                            openSet.Add(neighbor);
                        }
                    }
                }
            }

            return null; // 路径未找到
        }

        private float CalculateHeuristic(int3 a, int3 b)
        {
            // 曼哈顿距离 (对于方格地图通常比欧几里得距离效果好)
            return math.abs(a.x - b.x) + math.abs(a.y - b.y) + math.abs(a.z - b.z);
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

        private IEnumerable<int3> GetNeighbors(int3 pos)
        {
            foreach (var offset in _neighborOffsets)
            {
                int3 nextPos = pos + offset;

                // 1. 边界检查
                if (!IsValidPosition(nextPos)) continue;

                // 2. 存在性检查
                if (WorldGrid.TryGetValue(nextPos, out GridCellData cell))
                {
                    // 3. 可移动性检查 (IsMovable 在 RegisterIsland/Building/Bridge 时设置)
                    if (cell.IsMovable)
                    {
                        yield return nextPos;
                    }
                }
            }
        }
        // ====================================================================================

        public bool IsBuildingBuildable(int3 start, int3 end)
        {
            if (!IsInGridRange(start, end)) return false;
            for (int x = start.x; x <= end.x; x++)
                for (int y = start.y; y <= end.y; y++)
                    for (int z = start.z; z <= end.z; z++)
                        if (!WorldGrid.TryGetValue(new int3(x, y, z), out var cell) || !cell.IsBuildable) return false;
            return true;
        }

        public bool IsBridgeBuildable(int3 pos)
        {
            if (!IsValidPosition(pos)) return false;
            if (WorldGrid.TryGetValue(pos, out var cell))
            {
                return cell.BuildingID == "" && (cell.IsBridgeable || cell.Type == GridType.Space);
            }
            return false;
        }

        public float3 CalculateObjectCenterWorldPosition(int3 anchorGridPos, int3 size)
        {
            if (!SystemAPI.HasSingleton<GridConfigComponent>()) return float3.zero;
            float s = SystemAPI.GetSingleton<GridConfigComponent>().CellSize;
            float3 anchor = new float3(anchorGridPos.x * s, anchorGridPos.y * s, anchorGridPos.z * s);
            float3 diag = new float3((anchorGridPos.x + size.x - 1) * s, anchorGridPos.y * s, (anchorGridPos.z + size.z - 1) * s);
            return (anchor + diag) * 0.5f;
        }

        private delegate void CellUpdate(ref GridCellData d);
        private void UpdateCell(int3 p, CellUpdate action) { if (WorldGrid.TryGetValue(p, out var d)) { action(ref d); WorldGrid[p] = d; } }

        public bool IsValidPosition(int3 p)
        {
            if (!SystemAPI.HasSingleton<GridConfigComponent>()) return false;
            var c = SystemAPI.GetSingleton<GridConfigComponent>();
            return p.x >= 0 && p.x < c.Width && p.y >= 0 && p.y < c.Height && p.z >= 0 && p.z < c.Length;
        }

        private bool IsInGridRange(int3 s, int3 e) => IsValidPosition(s) && IsValidPosition(e);

        private bool CheckAreaType(int3 s, int3 e, GridType t)
        {
            for (int x = s.x; x <= e.x; x++) for (int y = s.y; y <= e.y; y++) for (int z = s.z; z <= e.z; z++)
                        if (!WorldGrid.TryGetValue(new int3(x, y, z), out var c) || c.Type != t) return false;
            return true;
        }

        private int2 RotateCoordinate(int x, int z, int r, int w, int h)
        {
            switch (r % 4)
            {
                case 0: return new int2(x, z);
                case 1: return new int2(z, w - 1 - x);
                case 2: return new int2(w - 1 - x, h - 1 - z);
                case 3: return new int2(h - 1 - z, x);
                default: return new int2(x, z);
            }
        }
    }
}