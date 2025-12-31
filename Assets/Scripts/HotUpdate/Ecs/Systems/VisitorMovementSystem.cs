using GameFramework.ECS.Components;
using GameFramework.Managers;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using cfg;

namespace GameFramework.ECS.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class VisitorMovementSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float dt = SystemAPI.Time.DeltaTime;
            float cellSize = SystemAPI.HasSingleton<GridConfigComponent>() ?
                             SystemAPI.GetSingleton<GridConfigComponent>().CellSize : 1.0f;

            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (transform, visitor, gridPos, pathBuffer, visitedBuffer, visitorEntity) in
                     SystemAPI.Query<RefRW<LocalTransform>, RefRW<VisitorComponent>, RefRW<GridPositionComponent>, DynamicBuffer<PathBufferElement>, DynamicBuffer<VisitedBuildingElement>>()
                     .WithEntityAccess())
            {
                if (visitor.ValueRO.CurrentState != VisitorState.Moving) continue;

                // 1. 移动逻辑
                if (!pathBuffer.IsEmpty)
                {
                    int3 targetGrid = pathBuffer[0].GridPos;
                    float yOffset = 0.5f;
                    float3 targetWorldPos = new float3(
                        targetGrid.x * cellSize,
                        targetGrid.y * cellSize + yOffset,
                        targetGrid.z * cellSize
                    );

                    float3 currentPos = transform.ValueRO.Position;
                    float3 dir = targetWorldPos - currentPos;
                    float dist = math.length(dir);
                    float step = visitor.ValueRO.MoveSpeed * dt;

                    if (dist <= step)
                    {
                        transform.ValueRW.Position = targetWorldPos;
                        gridPos.ValueRW.Value = targetGrid;
                        pathBuffer.RemoveAt(0);
                    }
                    else
                    {
                        float3 moveVec = math.normalize(dir) * step;
                        transform.ValueRW.Position += moveVec;
                        if (math.lengthsq(moveVec) > 0.001f)
                        {
                            quaternion lookRot = quaternion.LookRotation(moveVec, math.up());
                            transform.ValueRW.Rotation = math.slerp(transform.ValueRO.Rotation, lookRot, dt * 15f);
                        }
                    }
                    continue; // 移动中，不处理到达
                }

                // 2. 到达逻辑
                Entity targetBuilding = visitor.ValueRO.TargetBuildingEntity;
                if (targetBuilding == Entity.Null || !SystemAPI.Exists(targetBuilding))
                {
                    EnterIdleState(visitor);
                    continue;
                }

                // 获取目标建筑属性
                int bType = 0;
                int bSubtype = 0;
                if (SystemAPI.HasComponent<BuildingComponent>(targetBuilding))
                {
                    var bComp = SystemAPI.GetComponent<BuildingComponent>(targetBuilding);
                    bType = bComp.BuildingType;
                    bSubtype = bComp.BuildingSubtype;
                }

                // [核心修改] 检查是否到达机场 (Type 1, Subtype 2)
                if (bType == 1 && bSubtype == 2)
                {
                    Debug.Log($"[Visitor] 游客 {visitor.ValueRO.Name} 到达机场，成功离岛。");
                    ecb.DestroyEntity(visitorEntity);
                    continue;
                }

                // 检查服务设施排队
                bool joinedQueue = false;
                if (SystemAPI.HasComponent<ServiceComponent>(targetBuilding))
                {
                    var serviceComp = SystemAPI.GetComponent<ServiceComponent>(targetBuilding);
                    var queue = SystemAPI.GetBuffer<ServiceQueueElement>(targetBuilding);

                    if (queue.Length < serviceComp.MaxVisitorCapacity)
                    {
                        queue.Add(new ServiceQueueElement { VisitorEntity = visitorEntity });
                        visitor.ValueRW.CurrentState = VisitorState.Waiting;
                        joinedQueue = true;
                    }
                }

                if (!joinedQueue)
                {
                    EnterIdleState(visitor);
                }
            }

            ecb.Playback(EntityManager);
            ecb.Dispose();
        }

        private void EnterIdleState(RefRW<VisitorComponent> visitor)
        {
            visitor.ValueRW.CurrentState = VisitorState.Idle;
            visitor.ValueRW.StateTimer = 2.0f;
            visitor.ValueRW.TargetBuildingEntity = Entity.Null;
        }
    }
}