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

        public bool TryGetFactoryConfig(int buildingId, out ProductionComponent config)
        {
            config = default;
            var tables = ConfigManager.Instance.Tables;
            if (tables == null) return false;

            // 1. 先通过 buildingId 获取建筑配置
            var buildingData = tables.BuildingCfg.GetOrDefault(buildingId);
            if (buildingData == null) return false;

            // 2. 【核心修复】获取建筑配置里的 FunctionId (比如油田的FunctionId是210001)
            int factoryId = buildingData.FunctionId;

            // 3. 使用 factoryId 去查工厂配置表，而不是用 buildingId
            var factoryData = tables.FactoryCfg.GetOrDefault(factoryId);

            // 如果没找到，说明配表有误或该建筑没配生产数据
            if (factoryData == null)
            {
                // UnityEngine.Debug.LogWarning($"建筑 {buildingId} 的 FunctionId {factoryId} 在 FactoryCfg 中找不到对应配置！");
                return false;
            }

            // 4. 映射数据
            config = new ProductionComponent
            {
                // 注意：InputItem 是个列表，这里只取第一组作为演示，具体根据你的需求改为遍历或取特定索引
                InputItemId = (factoryData.InputItem != null && factoryData.InputItem.Count > 0 && factoryData.InputItem[0].Count > 0)
                              ? factoryData.InputItem[0][0] : 0,
                InputCount = (factoryData.InputItem != null && factoryData.InputItem.Count > 0 && factoryData.InputItem[0].Count > 1)
                              ? factoryData.InputItem[0][1] : 0,

                OutputItemId = (factoryData.OutputItem != null && factoryData.OutputItem.Count > 0 && factoryData.OutputItem[0].Count > 0)
                               ? factoryData.OutputItem[0][0] : 0,
                OutputCount = (factoryData.OutputItem != null && factoryData.OutputItem.Count > 0 && factoryData.OutputItem[0].Count > 1)
                               ? factoryData.OutputItem[0][1] : 0,

                ProductionInterval = factoryData.Time,
                MaxReserves = factoryData.MaxReserves,

                // 运行时初始状态
                IsActive = true,
                Timer = 0f,
                CurrentReserves = 0
            };

            return true;
        }
    }
}