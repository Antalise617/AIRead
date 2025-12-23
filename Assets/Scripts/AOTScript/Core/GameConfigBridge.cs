using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using GameFramework.ECS.Systems; // 引用 IslandData 结构

namespace GameFramework.Core
{
    public interface IGameConfigService
    {
        int3 GetBuildingSize(int configId);
        int3 GetIslandSize(int configId);
        int GetIslandAirSpace(int configId);
        string GetResourceName(int configId, int type);

        // 获取建筑功能类型 (返回 int)
        int GetBuildingFunctionType(int configId);

        // 获取游客中心配置 (x=库存, y=间隔)
        float2 GetVisitorCenterConfig(int configId);

        // 获取完整的岛屿数据 (用于 GridSystem)
        IslandData GetIslandData(int configId);

        // 尝试获取工厂/生产配置
        bool TryGetFactoryConfig(int configId, out GameFramework.ECS.Components.ProductionComponent config);

        // 获取服务类建筑配置
        ServiceBuildingInfo GetServiceConfig(int buildingId);
    }

    public struct ServiceBuildingInfo
    {
        public bool Found;
        public float ServiceTime;
        public int QueueCapacity;
        public int MaxConcurrentNum;
        public int OutputItemId;
        public int OutputItemCount;
    }

    public static class GameConfigBridge
    {
        // 核心服务实例（由热更侧注入）
        public static IGameConfigService Service;

        // --- 基础接口转发 ---
        public static int3 GetBuildingSize(int configId) => Service?.GetBuildingSize(configId) ?? new int3(1, 1, 1);
        public static int3 GetIslandSize(int configId) => Service?.GetIslandSize(configId) ?? new int3(1, 1, 1);
        public static int GetIslandAirSpace(int configId) => Service?.GetIslandAirSpace(configId) ?? 0;
        public static string GetResourceName(int configId, int type) => Service?.GetResourceName(configId, type);
        public static int GetBuildingFunctionType(int configId) => Service?.GetBuildingFunctionType(configId) ?? 0;
        public static float2 GetVisitorCenterConfig(int configId) => Service?.GetVisitorCenterConfig(configId) ?? float2.zero;
        public static IslandData GetIslandData(int configId) => Service?.GetIslandData(configId);

        public static bool TryGetFactoryConfig(int configId, out GameFramework.ECS.Components.ProductionComponent config)
        {
            if (Service != null) return Service.TryGetFactoryConfig(configId, out config);
            config = default;
            return false;
        }

        public static ServiceBuildingInfo GetServiceConfig(int buildingId)
        {
            return Service?.GetServiceConfig(buildingId) ?? new ServiceBuildingInfo();
        }

        // --- 复杂对象及属性提取委托 (解决 cfg 命名空间不可见问题) ---

        // 1. 随机获取岛屿等级配置对象 (返回 object 类型)
        public delegate object GetRandomIslandConfigDelegate();
        public static GetRandomIslandConfigDelegate OnGetRandomFirstLevelIsland;
        public static object GetRandomFirstLevelIslandConfig() => OnGetRandomFirstLevelIsland?.Invoke();

        // 2. 从配置 object 中提取具体数值的委托
        public delegate int GetIntFromObjDelegate(object obj);
        public delegate List<int> GetListFromObjDelegate(object obj);

        public static GetIntFromObjDelegate OnGetIslandType;
        public static GetIntFromObjDelegate OnGetIslandBonusType;
        public static GetIntFromObjDelegate OnGetIslandValue;
        public static GetListFromObjDelegate OnGetIslandStructures;

        // 供 AOT 系统调用的安全方法
        public static int GetIslandLevelType(object obj) => OnGetIslandType?.Invoke(obj) ?? 0;
        public static int GetIslandLevelBonusType(object obj) => OnGetIslandBonusType?.Invoke(obj) ?? 0;
        public static int GetIslandLevelValue(object obj) => OnGetIslandValue?.Invoke(obj) ?? 0;
        public static List<int> GetIslandLevelStructures(object obj) => OnGetIslandStructures?.Invoke(obj);
    }
}