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

        // === 【新增】接口实现 ===
        public string GetBuildingName(int configId)
        {
            var cfg = ConfigManager.Instance.Tables.TbBuild.GetOrDefault(configId);
            // 假设配置表中的 Name 字段就是 int 类型 (例如语言包ID)
            return cfg != null ? cfg.Name : string.Empty;
        }

        public int GetBuildingSubtype(int configId)
        {
            var cfg = ConfigManager.Instance.Tables.TbBuild.GetOrDefault(configId);
            // 将枚举强转为 int 返回
            return cfg != null ? (int)cfg.BuildingSubtype : 0;
        }
        // ========================

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

        public FactoryConfigDTO GetFactoryConfig(int configId)
        {
            var dto = new FactoryConfigDTO { IsValid = false };

            var tables = ConfigManager.Instance.Tables;
            if (tables == null) return dto;

            var levelData = tables.TbBuildingLevel.GetOrDefault(configId);
            if (levelData == null) return dto;

            // 1. 输入/输出 (保持不变)
            if (levelData.ItemCost != null)
            {
                foreach (var item in levelData.ItemCost)
                    if (item.Count >= 2) dto.Inputs.Add(new int2(item[0], item[1]));
            }
            if (levelData.OutPut != null)
            {
                foreach (var item in levelData.OutPut)
                    if (item.Count >= 2) dto.Outputs.Add(new int2(item[0], item[1]));
            }

            // 2. 基础属性
            dto.ProductionInterval = levelData.OutputCycle;

            int storage = 100;
            if (levelData.OutputStorage != null && levelData.OutputStorage.Count > 0)
            {
                var sItem = levelData.OutputStorage[0];
                storage = sItem.Count >= 2 ? sItem[1] : (sItem.Count >= 1 ? sItem[0] : 100);
            }
            dto.MaxReserves = storage;

            // 3. 【新增】读取岗位、职业、岛屿亲和
            dto.JobSlots = levelData.JobSlots;
            dto.DemandOccupation = (int)levelData.DemandOccupation; // 强转枚举

            if (levelData.IslandAffinity != null)
            {
                // Luban 生成的 List<IslandType> 枚举列表，转为 int
                foreach (var affinity in levelData.IslandAffinity)
                {
                    dto.IslandAffinity.Add((int)affinity);
                }
            }

            dto.IsValid = dto.Outputs.Count > 0;
            return dto;
        }

        public ServiceBuildingInfo GetServiceConfig(int buildingId)
        {
            var info = new ServiceBuildingInfo { Found = false };
            var tables = ConfigManager.Instance.Tables;
            if (tables == null) return info;

            // 1. 读取等级表
            var levelData = tables.TbBuildingLevel.GetOrDefault(buildingId);
            if (levelData == null) return info;

            // 2. 填充数据
            info.Found = true;
            info.ServiceTime = levelData.DwellTime > 0 ? levelData.DwellTime : 5.0f;

            // 【修改】直接读取 VisitorCapacity 作为总容量
            // 现在的逻辑是：这个值决定了建筑里能塞多少人，无论是在窗口办事还是在后面排队
            info.MaxVisitorCapacity = levelData.VisitorCapacity > 0 ? levelData.VisitorCapacity : 5;

            // 产出配置 (假设服务奖励金币 ID=1)
            info.OutputItemId = 1;
            info.OutputItemCount = 10;

            return info;
        }
        public int GetBuildingProsperity(int configId)
        {
            var cfg = ConfigManager.Instance.Tables.TbBuildingLevel.GetOrDefault(configId);
            return cfg != null ? cfg.Prosperity : 0;
        }

        // 【实现】获取耗电量
        public int GetBuildingPowerConsumption(int configId)
        {
            var cfg = ConfigManager.Instance.Tables.TbBuildingLevel.GetOrDefault(configId);
            return cfg != null ? cfg.PowerConsumption : 0;
        }
    }
}