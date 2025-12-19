using GameFramework.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;

namespace Game.HotUpdate
{
    public class GlobalInventoryManager : Singleton<GlobalInventoryManager>
    {
        private readonly Dictionary<int, long> _inventory = new Dictionary<int, long>();
        public event Action<int, long, long> OnItemChanged;

        // 定义资源 ID (请确保这些 ID 在你的 Item.xlsx 表中存在)
        public const int ITEM_ID_WOOD = 101;
        public const int ITEM_ID_STONE = 102;
        public const int ITEM_ID_OIL = 103;
        public const int ITEM_ID_GOLD = 104;

        /// <summary>
        /// 加载库存数据（传入 null 则初始化默认资源）
        /// </summary>
        public void LoadInventory(Dictionary<int, long> savedData)
        {
            _inventory.Clear();

            if (savedData != null && savedData.Count > 0)
            {
                // 如果有存档数据，就加载存档
                foreach (var kvp in savedData)
                {
                    _inventory[kvp.Key] = kvp.Value;
                }
                Debug.Log("全局物品管理器：已加载存档数据");
            }
            else
            {
                // -------------------------------------------------------------
                // 【核心修改】如果没有数据，直接发放默认测试资源
                // -------------------------------------------------------------
                Debug.Log("全局物品管理器：未发现存档，发放【默认模拟数据】...");

                // 模拟发放资源：木头、石头、石油各 1000，金币 5000
                // AddItem 内部会自动校验 ID 是否有效，并触发 UI 刷新事件
                AddItem(ITEM_ID_WOOD, 1000);
                AddItem(ITEM_ID_STONE, 1000);
                AddItem(ITEM_ID_OIL, 1000);
                AddItem(ITEM_ID_GOLD, 5000);
            }
        }

        #region 查询与操作 (保持不变)

        public cfg.Item_Cfg GetConfig(int itemId)
        {
            return ConfigManager.Instance.Tables.ItemCfg.GetOrDefault(itemId);
        }

        public long GetItemCount(int itemId)
        {
            return _inventory.TryGetValue(itemId, out long count) ? count : 0;
        }

        public bool HasItem(int itemId, long amount) => GetItemCount(itemId) >= amount;

        public void AddItem(int itemId, long amount)
        {
            if (amount <= 0) return;

            // 简单校验配置
            if (GetConfig(itemId) == null)
            {
                Debug.LogError($"[GlobalInventoryManager] 无法添加物品，ID {itemId} 在配置表中不存在！");
                return;
            }

            if (!_inventory.ContainsKey(itemId)) _inventory[itemId] = 0;
            _inventory[itemId] += amount;

            // 触发事件通知 UI 更新
            OnItemChanged?.Invoke(itemId, amount, _inventory[itemId]);
        }

        public bool TryConsumeItem(int itemId, long amount)
        {
            if (amount <= 0) return true;
            if (GetItemCount(itemId) < amount) return false;

            _inventory[itemId] -= amount;
            OnItemChanged?.Invoke(itemId, -amount, _inventory[itemId]);
            return true;
        }

        #endregion
    }

}