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

            // 遍历所有处于 Moving 状态的游客
            foreach (var (transform, visitor, gridPos, pathBuffer, visitedBuffer) in
                     SystemAPI.Query<RefRW<LocalTransform>, RefRW<VisitorComponent>, RefRW<GridPositionComponent>, DynamicBuffer<PathBufferElement>, DynamicBuffer<VisitedBuildingElement>>())
            {
                if (visitor.ValueRO.CurrentState != VisitorState.Moving) continue;

                // 检查路径是否走完
                if (pathBuffer.IsEmpty)
                {
                    // 路径走完 = 到达目的地
                    visitor.ValueRW.CurrentState = VisitorState.Idle;
                    visitor.ValueRW.StateTimer = 2.0f; // 在目的地停留 2 秒

                    // 记录已访问 (这里简单记录一下目标格子的假设ID，实际应该在 Behavior 阶段传过来)
                    // 为了简化，我们假设到了目的地就完成了访问逻辑
                    Debug.Log($"[Movement] 游客 {visitor.ValueRO.Name} 到达目的地!");
                    continue;
                }

                // === 移动逻辑 ===

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