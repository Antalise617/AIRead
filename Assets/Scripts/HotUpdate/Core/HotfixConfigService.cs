using System.Collections.Generic;
using cfg; // 引用 Luban 配置命名空间
using GameFramework.Core;
using GameFramework.ECS.Components;
using GameFramework.ECS.Systems;
using Unity.Mathematics;
using UnityEngine;

namespace HotUpdate.Core
{
    public class HotfixConfigService : IGameConfigService
    {
        public HotfixConfigService()
        {
            // 1. 注入 Service 实例
            GameConfigBridge.Service = this;

            // 2. 初始化额外的委托注入 (处理跨程序集 object 解析)
            InitializeBridgeDelegates();

            Debug.Log("[HotfixConfigService] 初始化完成并已注入 AOT Bridge。");
        }

        private void InitializeBridgeDelegates()
        {
            // 注入：随机获取一级岛屿等级配置
            GameConfigBridge.OnGetRandomFirstLevelIsland = () =>
            {
                var tables = ConfigManager.Instance.Tables;
                if (tables == null) return null;
                // 筛选 ID 最后一位为 1 的记录（对应各表层类型的1级形态）
                var pool = tables.TbIslandLevel.DataList.FindAll(x => x.Id % 10 == 1);
                return pool.Count > 0 ? pool[UnityEngine.Random.Range(0, pool.Count)] : null;
            };

            // 注入：从 object 中提取具体字段 (强转为热更侧可见的 cfg.IslandLevel)
            GameConfigBridge.OnGetIslandType = (obj) => (obj is IslandLevel c) ? (int)c.IslandType : 0;
            GameConfigBridge.OnGetIslandBonusType = (obj) => (obj is IslandLevel c) ? (int)c.BonusBuildingTypes : 0;
            GameConfigBridge.OnGetIslandValue = (obj) => (obj is IslandLevel c) ? c.Value : 0;
            GameConfigBridge.OnGetIslandStructures = (obj) => (obj is IslandLevel c) ? c.BuildableStructures : null;
        }

        // --- IGameConfigService 实现 ---

        public int3 GetBuildingSize(int configId)
        {
            var cfg = ConfigManager.Instance.Tables.TbBuild.GetOrDefault(configId);
            return cfg != null ? new int3(cfg.Length, 1, cfg.Width) : new int3(1, 1, 1);
        }

        public int3 GetIslandSize(int configId)
        {
            var cfg = ConfigManager.Instance.Tables.TbIsland.GetOrDefault(configId);
            return cfg != null ? new int3(cfg.Length, cfg.Height, cfg.Width) : new int3(1, 1, 1);
        }

        public int GetIslandAirSpace(int configId)
        {
            return ConfigManager.Instance.Tables.TbIsland.GetOrDefault(configId)?.AirHeight ?? 0;
        }

        public string GetResourceName(int configId, int typeInt)
        {
            var tables = ConfigManager.Instance.Tables;
            if (tables == null) return null;
            PlacementType type = (PlacementType)typeInt;
            switch (type)
            {
                case PlacementType.Island: return tables.TbIsland.GetOrDefault(configId)?.ResourceName;
                case PlacementType.Building: return tables.TbBuild.GetOrDefault(configId)?.ResourceName;
                case PlacementType.Bridge: return tables.TbBridgeConfig.GetOrDefault(configId)?.ResourceName;
                default: return null;
            }
        }

        public int GetBuildingFunctionType(int configId)
        {
            var cfg = ConfigManager.Instance.Tables.TbBuild.GetOrDefault(configId);
            return cfg != null ? (int)cfg.BuildingType : 0;
        }

        public float2 GetVisitorCenterConfig(int configId) => new float2(5, 2.0f);

        public IslandData GetIslandData(int configId)
        {
            var cfg = ConfigManager.Instance.Tables.TbIsland.GetOrDefault(configId);
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
            if (cfg.BuildArea != null) foreach (var p in cfg.BuildArea) if (p.Count >= 2) data.BuildArea.Add(new int2(p[0], p[1]));
            if (cfg.AttachmentPoint != null) foreach (var p in cfg.AttachmentPoint) if (p.Count >= 2) data.AttachmentPoint.Add(new int2(p[0], p[1]));
            return data;
        }

        public bool TryGetFactoryConfig(int configId, out ProductionComponent config)
        {
            config = default;
            var tables = ConfigManager.Instance.Tables;
            if (tables == null) return false;
            var factoryData = tables.TbBuildingLevel.GetOrDefault(configId);
            if (factoryData == null) return false;
            config = new ProductionComponent
            {
                InputItemId = (factoryData.ItemCost != null && factoryData.ItemCost.Count > 0 && factoryData.ItemCost[0].Count > 0) ? factoryData.ItemCost[0][0] : 0,
                InputCount = (factoryData.ItemCost != null && factoryData.ItemCost.Count > 0 && factoryData.ItemCost[0].Count > 1) ? factoryData.ItemCost[0][1] : 0,
                OutputItemId = (factoryData.OutPut != null && factoryData.OutPut.Count > 0 && factoryData.OutPut[0].Count > 0) ? factoryData.OutPut[0][0] : 0,
                OutputCount = (factoryData.OutPut != null && factoryData.OutPut.Count > 0 && factoryData.OutPut[0].Count > 1) ? factoryData.OutPut[0][1] : 0,
                ProductionInterval = factoryData.OutputCycle,
                MaxReserves = 5000,
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
            if (buildingCfg == null || buildingCfg.BuildingType != cfg.zsEnum.BuildingType.Service) return info;
            var levelData = tables.TbBuildingLevel.GetOrDefault(buildingId);
            if (levelData != null)
            {
                info.Found = true;
                info.ServiceTime = levelData.DwellTime;
                info.QueueCapacity = 10;
                info.MaxConcurrentNum = 1;
                info.OutputItemId = (levelData.OutPut.Count > 0) ? levelData.OutPut[0][0] : 0;
                info.OutputItemCount = (levelData.OutPut.Count > 0) ? levelData.OutPut[0][1] : 0;
            }
            return info;
        }
    }
}