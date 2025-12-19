using GameFramework.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;

namespace Game.HotUpdate
{
    /// <summary>
    /// 全局物品库存管理器 (单例)
    /// </summary>
    public class GlobalInventoryManager : Singleton<GlobalInventoryManager>
    {
        // 运行时库存缓存：Key = ItemID, Value = 拥有数量
        private readonly Dictionary<int, long> _inventory = new Dictionary<int, long>();

        // 物品变更事件：ItemID, 变更量, 变更后总量
        public event Action<int, long, long> OnItemChanged;

        /// <summary>
        /// 初始化（可选，如果需要预加载存档）
        /// </summary>
        public void LoadInventory(Dictionary<int, long> savedData)
        {
            _inventory.Clear();
            if (savedData != null)
            {
                foreach (var kvp in savedData)
                {
                    _inventory[kvp.Key] = kvp.Value;
                }
            }
            Debug.Log("全局物品管理器加载完成");
        }

        #region 查询方法

        /// <summary>
        /// 获取物品配置信息
        /// </summary>
        public cfg.Item_Cfg GetConfig(int itemId)
        {
            // 通过 ConfigManager 访问 Luban 表
            var itemCfg = ConfigManager.Instance.Tables.ItemCfg;
            return itemCfg.GetOrDefault(itemId);
        }

        /// <summary>
        /// 获取当前拥有的数量
        /// </summary>
        public long GetItemCount(int itemId)
        {
            return _inventory.TryGetValue(itemId, out long count) ? count : 0;
        }

        /// <summary>
        /// 检查物品是否足够
        /// </summary>
        public bool HasItem(int itemId, long amount)
        {
            return GetItemCount(itemId) >= amount;
        }

        #endregion

        #region 操作方法

        /// <summary>
        /// 添加物品
        /// </summary>
        public void AddItem(int itemId, long amount)
        {
            if (amount <= 0) return;

            // 校验配置是否存在，防止添加非法ID
            var cfg = GetConfig(itemId);
            if (cfg == null)
            {
                Debug.LogError($"[Inventory] 尝试添加不存在的物品 ID: {itemId}");
                return;
            }

            if (!_inventory.ContainsKey(itemId))
            {
                _inventory[itemId] = 0;
            }

            _inventory[itemId] += amount;

            Debug.Log($"[Inventory] 获得: {cfg.Name} x{amount} (当前: {_inventory[itemId]})");
            OnItemChanged?.Invoke(itemId, amount, _inventory[itemId]);
        }

        /// <summary>
        /// 消耗物品
        /// </summary>
        /// <returns>如果扣除成功返回 true，不足返回 false</returns>
        public bool TryConsumeItem(int itemId, long amount)
        {
            if (amount <= 0) return true;

            long current = GetItemCount(itemId);
            if (current < amount)
            {
                // 可选：在这里触发“资源不足”的提示事件
                Debug.LogWarning($"[Inventory] 资源不足: ID {itemId}, 需要 {amount}, 拥有 {current}");
                return false;
            }

            _inventory[itemId] -= amount;

            // 如果减到0，是否从字典移除看需求，通常保留Key方便查询
            if (_inventory[itemId] < 0) _inventory[itemId] = 0; // 防御性代码

            var cfg = GetConfig(itemId);
            Debug.Log($"[Inventory] 消耗: {cfg?.Name ?? itemId.ToString()} x{amount} (剩余: {_inventory[itemId]})");

            OnItemChanged?.Invoke(itemId, -amount, _inventory[itemId]);
            return true;
        }

        #endregion
    }
}