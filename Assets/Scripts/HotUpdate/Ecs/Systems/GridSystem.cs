using System.Collections.Generic;
using cfg;
using GameFramework.ECS.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

namespace GameFramework.ECS.Systems
{

    //[UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    public partial class GridSystem : SystemBase
    {
        public NativeParallelHashMap<int3, GridCellData> WorldGrid;
        public NativeParallelHashMap<int3, Entity> BridgeEntityMap;
        private Random _random;
        private bool _isInitialized = false;

        protected override void OnCreate()
        {
            BridgeEntityMap = new NativeParallelHashMap<int3, Entity>(1000, Allocator.Persistent);
            WorldGrid = new NativeParallelHashMap<int3, GridCellData>(150000, Allocator.Persistent);
            _random = new Random(1234);

            if (!SystemAPI.HasSingleton<GridConfigComponent>())
            {
                EntityManager.CreateSingleton(new GridConfigComponent { Width = 100, Length = 100, Height = 15, CellSize = 2.0f });
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
                        WorldGrid.TryAdd(gridKey, new GridCellData { Position = gridKey, WorldPosition = worldPos, Type = GridType.Space });
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

            // 建造区
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

        // 其他关键逻辑保持不变...
        public bool CheckIslandPlacement(int3 anchorPos, int3 size, int airspace)
        {
            if (!WorldGrid.IsCreated) return false;
            int3 bottom = new int3(anchorPos.x, anchorPos.y - size.y + 1, anchorPos.z);
            int3 top = new int3(anchorPos.x + size.x - 1, anchorPos.y + airspace, anchorPos.z + size.z - 1);
            return IsInGridRange(bottom, top) && CheckAreaType(bottom, top, GridType.Space);
        }

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

        public bool RegisterBuilding(int3 pos, int3 size, FixedString64Bytes id)
        {
            int3 end = pos + size - new int3(1, 1, 1);
            if (!IsBuildingBuildable(pos, end)) return false;
            for (int x = 0; x < size.x; x++)
                for (int z = 0; z < size.z; z++)
                    for (int y = 0; y < size.y; y++)
                        UpdateCell(pos + new int3(x, y, z), (ref GridCellData d) => { d.Type = GridType.Building; d.BuildingID = id; d.IsMovable = true; d.IsBuildable = false; });
            return true;
        }

        /// <summary>
        /// 移除建筑物 (恢复网格状态)
        /// </summary>
        public bool UnregisterBuilding(int3 pos, int3 size, FixedString64Bytes buildingId)
        {
            if (!IsValidPosition(pos)) return false;

            for (int x = 0; x < size.x; x++)
            {
                for (int z = 0; z < size.z; z++)
                {
                    for (int y = 0; y < size.y; y++)
                    {
                        int3 current = pos + new int3(x, y, z);

                        // 只有当格子里的 ID 匹配时才重置，避免误删
                        if (WorldGrid.TryGetValue(current, out GridCellData cell) && cell.BuildingID == buildingId)
                        {
                            UpdateCell(current, (ref GridCellData data) => {
                                // 恢复为空地状态 (注意：这里假设移除后变成默认的可建造区域)
                                // 如果你的逻辑是建筑物下有地基，这里可能需要设为 GridType.Island
                                // 但为了通用，我们先设为可移动、可建造的 Space
                                data.Type = GridType.Space;
                                data.BuildingID = "";
                                data.IsMovable = true;
                                data.IsBuildable = true; // 允许再次建造
                                data.IsBridgeable = false;
                            });
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 移除岛屿 (清理整个区域)
        /// </summary>
        public bool UnregisterIsland(int3 anchorPos, int3 size, int airspace, FixedString64Bytes islandId)
        {
            if (!WorldGrid.IsCreated) return false;
            int3 startPos = new int3(anchorPos.x, anchorPos.y - size.y + 1, anchorPos.z);

            // 1. 清理本体
            for (int x = 0; x < size.x; x++)
                for (int z = 0; z < size.z; z++)
                    for (int h = 0; h < size.y; h++)
                        ResetCellIfMatch(startPos + new int3(x, h, z), islandId);

            // 2. 清理空域
            for (int x = 0; x < size.x; x++)
                for (int z = 0; z < size.z; z++)
                    for (int h = 1; h <= airspace; h++)
                        ResetCellIfMatch(anchorPos + new int3(x, h, z), islandId);

            // 3. 额外清理表面（防止残留的可建造标记）
            for (int x = 0; x < size.x; x++)
            {
                for (int z = 0; z < size.z; z++)
                {
                    int3 surfacePos = anchorPos + new int3(x, 1, z);
                    // 如果ID匹配，或者已经是空的但有残留标记
                    if (WorldGrid.TryGetValue(surfacePos, out GridCellData cell))
                    {
                        // 简单粗暴：直接重置该区域
                        UpdateCell(surfacePos, (ref GridCellData data) => {
                            data.IsMovable = false;
                            data.IsBuildable = false;
                            data.IsBridgeable = false;
                        });
                    }
                }
            }
            return true;
        }

        public bool RegisterBridge(int3 pos, FixedString64Bytes id)
        {
            
            if (!IsBridgeBuildable(pos)) return false;
            UpdateCell(pos, (ref GridCellData d) => { d.Type = GridType.PublicBridge; d.BuildingID = id; d.IsMovable = true; d.IsBuildable = false; d.IsBridgeable = false; });

            int3[] offsets = { new int3(1, 0, 0), new int3(-1, 0, 0), new int3(0, 0, 1), new int3(0, 0, -1) };
            foreach (var off in offsets)
            {
                int3 n = pos + off;
                if (IsValidPosition(n)) UpdateCell(n, (ref GridCellData d) => { if (d.Type == GridType.Space) d.IsBridgeable = true; });
            }
            return true;
        }

        public float3 CalculateObjectCenterWorldPosition(int3 anchorGridPos, int3 size)
        {
            if (!SystemAPI.HasSingleton<GridConfigComponent>()) return float3.zero;
            float s = SystemAPI.GetSingleton<GridConfigComponent>().CellSize;
            float3 anchor = new float3(anchorGridPos.x * s, anchorGridPos.y * s, anchorGridPos.z * s);
            float3 diag = new float3((anchorGridPos.x + size.x - 1) * s, anchorGridPos.y * s, (anchorGridPos.z + size.z - 1) * s);
            return (anchor + diag) * 0.5f;
        }

        public List<int3> FindPath(int3 start, int3 end)
        {
            // 基础检查
            if (!IsValidPosition(start) || !IsValidPosition(end)) return null;

            // 检查起终点是否可移动
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

        // --- A* 辅助方法 ---

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

        private List<int3> GetNeighbors(int3 pos)
        {
            List<int3> list = new List<int3>();
            int3[] dirs = { new int3(1, 0, 0), new int3(-1, 0, 0), new int3(0, 0, 1), new int3(0, 0, -1) };
            foreach (var dir in dirs)
            {
                int3 next = pos + dir;
                // 确保坐标有效，且格子属性为 IsMovable
                if (IsValidPosition(next) && WorldGrid.TryGetValue(next, out GridCellData cell) && cell.IsMovable)
                {
                    list.Add(next);
                }
            }
            return list;
        }

        // 辅助方法
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

        private void ResetCellIfMatch(int3 pos, FixedString64Bytes id)
        {
            // 检查位置有效性，且 ID 匹配才重置
            if (IsValidPosition(pos) && WorldGrid.TryGetValue(pos, out GridCellData cell) && cell.IslandID == id)
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
    }
}