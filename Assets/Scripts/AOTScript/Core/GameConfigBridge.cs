using UnityEngine;
using Unity.Mathematics;
using GameFramework.ECS.Systems; // 引用 GridSystem 中的 IslandData

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

        bool TryGetFactoryConfig(int configId, out GameFramework.ECS.Components.ProductionComponent config);

    }

    public static class GameConfigBridge
    {
        public static IGameConfigService Service;

        public static int3 GetBuildingSize(int configId) => Service?.GetBuildingSize(configId) ?? new int3(1, 1, 1);
        public static int3 GetIslandSize(int configId) => Service?.GetIslandSize(configId) ?? new int3(1, 1, 1);
        public static int GetIslandAirSpace(int configId) => Service?.GetIslandAirSpace(configId) ?? 0;
        public static string GetResourceName(int configId, int type) => Service?.GetResourceName(configId, type);
        public static int GetBuildingFunctionType(int configId) => Service?.GetBuildingFunctionType(configId) ?? 0;
        public static float2 GetVisitorCenterConfig(int configId) => Service?.GetVisitorCenterConfig(configId) ?? float2.zero;
        public static IslandData GetIslandData(int configId) => Service?.GetIslandData(configId);

        // 在 GameConfigBridge 类中添加
        public static bool TryGetFactoryConfig(int configId, out GameFramework.ECS.Components.ProductionComponent config)
        {
            if (Service != null) return Service.TryGetFactoryConfig(configId, out config);
            config = default;
            return false;
        }
    }
}