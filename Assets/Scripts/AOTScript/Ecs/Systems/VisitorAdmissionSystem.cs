using GameFramework.ECS.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace GameFramework.ECS.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(VisitorMovementSystem))]
    public partial class VisitorAdmissionSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            // 遍历所有“已到达”的游客
            foreach (var (visitorRef, gridPos, view, entity) in
                     SystemAPI.Query<RefRW<VisitorComponent>, RefRO<GridPositionComponent>, ViewInstanceComponent>()
                     .WithEntityAccess())
            {
                // 1. 状态检查
                if (visitorRef.ValueRO.CurrentState != VisitorState.Arrived) continue;

                // 2. 获取该游客心心念念的目标建筑
                Entity targetBuilding = visitorRef.ValueRO.TargetBuildingEntity;

                // 3. 严谨性校验：
                //    - 目标实体是否存在？
                //    - 目标实体是否真的有服务功能？(防止建筑中途被销毁或替换)
                if (targetBuilding != Entity.Null &&
                    EntityManager.Exists(targetBuilding) &&
                    EntityManager.HasComponent<ServiceComponent>(targetBuilding))
                {
                    // 获取建筑数据
                    var serviceComp = EntityManager.GetComponentData<ServiceComponent>(targetBuilding);
                    var queueBuffer = EntityManager.GetBuffer<ServiceQueueElement>(targetBuilding);

                    // 可选：距离校验（双重保险）
                    // 只有当游客逻辑坐标和建筑逻辑坐标距离很近时才允许进入
                    // 如果你的寻路只导航到建筑边缘，这里的距离容差可能需要设为 1 或 2（格子距离）
                    /*
                    var buildingPos = EntityManager.GetComponentData<GridPositionComponent>(targetBuilding).Value;
                    if (math.distance(gridPos.ValueRO.Value, buildingPos) > 3.0f) // 允许3格误差
                    {
                        // 距离太远，可能是寻路卡住了或者出bug了
                        continue;
                    }
                    */

                    // 4. 尝试进入队列
                    if (queueBuffer.Length < serviceComp.QueueCapacity)
                    {
                        // === 成功进入 ===

                        // 加入队列
                        queueBuffer.Add(new ServiceQueueElement { VisitorEntity = entity });

                        // 修改游客状态
                        visitorRef.ValueRW.CurrentState = VisitorState.Waiting;
                        visitorRef.ValueRW.TargetBuildingEntity = Entity.Null; // 清空目标，防止重复逻辑

                        // 隐藏模型
                        if (view.GameObject != null) view.GameObject.SetActive(false);

                        Debug.Log($"[Admission] 游客进入目标建筑。");
                    }
                    else
                    {
                        // === 队列满 ===
                        HandleRejection(ref visitorRef.ValueRW);
                    }
                }
                else
                {
                    // 目标建筑不存在了（被拆了？）或者不是服务建筑
                    HandleRejection(ref visitorRef.ValueRW);
                }
            }

            ecb.Playback(EntityManager);
            ecb.Dispose();
        }

        private void HandleRejection(ref VisitorComponent visitor)
        {
            visitor.CurrentState = VisitorState.Idle;
            visitor.TargetBuildingEntity = Entity.Null;
            visitor.StateTimer = 2.0f; // 发呆一会儿重新寻找
        }
    }
}