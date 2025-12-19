using GameFramework.ECS.Components;
using GameFramework.Core;
using Unity.Entities;
using Unity.Burst;
using UnityEngine;

namespace GameFramework.ECS.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class ProductionSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float dt = SystemAPI.Time.DeltaTime;
            var em = EntityManager;

            // 遍历所有带生产组件的实体
            // 注意：这里需要 RefRW (读写权限)
            foreach (var (prodRef, entity) in SystemAPI.Query<RefRW<ProductionComponent>>().WithEntityAccess())
            {
                ref var prod = ref prodRef.ValueRW;

                // 1. 检查开关
                if (!prod.IsActive) continue;

                // 2. 检查储量是否已满
                // 如果当前储量 + 产出量 > 上限，则停止生产
                if (prod.CurrentReserves + prod.OutputCount > prod.MaxReserves)
                {
                    // 储量已满，停止计时 (可选：在这里播放满仓特效)
                    continue;
                }

                // 3. 检查原料是否充足 (如果有输入要求)
                // 原料通常存在建筑自身的背包里 (InventoryItemElement)
                if (prod.InputItemId > 0 && prod.InputCount > 0)
                {
                    // TODO:补充逻辑
                }

                // 4. 推进进度
                prod.Timer += dt;

                // 5. 完成生产
                if (prod.Timer >= prod.ProductionInterval)
                {
                    prod.Timer -= prod.ProductionInterval;

                    // A. 扣除原料
                    if (prod.InputItemId > 0 && prod.InputCount > 0)
                    {
                        // TODO:补充逻辑
                    }

                    // B. 增加储量 (关键变化：不再直接进背包，而是进 CurrentReserves)
                    prod.CurrentReserves += prod.OutputCount;

                    // Debug.Log($"[Production] 建筑 {entity.Index} 产出增加，当前储量: {prod.CurrentReserves}/{prod.MaxReserves}");
                }
            }
        }

        // ========================================================================
        // [新增] 静态 API：收取产物
        // 供 InteractionSystem (点击事件) 或 UI 调用
        // ========================================================================

        /// <summary>
        /// 尝试收取建筑内的产物到目标背包 (通常是玩家背包)
        /// </summary>
        /// <returns>收取的数量</returns>
        public static int CollectProduction(EntityManager em, Entity buildingEntity, Entity targetInventoryEntity)
        {
            if (!em.HasComponent<ProductionComponent>(buildingEntity)) return 0;

            var prod = em.GetComponentData<ProductionComponent>(buildingEntity);

            if (prod.CurrentReserves <= 0) return 0;

            int amountToCollect = prod.CurrentReserves;
            int itemId = prod.OutputItemId;

            // 收集产物
            // TODO:补充逻辑

            // 2. 清空建筑储量并回写组件
            prod.CurrentReserves = 0;
            // 收集后通常重置计时器，或者保持原样，视策划需求而定。这里保持原样，允许连续生产。
            em.SetComponentData(buildingEntity, prod);

            Debug.Log($"[Production] 收取了 {amountToCollect} 个 {itemId}");
            return amountToCollect;
        }
    }
}