using GameFramework.ECS.Components;
using GameFramework.Core;
using Unity.Entities;
using Unity.Burst;
using UnityEngine;
using Unity.Collections;
using Game.HotUpdate; // [新增] 引入命名空间

namespace GameFramework.ECS.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class ServiceProcessingSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float dt = SystemAPI.Time.DeltaTime;
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (srvRef, entity) in SystemAPI.Query<RefRW<ServiceComponent>>().WithEntityAccess())
            {
                ref var srv = ref srvRef.ValueRW;
                if (!srv.IsActive) continue;

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
                            // [修复] 使用 GlobalInventoryManager
                            GlobalInventoryManager.Instance.AddItem(srv.OutputItemId, srv.OutputItemCount);
                        }

                        // 2. 通知游客服务完成
                        if (!queue.IsEmpty)
                        {
                            Entity visitorEntity = queue[0].VisitorEntity;
                            if (SystemAPI.Exists(visitorEntity))
                            {
                                // ecb.AddComponent<ServiceCompleteTag>(visitorEntity);
                            }
                            queue.RemoveAt(0);
                        }

                        // 4. 重置建筑状态
                        srv.IsServing = false;
                    }
                }
                else
                {
                    // --- 阶段 B: 尝试开始服务 (逻辑保留) ---
                }
            }

            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    }
}