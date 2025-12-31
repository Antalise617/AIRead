using GameFramework.Core;
using System.Collections.Generic;
using UnityEngine;
using cfg;
using GameFramework;
using System;
using GameFramework.Managers; // [新增] 必须引用此命名空间以使用 ItemDTO

namespace Game.HotUpdate
{
    public class GlobalInventoryManager : Singleton<GlobalInventoryManager>
    {
        private readonly Dictionary<int, long> _inventory = new Dictionary<int, long>();

        // 参数: itemId, delta(变化量), total(当前总量)
        public event Action<int, long, long> OnItemChanged;

        protected override void Awake()
        {
            base.Awake();
        }

        public void LoadInventory(Dictionary<int, long> savedData)
        {
            _inventory.Clear();

            // 1. 优先加载存档
            if (savedData != null && savedData.Count > 0)
            {
                foreach (var kvp in savedData)
                {
                    _inventory[kvp.Key] = kvp.Value;
                }
                Debug.Log("[GlobalInventoryManager] 已加载存档数据");
            }
            // 2. 没有存档时，读取配置表
            else
            {
                Debug.Log("[GlobalInventoryManager] 未发现存档，读取配置表初始化资源...");

                var tables = ConfigManager.Instance.Tables;
                // [修改前] if (tables != null && tables.TbGameConfig.DataList.Count > 0)
                // [修改后] 直接判断表是否存在
                if (tables != null && tables.TbGameConfig != null)
                {
                    // [修改前] var gameCfg = tables.TbGameConfig.DataList[0];
                    // [修改后] 单例模式下，TbGameConfig 本身就是数据入口（或者代理了数据）
                    var gameCfg = tables.TbGameConfig;

                    if (gameCfg.InitialResources != null)
                    {
                        foreach (var resInfo in gameCfg.InitialResources)
                        {
                            if (resInfo.Count >= 2)
                            {
                                int itemId = resInfo[0];
                                int count = resInfo[1];
                                AddItem(itemId, count);
                            }
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("[GlobalInventoryManager] GameConfig 表为空！");
                }
            }
        }

        // ========================================================================
        // [新增] 修复 CS1061 错误：处理服务器同步下来的道具列表
        // ========================================================================
        public void UpdateItems(List<ItemDTO> items)
        {
            if (items == null) return;

            foreach (var item in items)
            {
                // ItemDTO 中的 count 通常是当前拥有的绝对数量
                UpdateSingleItemFromNetwork(item.item_id, item.count);
            }

            Debug.Log($"[GlobalInventoryManager] 已同步更新 {items.Count} 个物品数据");
        }

        /// <summary>
        /// 内部辅助：根据最新总量更新本地缓存，并计算差值触发事件
        /// </summary>
        private void UpdateSingleItemFromNetwork(int itemId, long serverCount)
        {
            long localCount = GetItemCount(itemId);

            // 如果数量有变化，才执行更新和回调
            if (localCount != serverCount)
            {
                _inventory[itemId] = serverCount;

                long delta = serverCount - localCount;
                OnItemChanged?.Invoke(itemId, delta, serverCount);

                // Debug.Log($"[Inventory Sync] ID:{itemId} 变化:{delta} 当前:{serverCount}");
            }
        }
        // ========================================================================

        public Item GetConfig(int itemId)
        {
            if (ConfigManager.Instance == null || ConfigManager.Instance.Tables == null) return null;
            return ConfigManager.Instance.Tables.TbItem.GetOrDefault(itemId);
        }

        public long GetItemCount(int itemId) => _inventory.TryGetValue(itemId, out long count) ? count : 0;

        public bool HasItem(int itemId, long amount) => GetItemCount(itemId) >= amount;

        public void AddItem(int itemId, long amount)
        {
            if (amount <= 0) return;
            if (GetConfig(itemId) == null) return;

            if (!_inventory.ContainsKey(itemId)) _inventory[itemId] = 0;
            _inventory[itemId] += amount;

            OnItemChanged?.Invoke(itemId, amount, _inventory[itemId]);
            Debug.Log($"[Inventory] 获得物品: {itemId} x{amount} (当前: {_inventory[itemId]})");
        }

        public bool TryConsumeItem(int itemId, long amount)
        {
            if (amount <= 0) return true;
            if (GetItemCount(itemId) < amount) return false;

            _inventory[itemId] -= amount;
            OnItemChanged?.Invoke(itemId, -amount, _inventory[itemId]);
            return true;
        }
    }
}