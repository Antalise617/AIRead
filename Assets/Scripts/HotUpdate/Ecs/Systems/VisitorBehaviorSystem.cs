using GameFramework.ECS.Components;
using GameFramework.Managers;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace GameFramework.ECS.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class VisitorBehaviorSystem : SystemBase
    {
        private Random _random;

        // 定义建筑类型数字常量 (根据您的配置表调整)
        private const int TYPE_NONE = 0;
        private const int TYPE_VISITOR_CENTER = 1;
        private const int TYPE_AIRPORT = 2;
        private const int TYPE_SHOP = 3;

        protected override void OnCreate()
        {
            _random = new Random(1234);
        }

        protected override void OnUpdate()
        {
            float dt = SystemAPI.Time.DeltaTime;

            // 1. 缓存所有建筑数据 (Entity, ConfigId, Position)
            // 这样我们在遍历游客时可以快速筛选目标
            var buildingQuery = SystemAPI.QueryBuilder().WithAll<BuildingComponent, GridPositionComponent>().Build();
            var buildingEntities = buildingQuery.ToEntityArray(Allocator.Temp);
            var buildingComps = buildingQuery.ToComponentDataArray<BuildingComponent>(Allocator.Temp);
            var buildingPositions = buildingQuery.ToComponentDataArray<GridPositionComponent>(Allocator.Temp);

            // 2. 遍历所有处于 Idle 状态的游客
            foreach (var (visitor, gridPos, pathReq, visitedBuffer, entity) in
                     SystemAPI.Query<RefRW<VisitorComponent>, RefRO<GridPositionComponent>, RefRW<PathfindingRequest>, DynamicBuffer<VisitedBuildingElement>>()
                     .WithEntityAccess())
            {
                if (visitor.ValueRO.CurrentState != VisitorState.Idle) continue;

                // 处理停留倒计时
                if (visitor.ValueRO.StateTimer > 0)
                {
                    visitor.ValueRW.StateTimer -= dt;
                    continue;
                }

                // === AI 决策逻辑 ===
                // 目标：先访问 2 个商店 (Type=3)，然后去机场 (Type=2)
                int requiredShops = 2;
                int visitedShopCount = 0;

                for (int i = 0; i < visitedBuffer.Length; i++)
                {
                    int type = GetBuildingTypeAsInt(visitedBuffer[i].BuildingConfigId);
                    if (type == TYPE_SHOP) visitedShopCount++;
                }

                // 决定目标类型
                int targetType = (visitedShopCount < requiredShops) ? TYPE_SHOP : TYPE_AIRPORT;

                // === 寻找符合条件的建筑 ===
                Entity targetEntity = Entity.Null;
                int3 targetPos = int3.zero;
                int targetConfigId = 0;

                // 从所有建筑中筛选
                NativeList<int> candidates = new NativeList<int>(Allocator.Temp);
                for (int i = 0; i < buildingComps.Length; i++)
                {
                    int bType = GetBuildingTypeAsInt(buildingComps[i].ConfigId);

                    if (bType == targetType)
                    {
                        // 如果是商店，且已经去过这个具体的ID，则跳过 (不重复进同一家店)
                        if (targetType == TYPE_SHOP && HasVisited(visitedBuffer, buildingComps[i].ConfigId))
                            continue;

                        candidates.Add(i);
                    }
                }

                // 随机选择一个
                if (candidates.Length > 0)
                {
                    int index = candidates[_random.NextInt(candidates.Length)];
                    targetEntity = buildingEntities[index];
                    targetPos = buildingPositions[index].Value;
                    targetConfigId = buildingComps[index].ConfigId;
                }
                candidates.Dispose();

                // === 执行结果 ===
                if (targetEntity != Entity.Null)
                {
                    // 设置状态 -> 寻路中
                    visitor.ValueRW.CurrentState = VisitorState.Pathfinding;

                    // 发起请求
                    pathReq.ValueRW.StartPos = gridPos.ValueRO.Value;
                    pathReq.ValueRW.EndPos = targetPos;
                    pathReq.ValueRW.IsPending = true;

                    Debug.Log($"[Behavior] 游客 {visitor.ValueRO.Name} 决定去类型[{targetType}] (ID:{targetConfigId})");
                }
                else
                {
                    // 没找到目标 (比如没商店了)，原地发呆 2 秒再试
                    visitor.ValueRW.StateTimer = 2.0f;
                }
            }

            // 释放临时数组
            buildingEntities.Dispose();
            buildingComps.Dispose();
            buildingPositions.Dispose();
        }

        // 模拟查表方法：根据 ConfigId 返回 int 类型的建筑分类
        // 请根据您的实际 ConfigManager 逻辑修改此处
        private int GetBuildingTypeAsInt(int configId)
        {
            // 示例逻辑：根据ID范围硬编码，或者调用 ConfigManager
            // 假设：300000+ 是桥, 200000+ 是建筑
            // 200001 = 游客中心(1), 200002 = 机场(2), 200003 = 商店(3)

            if (configId == 200001) return TYPE_VISITOR_CENTER;
            if (configId == 200002) return TYPE_AIRPORT; // 机场
            if (configId >= 200003) return TYPE_SHOP;    // 商店

            // 也可以尝试通过 ConfigManager 获取（如果代码生成没报错）
            /*
            if (ConfigManager.Instance.Tables != null) {
                var cfg = ConfigManager.Instance.Tables.BuildingCfg.Get(configId);
                return (int)cfg.FunctionType; 
            }
            */
            return TYPE_NONE;
        }

        private bool HasVisited(DynamicBuffer<VisitedBuildingElement> buffer, int configId)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i].BuildingConfigId == configId) return true;
            }
            return false;
        }
    }
}