using GameFramework.ECS.Components;
using GameFramework.Core;
using Unity.Entities;
using Unity.Burst;
using UnityEngine;
using Unity.Collections;

namespace GameFramework.ECS.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class ServiceProcessingSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float dt = SystemAPI.Time.DeltaTime;
            var ecb = new EntityCommandBuffer(Allocator.Temp); // 使用 ECB 来操作组件

            foreach (var (srvRef, entity) in SystemAPI.Query<RefRW<ServiceComponent>>().WithEntityAccess())
            {
                ref var srv = ref srvRef.ValueRW;
                if (!srv.IsActive) continue;

                // ... (电力检查、库存获取等代码保持不变) ...

                var queue = SystemAPI.GetBuffer<ServiceQueueElement>(entity);

                if (srv.IsServing)
                {
                    // --- 阶段 A: 服务进行中 ---
                    srv.ServiceTimer -= dt;

                    if (srv.ServiceTimer <= 0)
                    {
                        // 1. 服务完成，发钱
                        if (srv.OutputItemId > 0 && srv.OutputItemCount > 0)
                        {
                            GameInventoryBridge.AddItem(srv.OutputItemId, srv.OutputItemCount);
                        }

                        // 2. 【核心修改】通知游客服务完成
                        if (!queue.IsEmpty)
                        {
                            Entity visitorEntity = queue[0].VisitorEntity;

                            // 检查游客实体是否还存在 (防止游客中途被销毁导致报错)
                            if (SystemAPI.Exists(visitorEntity))
                            {
                                // 给游客挂一个 Tag，告诉 VisitorBehaviorSystem "你可以走了"
                                //ecb.AddComponent<ServiceCompleteTag>(visitorEntity);
                            }

                            // 3. 从建筑队列中移除
                            queue.RemoveAt(0);
                        }

                        // 4. 重置建筑状态
                        srv.IsServing = false;
                    }
                }
                else
                {
                    // --- 阶段 B: 尝试开始服务 (保持不变) ---
                    // ... (查找库存、扣库存、设置 IsServing = true) ...
                }
            }

            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    }
}