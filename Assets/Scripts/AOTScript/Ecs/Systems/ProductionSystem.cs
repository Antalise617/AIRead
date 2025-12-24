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

            // 遍历所有拥有生产组件的实体
            foreach (var (prodRef, entity) in SystemAPI.Query<RefRW<ProductionComponent>>().WithEntityAccess())
            {
                ref var prod = ref prodRef.ValueRW;

                // 1. 检查开关
                if (!prod.IsActive) continue;

                // 2. 检查电力 (如果有电力组件)
                // 逻辑：有组件且 IsPowered=false 时才停止，否则视为有电/不需要电
                if (SystemAPI.HasComponent<ElectricityComponent>(entity))
                {
                    if (!SystemAPI.GetComponent<ElectricityComponent>(entity).IsPowered) continue;
                }

                // 获取 Buffer 数据 (使用 DynamicBuffer 处理列表数据)
                DynamicBuffer<ProductionOutputElement> outputs = default;
                DynamicBuffer<ProductionInputElement> inputs = default;

                if (SystemAPI.HasBuffer<ProductionOutputElement>(entity))
                    outputs = SystemAPI.GetBuffer<ProductionOutputElement>(entity);

                if (SystemAPI.HasBuffer<ProductionInputElement>(entity))
                    inputs = SystemAPI.GetBuffer<ProductionInputElement>(entity);

                // 3. 检查产物库存是否已满
                // 逻辑：(当前库存总量 + 本次预产出总量) > 上限 则停止
                int currentStorageTotal = 0;
                int outputPerCycleTotal = 0;

                if (outputs.IsCreated)
                {
                    foreach (var outItem in outputs)
                    {
                        currentStorageTotal += outItem.CurrentStorage;
                        outputPerCycleTotal += outItem.CountPerCycle;
                    }
                }

                if (currentStorageTotal + outputPerCycleTotal > prod.MaxReserves)
                {
                    continue; // 仓库满了，停止生产
                }

                // 4. 【关键】检查原料是否充足 (接入 GlobalInventoryManager)
                bool hasIngredients = true;
                if (inputs.IsCreated)
                {
                    foreach (var inItem in inputs)
                    {
                        // 调用 Bridge 查询全局库存
                        if (!GameInventoryBridge.HasItem(inItem.ItemId, inItem.Count))
                        {
                            hasIngredients = false;
                            break;
                        }
                    }
                }

                if (!hasIngredients) continue; // 原料不足，等待

                // 5. 推进生产进度
                prod.Timer += dt;

                // 6. 生产周期完成
                if (prod.Timer >= prod.ProductionInterval)
                {
                    // A. 【关键】实际扣除原料
                    // 再次检查并扣除（防止极少数情况下的时序问题）
                    bool consumeSuccess = true;
                    if (inputs.IsCreated)
                    {
                        foreach (var inItem in inputs)
                        {
                            // 调用 Bridge 扣除物品
                            if (!GameInventoryBridge.TryConsumeItem(inItem.ItemId, inItem.Count))
                            {
                                consumeSuccess = false;
                                break;
                            }
                        }
                    }

                    if (consumeSuccess)
                    {
                        // B. 扣除成功，重置计时器并增加产出
                        prod.Timer -= prod.ProductionInterval;

                        if (outputs.IsCreated)
                        {
                            for (int i = 0; i < outputs.Length; i++)
                            {
                                var outItem = outputs[i];
                                outItem.CurrentStorage += outItem.CountPerCycle;
                                outputs[i] = outItem; // 回写 Buffer (Struct 值类型需要重新赋值)
                            }
                        }

                        // Debug.Log($"[Production] 生产完成！耗时: {prod.ProductionInterval}s");
                    }
                    else
                    {
                        // 扣除失败（可能刚刚被用了），回退进度，下一帧重试
                        prod.Timer = prod.ProductionInterval;
                    }
                }
            }
        }

        // ========================================================================
        // 静态 API：收取产物 (接入背包)
        // ========================================================================
        public static int CollectProduction(EntityManager em, Entity buildingEntity)
        {
            if (!em.HasComponent<ProductionComponent>(buildingEntity)) return 0;
            if (!em.HasBuffer<ProductionOutputElement>(buildingEntity)) return 0;

            var outputs = em.GetBuffer<ProductionOutputElement>(buildingEntity);
            int totalCollected = 0;

            for (int i = 0; i < outputs.Length; i++)
            {
                var item = outputs[i];
                if (item.CurrentStorage > 0)
                {
                    // 1. 加到全局背包
                    GameInventoryBridge.AddItem(item.ItemId, item.CurrentStorage);

                    totalCollected += item.CurrentStorage;
                    Debug.Log($"[Production] 收取: ID {item.ItemId} x{item.CurrentStorage}");

                    // 2. 清空建筑内库存
                    item.CurrentStorage = 0;
                    outputs[i] = item; // 回写
                }
            }

            return totalCollected;
        }
    }
}