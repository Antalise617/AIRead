using GameFramework.ECS.Components;
using GameFramework.Core;
using GameFramework.Events; // 引用 EventManager
using Unity.Entities;
using Unity.Burst;
using UnityEngine;

namespace GameFramework.ECS.Systems
{
    // 定义一个收集事件，用于播放特效或飘字
    public struct ProductionCollectedEvent : IGameEvent
    {
        public int ItemId;
        public int Count;
        public Vector3 WorldPos;
    }

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class ProductionSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float dt = SystemAPI.Time.DeltaTime;
            var em = EntityManager;

            // 遍历所有带生产组件的实体
            foreach (var (prodRef, entity) in SystemAPI.Query<RefRW<ProductionComponent>>().WithEntityAccess())
            {
                ref var prod = ref prodRef.ValueRW;

                // 1. 检查开关
                if (!prod.IsActive) continue;

                // 2. 检查储量是否已满
                if (prod.CurrentReserves + prod.OutputCount > prod.MaxReserves)
                {
                    continue; // 储量已满
                }

                // 3. [已完善] 检查原料是否充足 (通过桥接访问全局库存)
                if (prod.InputItemId > 0 && prod.InputCount > 0)
                {
                    // 如果全局库存不足，暂停计时
                    if (!GameInventoryBridge.HasItem(prod.InputItemId, prod.InputCount))
                    {
                        continue;
                    }
                }

                // 4. 推进进度
                prod.Timer += dt;

                // 5. 完成生产
                if (prod.Timer >= prod.ProductionInterval)
                {
                    // A. [已完善] 扣除原料
                    // 再次检查并尝试扣除（防止多线程或极其边缘的情况，虽然这里是主线程）
                    if (prod.InputItemId > 0 && prod.InputCount > 0)
                    {
                        if (!GameInventoryBridge.TryConsumeItem(prod.InputItemId, prod.InputCount))
                        {
                            // 扣除失败（可能瞬间被用掉了），回退进度，这帧不产出
                            prod.Timer = prod.ProductionInterval;
                            continue;
                        }
                    }

                    // B. 增加储量
                    prod.Timer -= prod.ProductionInterval;
                    prod.CurrentReserves += prod.OutputCount;

                    // 防御性逻辑：确保不超过上限
                    if (prod.CurrentReserves > prod.MaxReserves)
                        prod.CurrentReserves = prod.MaxReserves;
                }
            }
        }

        // ========================================================================
        // [已完善] 静态 API：收取产物
        // ========================================================================

        /// <summary>
        /// 尝试收取建筑内的产物到目标背包
        /// </summary>
        public static int CollectProduction(EntityManager em, Entity buildingEntity)
        {
            if (!em.HasComponent<ProductionComponent>(buildingEntity)) return 0;

            // 获取组件副本
            var prod = em.GetComponentData<ProductionComponent>(buildingEntity);

            if (prod.CurrentReserves <= 0) return 0;

            int amountToCollect = prod.CurrentReserves;
            int itemId = prod.OutputItemId;

            // 1. [关键] 将产物添加到全局库存 (通过桥接)
            GameInventoryBridge.AddItem(itemId, amountToCollect);

            // 2. 清空建筑储量并回写组件
            prod.CurrentReserves = 0;
            em.SetComponentData(buildingEntity, prod);

            Debug.Log($"[Production] 成功收取: ID {itemId} x{amountToCollect}");


            return amountToCollect;
        }
    }
}