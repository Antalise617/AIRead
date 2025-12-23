using GameFramework.Core;
using Unity.Mathematics;
using cfg;
using GameFramework.ECS.Components;
using GameFramework.ECS.Systems;
using System.Collections.Generic;

namespace HotUpdate.Core
{
    public class HotfixConfigService : IGameConfigService
    {
        public int3 GetBuildingSize(int configId)
        {
            var cfg = ConfigManager.Instance.Tables.TbBuild.Get(configId);
            // 字段对应：Length(长), Width(宽)
            return cfg != null ? new int3(cfg.Length, 1, cfg.Width) : new int3(1, 1, 1);
        }

        public int3 GetIslandSize(int configId)
        {
            var cfg = ConfigManager.Instance.Tables.TbIsland.Get(configId);
            return cfg != null ? new int3(cfg.Length, cfg.Height, cfg.Width) : new int3(1, 1, 1);
        }

        public int GetIslandAirSpace(int configId)
        {
            return ConfigManager.Instance.Tables.TbIsland.Get(configId)?.AirHeight ?? 0;
        }

        public string GetResourceName(int configId, int typeInt)
        {
            PlacementType type = (PlacementType)typeInt;
            if (ConfigManager.Instance.Tables == null) return null;
            switch (type)
            {
                // 字段对应：ResourceName
                case PlacementType.Island: return ConfigManager.Instance.Tables.TbIsland.Get(configId)?.ResourceName;
                case PlacementType.Building: return ConfigManager.Instance.Tables.TbBuild.Get(configId)?.ResourceName;
                case PlacementType.Bridge: return ConfigManager.Instance.Tables.TbBridgeConfig.Get(configId)?.ResourceName;
            }
            return null;
        }

        public int GetBuildingFunctionType(int configId)
        {
            var cfg = ConfigManager.Instance.Tables.TbBuild.Get(configId);
            // 字段对应：BuildingType (枚举)
            return cfg != null ? (int)cfg.BuildingType : 0;
        }

        public float2 GetVisitorCenterConfig(int configId)
        {
            return new float2(5, 2.0f);
        }

        public IslandData GetIslandData(int configId)
        {
            var cfg = ConfigManager.Instance.Tables.TbIsland.Get(configId);
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

            // 处理 BuildArea 列表 (List<List<int>>)
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

        /// <summary>
        /// 【核心修改】从建筑等级表中获取生产配置
        /// </summary>
        public bool TryGetFactoryConfig(int buildingId, out ProductionComponent config)
        {
            config = default;
            var tables = ConfigManager.Instance.Tables;
            if (tables == null) return false;

            // 1. 生产数据现在存放在 TbBuildingLevel 中
            // 假设 buildingId 对应等级表的 ID，且目前默认取 1 级或特定的条目
            // 注意：等级表通常是复合 Key (ID+Level) 或者 ID 代表特定等级的行
            var factoryData = tables.TbBuildingLevel.GetOrDefault(buildingId);

            if (factoryData == null)
            {
                return false;
            }

            // 2. 映射产出与消耗数据 (假设 InputItem 和 OutputItem 是 List<List<int>> 结构)
            config = new ProductionComponent
            {
                // 输入：取第一个列表的第一个元素为ID，第二个为数量
                InputItemId = (factoryData.ItemCost != null && factoryData.ItemCost.Count > 0 && factoryData.ItemCost[0].Count > 0)
                                ? factoryData.ItemCost[0][0] : 0,
                InputCount = (factoryData.ItemCost != null && factoryData.ItemCost.Count > 0 && factoryData.ItemCost[0].Count > 1)
                                ? factoryData.ItemCost[0][1] : 0,

                // 输出：取第一个列表的第一个元素为ID，第二个为数量
                OutputItemId = (factoryData.OutPut != null && factoryData.OutPut.Count > 0 && factoryData.OutPut[0].Count > 0)
                                 ? factoryData.OutPut[0][0] : 0,
                OutputCount = (factoryData.OutPut != null && factoryData.OutPut.Count > 0 && factoryData.OutPut[0].Count > 1)
                                 ? factoryData.OutPut[0][1] : 0,

                ProductionInterval = factoryData.OutputCycle,       // 对应等级表中的 Time 字段
                MaxReserves = 5000,       // 对应等级表中的 MaxReserves 字段

                // 运行时初始状态
                IsActive = true,
                Timer = 0f,
                CurrentReserves = 0
            };

            return true;
        }

        public ServiceBuildingInfo GetServiceConfig(int buildingId)
        {
            var info = new ServiceBuildingInfo { Found = false };
            var tables = ConfigManager.Instance.Tables;
            if (tables == null) return info;

            var buildingCfg = tables.TbBuild.GetOrDefault(buildingId);
            if (buildingCfg == null) return info;

            // 根据新配表的 FunctionType 枚举判断
            if (buildingCfg.BuildingType != cfg.zsEnum.BuildingType.Service)
                return info;

            // 服务类数据建议也从 TbBuildingLevel 中读取相应字段
            var levelData = tables.TbBuildingLevel.GetOrDefault(buildingId);
            if (levelData != null)
            {
                info.Found = true;
                info.ServiceTime = levelData.DwellTime;
                info.QueueCapacity = 10; // 默认值
                info.MaxConcurrentNum = 1; // 默认值
                info.OutputItemId = (levelData.OutPut.Count > 0) ? levelData.OutPut[0][0] : 0;
                info.OutputItemCount = (levelData.OutPut.Count > 0) ? levelData.OutPut[0][1] : 0;
            }

            return info;
        }
    }
}