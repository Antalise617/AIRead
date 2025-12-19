using GameFramework.Core;
using Unity.Mathematics;
using cfg.building;
using GameFramework.ECS.Components;
using GameFramework.ECS.Systems; // 引用 AOT 中的 IslandData
using System.Collections.Generic;

namespace HotUpdate.Core
{
    public class HotfixConfigService : IGameConfigService
    {
        public int3 GetBuildingSize(int configId)
        {
            var cfg = ConfigManager.Instance.Tables.BuildingCfg.Get(configId);
            // 【修改点】将 cfg.Size.X/Y 改为 cfg.Length 和 cfg.Width
            return cfg != null ? new int3(cfg.Length, 1, cfg.Width) : new int3(1, 1, 1);
        }

        public int3 GetIslandSize(int configId)
        {
            var cfg = ConfigManager.Instance.Tables.IslandCfg.Get(configId);
            return cfg != null ? new int3(cfg.Length, cfg.Height, cfg.Width) : new int3(1, 1, 1);
        }

        public int GetIslandAirSpace(int configId)
        {
            return ConfigManager.Instance.Tables.IslandCfg.Get(configId)?.AirHeight ?? 0;
        }

        public string GetResourceName(int configId, int typeInt)
        {
            PlacementType type = (PlacementType)typeInt;
            if (ConfigManager.Instance.Tables == null) return null;
            switch (type)
            {
                case PlacementType.Island: return ConfigManager.Instance.Tables.IslandCfg.Get(configId)?.ResourceName;
                case PlacementType.Building: return ConfigManager.Instance.Tables.BuildingCfg.Get(configId)?.ResourceName;
                case PlacementType.Bridge: return ConfigManager.Instance.Tables.BridgeCfg.Get(configId)?.ResourceName;
            }
            return null;
        }

        public int GetBuildingFunctionType(int configId)
        {
            var cfg = ConfigManager.Instance.Tables.BuildingCfg.Get(configId);
            return cfg != null ? (int)cfg.FunctionType : 0;
        }

        public float2 GetVisitorCenterConfig(int configId)
        {
            return new float2(5, 2.0f); // 默认配置
        }

        // 关键：构建 AOT 需要的 IslandData 对象
        public IslandData GetIslandData(int configId)
        {
            var cfg = ConfigManager.Instance.Tables.IslandCfg.Get(configId);
            if (cfg == null) return null;

            var data = new IslandData
            {
                Id = cfg.Id,
                Length = cfg.Length,
                Height = cfg.Height,
                Width = cfg.Width,
                AirHeight = cfg.AirHeight,
                BuildArea = new List<int2>(),
                AttachmentPoint = new List<int2>()
            };

            if (cfg.BuildArea != null)
            {
                foreach (var p in cfg.BuildArea)
                    if (p.Count >= 2) data.BuildArea.Add(new int2(p[0], p[1]));
            }

            if (cfg.AttachmentPoint != null)
            {
                foreach (var p in cfg.AttachmentPoint)
                    if (p.Count >= 2) data.AttachmentPoint.Add(new int2(p[0], p[1]));
            }

            return data;
        }

        public ProductionConfigData GetProductionConfig(int buildingConfigId)
        {
            var cfg = ConfigManager.Instance.Tables.BuildingCfg.Get(buildingConfigId);
            if (cfg != null)
            {
                // 这里模拟读取配置，实际请根据您的 Excel 表头修改字段名
                // 假设 ID 200003 是金矿 (无消耗，产出金币)
                if (buildingConfigId == 200003)
                {
                    return new ProductionConfigData
                    {
                        InputItemId = 0,
                        InputCount = 0,
                        OutputItemId = 1003, // 金币
                        OutputCount = 10,
                        Interval = 5.0f,
                        MaxReserves = 500
                    };
                }
                // 假设 ID 200004 是加工厂 (消耗木头，产出木板)
                else if (buildingConfigId == 200004)
                {
                    return new ProductionConfigData
                    {
                        InputItemId = 1001, // 木头
                        InputCount = 2,
                        OutputItemId = 1002, // 木板
                        OutputCount = 1,
                        Interval = 3.0f,
                        MaxReserves = 50
                    };
                }
            }
            return new ProductionConfigData { Interval = 9999f };
        }
    }
}