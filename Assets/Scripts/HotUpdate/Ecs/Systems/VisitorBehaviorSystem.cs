using GameFramework.ECS.Components;
using GameFramework.Managers;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using GameFramework.Core;
// using cfg.zsEnum; // 【删除】AOT代码不能引用HotUpdate中的命名空间
using Random = Unity.Mathematics.Random;

namespace GameFramework.ECS.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class VisitorBehaviorSystem : SystemBase
    {
        private Random _random;

        // 【修改】手动定义常量，对应配置表中的 BuildingType 枚举值
        // BuildingType: Core=1, Supply=2, Output=3, Service=4, Experience=5
        private const int TYPE_NONE = 0;
        private const int TYPE_VISITOR_CENTER = 1; // 对应 BuildingType.Core
        private const int TYPE_AIRPORT = 2;        // 对应 BuildingType.Supply
        private const int TYPE_SERVICE = 4;        // 【修改】对应 BuildingType.Service (值固定为4)

        protected override void OnCreate()
        {
            _random = new Random(1234);
        }

        protected override void OnUpdate()
        {
            float dt = SystemAPI.Time.DeltaTime;

            // 1. 缓存所有建筑数据
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
                // 目标：先访问 2 个服务设施 (Type=4)，然后去机场 (Type=2)
                int requiredServices = 2;
                int visitedServiceCount = 0;

                for (int i = 0; i < visitedBuffer.Length; i++)
                {
                    int type = GetBuildingTypeAsInt(visitedBuffer[i].BuildingConfigId);
                    if (type == TYPE_SERVICE) visitedServiceCount++;
                }

                // 决定目标类型
                int targetType = (visitedServiceCount < requiredServices) ? TYPE_SERVICE : TYPE_AIRPORT;

                // === 寻找符合条件的建筑 ===
                Entity targetEntity = Entity.Null;
                int3 targetPos = int3.zero;
                int targetConfigId = 0;

                NativeList<int> candidates = new NativeList<int>(Allocator.Temp);
                for (int i = 0; i < buildingComps.Length; i++)
                {
                    Entity buildingEnt = buildingEntities[i];

                    // 检查是否是服务型建筑，并且队列是否已满
                    bool isQueueFull = false;
                    if (SystemAPI.HasComponent<ServiceComponent>(buildingEnt))
                    {
                        var serviceComp = SystemAPI.GetComponent<ServiceComponent>(buildingEnt);
                        var queueBuffer = SystemAPI.GetBuffer<ServiceQueueElement>(buildingEnt);

                        // 使用上一轮修复的正确字段名: MaxVisitorCapacity
                        if (queueBuffer.Length >= serviceComp.MaxVisitorCapacity)
                        {
                            isQueueFull = true;
                        }
                    }

                    int bType = GetBuildingTypeAsInt(buildingComps[i].ConfigId);

                    // 只有类型匹配 且 队列未满 且 未访问过(若是服务设施) 才加入候选
                    if (bType == targetType && !isQueueFull)
                    {
                        // 如果目标是服务设施，且已经访问过该特定建筑ID，则跳过
                        if (targetType == TYPE_SERVICE && HasVisited(visitedBuffer, buildingComps[i].ConfigId))
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
                    visitor.ValueRW.TargetBuildingEntity = targetEntity;

                    // 发起请求
                    pathReq.ValueRW.StartPos = gridPos.ValueRO.Value;
                    pathReq.ValueRW.EndPos = targetPos;
                    pathReq.ValueRW.IsPending = true;
                }
                else
                {
                    // 没找到目标，原地发呆 2 秒再试
                    visitor.ValueRW.StateTimer = 2.0f;
                }
            }

            // 释放临时数组
            buildingEntities.Dispose();
            buildingComps.Dispose();
            buildingPositions.Dispose();
        }

        private int GetBuildingTypeAsInt(int configId)
        {
            var cfg = ConfigManager.Instance.Tables.TbBuild.GetOrDefault(configId);
            return cfg != null ? (int)cfg.BuildingType : 0;
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