using GameFramework.Core; // 引用 GameInventoryBridge
using GameFramework.ECS.Components;
using Unity.Entities;
using UnityEngine;

namespace GameFramework.ECS.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class ServiceProcessingSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float dt = SystemAPI.Time.DeltaTime;

            // 遍历所有激活的服务型建筑
            foreach (var (serviceRef, queueBuffer, slotBuffer, buildingEntity) in
                     SystemAPI.Query<RefRW<ServiceComponent>, DynamicBuffer<ServiceQueueElement>, DynamicBuffer<ServiceSlotElement>>()
                     .WithEntityAccess())
            {
                ref var service = ref serviceRef.ValueRW;
                if (!service.IsActive) continue;

                // =========================================================
                // 【修复点】：将 Buffer 赋值给本地变量以解决 CS1654 错误
                // =========================================================
                var slots = slotBuffer;
                var queue = queueBuffer;

                // =========================================================
                // 1. 调度逻辑：尝试把排队的人放入空闲槽位
                // =========================================================

                // 使用本地变量 'queue' 和 'slots' 替代原来的变量名
                if (queue.Length > 0)
                {
                    // 遍历所有槽位寻找空位
                    for (int i = 0; i < slots.Length; i++)
                    {
                        if (queue.Length == 0) break;

                        // 获取当前槽位数据
                        ServiceSlotElement slot = slots[i];

                        // 如果槽位未被占用
                        if (!slot.IsOccupied)
                        {
                            // 从队列取出第一个游客
                            Entity visitorEntity = queue[0].VisitorEntity;
                            queue.RemoveAt(0); // 修改本地变量 queue

                            // 校验游客实体有效性
                            if (EntityManager.Exists(visitorEntity))
                            {
                                // 填充槽位
                                slot.VisitorEntity = visitorEntity;
                                slot.Timer = 0f;
                                slot.IsOccupied = true;

                                // 回写 Buffer (这里使用 slots[i] 就不会报错了)
                                slots[i] = slot;

                                // 更新游客状态 -> BeingServed
                                if (SystemAPI.HasComponent<VisitorComponent>(visitorEntity))
                                {
                                    var visitor = SystemAPI.GetComponentRW<VisitorComponent>(visitorEntity);
                                    visitor.ValueRW.CurrentState = VisitorState.BeingServed;
                                }
                            }
                        }
                    }
                }

                // =========================================================
                // 2. 服务处理逻辑：遍历槽位进行计时和结算
                // =========================================================

                for (int i = 0; i < slots.Length; i++)
                {
                    ServiceSlotElement slot = slots[i];

                    if (slot.IsOccupied)
                    {
                        slot.Timer += dt;

                        // 检查是否完成服务
                        if (slot.Timer >= service.ServiceTime)
                        {
                            // A. 发放产出奖励
                            if (service.OutputItemId > 0 && service.OutputItemCount > 0)
                            {
                                GameInventoryBridge.AddItem(service.OutputItemId, service.OutputItemCount);
                            }

                            // B. 释放游客
                            FinishVisitorService(slot.VisitorEntity);

                            // C. 重置槽位
                            slot.IsOccupied = false;
                            slot.VisitorEntity = Entity.Null;
                            slot.Timer = 0f;
                        }

                        // 回写 Buffer 更新时间或状态
                        slots[i] = slot;
                    }
                }
            }
        }

        /// <summary>
        /// 结束单个游客的服务流程
        /// </summary>
        private void FinishVisitorService(Entity visitorEntity)
        {
            if (EntityManager.Exists(visitorEntity))
            {
                // 1. 恢复状态
                if (SystemAPI.HasComponent<VisitorComponent>(visitorEntity))
                {
                    var visitorRef = SystemAPI.GetComponentRW<VisitorComponent>(visitorEntity);
                    visitorRef.ValueRW.CurrentState = VisitorState.Idle;
                    visitorRef.ValueRW.StateTimer = 0.5f;
                }

                // 2. 恢复显隐
                if (EntityManager.HasComponent<ViewInstanceComponent>(visitorEntity))
                {
                    var view = EntityManager.GetComponentData<ViewInstanceComponent>(visitorEntity);
                    if (view.GameObject != null)
                    {
                        view.GameObject.SetActive(true);
                    }
                }
            }
        }
    }
}