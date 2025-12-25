using GameFramework.ECS.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace GameFramework.ECS.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class VisitorMovementSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float dt = SystemAPI.Time.DeltaTime;

            // 获取网格配置 (默认 1.0f)
            float cellSize = 2.0f;
            if (SystemAPI.HasSingleton<GridConfigComponent>())
            {
                cellSize = SystemAPI.GetSingleton<GridConfigComponent>().CellSize;
            }

            // 【修改 1】添加 .WithEntityAccess() 以便获取游客自身的 Entity
            foreach (var (transform, visitor, gridPos, pathBuffer, visitedBuffer, visitorEntity) in
                     SystemAPI.Query<RefRW<LocalTransform>, RefRW<VisitorComponent>, RefRW<GridPositionComponent>, DynamicBuffer<PathBufferElement>, DynamicBuffer<VisitedBuildingElement>>()
                     .WithEntityAccess())
            {
                if (visitor.ValueRO.CurrentState != VisitorState.Moving) continue;

                // 检查路径是否走完
                if (pathBuffer.IsEmpty)
                {
                    // === 到达目的地逻辑 ===
                    Entity targetBuilding = visitor.ValueRO.TargetBuildingEntity;
                    bool joinedQueue = false;

                    // 【修改 2】检查目标建筑是否存在且具备服务功能
                    if (targetBuilding != Entity.Null && SystemAPI.HasComponent<ServiceComponent>(targetBuilding))
                    {
                        // 获取建筑的服务组件和队列
                        var serviceComp = SystemAPI.GetComponent<ServiceComponent>(targetBuilding);

                        // 确保该建筑有队列Buffer (一般都有，但为了安全起见检查一下或者直接获取)
                        if (SystemAPI.HasBuffer<ServiceQueueElement>(targetBuilding))
                        {
                            var queue = SystemAPI.GetBuffer<ServiceQueueElement>(targetBuilding);

                            // 【修改 3】再次检查容量（防止路上队列满了）
                            if (queue.Length < serviceComp.MaxVisitorCapacity)
                            {
                                // 加入队列
                                queue.Add(new ServiceQueueElement { VisitorEntity = visitorEntity });

                                // 切换状态为 Waiting (排队中)
                                visitor.ValueRW.CurrentState = VisitorState.Waiting;
                                joinedQueue = true;

                                // Debug.Log($"[Movement] 游客 {visitor.ValueRO.Name} 已加入建筑 {targetBuilding.Index} 的排队队列 (当前: {queue.Length})");
                            }
                            else
                            {
                                // Debug.LogWarning($"[Movement] 游客 {visitor.ValueRO.Name} 到达但队列已满！");
                            }
                        }
                    }

                    // 如果没能成功加入队列（比如不是服务建筑，或者满了），则进入 Idle 状态
                    if (!joinedQueue)
                    {
                        visitor.ValueRW.CurrentState = VisitorState.Idle;
                        visitor.ValueRW.StateTimer = 2.0f; // 停留 2 秒后重新决策
                        visitor.ValueRW.TargetBuildingEntity = Entity.Null; // 清除无效目标
                    }

                    continue;
                }

                // === 移动逻辑 (保持不变) ===

                // 1. 获取下一个路点
                int3 targetGrid = pathBuffer[0].GridPos;

                // 2. 转换为世界坐标 (格子中心)
                float3 targetWorldPos = new float3(
                    targetGrid.x * cellSize,
                    targetGrid.y * cellSize + 0.5f, // 稍微抬高防止穿模
                    targetGrid.z * cellSize
                );

                // 3. 计算距离和移动
                float3 currentPos = transform.ValueRO.Position;
                float3 dir = targetWorldPos - currentPos;
                float dist = math.length(dir);
                float step = visitor.ValueRO.MoveSpeed * dt;

                if (dist <= step)
                {
                    // 到达该格子
                    transform.ValueRW.Position = targetWorldPos;
                    gridPos.ValueRW.Value = targetGrid; // 更新逻辑坐标
                    pathBuffer.RemoveAt(0);             // 移除该点，准备去下一个
                }
                else
                {
                    // 插值移动
                    float3 moveVec = math.normalize(dir) * step;
                    transform.ValueRW.Position += moveVec;

                    // 简单的朝向处理
                    if (math.lengthsq(moveVec) > 0.001f)
                    {
                        quaternion lookRot = quaternion.LookRotation(moveVec, math.up());
                        transform.ValueRW.Rotation = math.slerp(transform.ValueRO.Rotation, lookRot, dt * 10f);
                    }
                }
            }
        }
    }
}