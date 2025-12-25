using GameFramework.ECS.Components;
using GameFramework.Core;
using GameFramework.Managers; // 引用 UI 管理器
using Unity.Entities;
using Unity.Burst;
using UnityEngine;
using Cysharp.Threading.Tasks; // 引用 UniTask
using Unity.Collections;       // 引用 NativeList

namespace GameFramework.ECS.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class ProductionSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float dt = SystemAPI.Time.DeltaTime;

            // 缓存需要创建 UI 的实体，避免在 foreach 中进行结构性修改 (Adding Components)
            NativeList<Entity> entitiesNeedingUI = new NativeList<Entity>(Allocator.Temp);

            // 遍历所有拥有生产组件的实体
            foreach (var (prodRef, entity) in SystemAPI.Query<RefRW<ProductionComponent>>().WithEntityAccess())
            {
                ref var prod = ref prodRef.ValueRW;

                // 1. 检查开关
                if (!prod.IsActive) continue;

                // 2. 检查电力
                if (SystemAPI.HasComponent<ElectricityComponent>(entity))
                {
                    if (!SystemAPI.GetComponent<ElectricityComponent>(entity).IsPowered) continue;
                }

                // 获取 Buffer 数据
                DynamicBuffer<ProductionOutputElement> outputs = default;
                DynamicBuffer<ProductionInputElement> inputs = default;

                if (SystemAPI.HasBuffer<ProductionOutputElement>(entity))
                    outputs = SystemAPI.GetBuffer<ProductionOutputElement>(entity);

                if (SystemAPI.HasBuffer<ProductionInputElement>(entity))
                    inputs = SystemAPI.GetBuffer<ProductionInputElement>(entity);

                // 3. 检查产物库存是否已满
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
                    continue; // 仓库满了
                }

                // 4. 检查原料是否充足
                bool hasIngredients = true;
                if (inputs.IsCreated)
                {
                    foreach (var inItem in inputs)
                    {
                        if (!GameInventoryBridge.HasItem(inItem.ItemId, inItem.Count))
                        {
                            hasIngredients = false;
                            break;
                        }
                    }
                }

                if (!hasIngredients) continue; // 原料不足

                // === 此时，建筑确认正在进行生产 ===

                // 5. 推进生产进度
                prod.Timer += dt;

                // 【核心逻辑】如果正在生产 (Timer > 0) 且还没有进度条 Tag，加入创建列表
                if (prod.Timer > 0 && !SystemAPI.HasComponent<HasProgressBarTag>(entity))
                {
                    entitiesNeedingUI.Add(entity);
                }

                // 6. 生产周期完成
                if (prod.Timer >= prod.ProductionInterval)
                {
                    // A. 实际扣除原料
                    bool consumeSuccess = true;
                    if (inputs.IsCreated)
                    {
                        foreach (var inItem in inputs)
                        {
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
                                outputs[i] = outItem; // 回写 Buffer
                            }
                        }
                    }
                    else
                    {
                        // 扣除失败，回退进度
                        prod.Timer = prod.ProductionInterval;
                    }
                }
            }

            // === 循环结束后，批量处理 UI 创建 ===
            if (entitiesNeedingUI.Length > 0)
            {
                foreach (var entity in entitiesNeedingUI)
                {
                    // 二次校验防止实体已销毁
                    if (EntityManager.Exists(entity))
                    {
                        // 1. 添加 Tag
                        EntityManager.AddComponent<HasProgressBarTag>(entity);

                        // 2. 触发 UI 创建
                        CreateProgressBarAsync(entity).Forget();
                    }
                }
            }
            entitiesNeedingUI.Dispose();
        }

        // ========================================================================
        // 异步创建 UI 方法 (通过 UIManager)
        // ========================================================================
        private async UniTaskVoid CreateProgressBarAsync(Entity buildingEntity)
        {
            string widgetKey = "LittleProgressBarWidget"; // 必须与 Addressables Key 一致

            // 使用 UIManager 显示面板 (Normal 层)
            // 注意：LittleProgressBarWidget 必须继承自 UIFollowPanel
            var panel = await UIManager.Instance.ShowPanelAsync<UIFollowPanel>(widgetKey, UILayer.Normal);

            if (panel != null)
            {
                panel.Bind(buildingEntity);
            }
        }

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
                    GameInventoryBridge.AddItem(item.ItemId, item.CurrentStorage);
                    totalCollected += item.CurrentStorage;
                    Debug.Log($"[Production] 收取: ID {item.ItemId} x{item.CurrentStorage}");
                    item.CurrentStorage = 0;
                    outputs[i] = item;
                }
            }

            return totalCollected;
        }
    }
}