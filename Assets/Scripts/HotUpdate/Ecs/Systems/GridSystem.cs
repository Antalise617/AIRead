using System.Collections.Generic;
using cfg;
using GameFramework.ECS.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine; // 引入 UnityEngine 以使用 Debug
using Random = Unity.Mathematics.Random;

namespace GameFramework.ECS.Systems
{
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
                        WorldGrid.TryAdd(gridKey, new GridCellData { Position = gridKey, WorldPosition = worldPos, Type = GridType.Space });
                    }
                }
            }
        }

        // [核心调试] 注册岛屿
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

        // [核心调试] 检查位置是否有效
        public bool CheckIslandPlacement(int3 anchorPos, int3 size, int airspace)
        {
            if (!WorldGrid.IsCreated)
            {
                Debug.LogError("[GridSystem] WorldGrid 尚未初始化！");
                return false;
            }

            // 岛屿是向下延伸的，所以底部坐标要减去高度
            int3 bottom = new int3(anchorPos.x, anchorPos.y - size.y + 1, anchorPos.z);
            int3 top = new int3(anchorPos.x + size.x - 1, anchorPos.y + airspace, anchorPos.z + size.z - 1);

            // 1. 检查边界
            if (!IsInGridRange(bottom, top))
            {
                var config = SystemAPI.GetSingleton<GridConfigComponent>();
                Debug.LogWarning($"[GridSystem] 放置失败：超出边界。Bottom:{bottom} Top:{top} MapLimit:({config.Width},{config.Height},{config.Length})");
                return false;
            }

            // 2. 检查空间是否已被占用
            if (!CheckAreaType(bottom, top, GridType.Space))
            {
                Debug.LogWarning($"[GridSystem] 放置失败：区域内已有其他物体。Range: {bottom} to {top}");
                return false;
            }

            return true;
        }

        // ... (其他方法保持不变: IsBuildingBuildable, IsBridgeBuildable, RegisterBuilding, UnregisterBuilding, UnregisterIsland 等)

        // 辅助方法
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

        // FindPath 等其他方法省略，确保类结构完整即可
        public List<int3> FindPath(int3 start, int3 end) => null; // 占位符，保持原文件有此方法的话
    }
}