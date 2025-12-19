using GameFramework.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;

namespace Game.HotUpdate
{
    // 【关键修改】在这里添加 ", IGameInventoryService"
    public class GlobalInventoryManager : Singleton<GlobalInventoryManager>, IGameInventoryService
    {
        private readonly Dictionary<int, long> _inventory = new Dictionary<int, long>();
        public event Action<int, long, long> OnItemChanged;

        // 定义资源 ID
        public const int ITEM_ID_WOOD = 101;
        public const int ITEM_ID_STONE = 102;
        public const int ITEM_ID_OIL = 103;
        public const int ITEM_ID_GOLD = 104;

        /// <summary>
        /// 加载库存数据
        /// </summary>
        public void LoadInventory(Dictionary<int, long> savedData)
        {
            _inventory.Clear();

            if (savedData != null && savedData.Count > 0)
            {
                foreach (var kvp in savedData)
                {
                    _inventory[kvp.Key] = kvp.Value;
                }
                Debug.Log("全局物品管理器：已加载存档数据");
            }
            else
            {
                Debug.Log("全局物品管理器：未发现存档，发放【默认模拟数据】...");
                AddItem(ITEM_ID_WOOD, 1000);
                AddItem(ITEM_ID_STONE, 1000);
                AddItem(ITEM_ID_OIL, 1000);
                AddItem(ITEM_ID_GOLD, 5000);
            }
        }

        #region IGameInventoryService 接口实现 & 查询操作

        public cfg.Item_Cfg GetConfig(int itemId)
        {
            // 确保 ConfigManager 已初始化且表已加载
            if (ConfigManager.Instance == null || ConfigManager.Instance.Tables == null)
                return null;
            return ConfigManager.Instance.Tables.ItemCfg.GetOrDefault(itemId);
        }

        public long GetItemCount(int itemId)
        {
            return _inventory.TryGetValue(itemId, out long count) ? count : 0;
        }

        // 接口方法实现
        public bool HasItem(int itemId, long amount) => GetItemCount(itemId) >= amount;

        // 接口方法实现
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

            OnItemChanged?.Invoke(itemId, amount, _inventory[itemId]);
        }

        // 接口方法实现
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