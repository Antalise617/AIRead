using GameFramework.Core;
using System.Collections.Generic;
using UnityEngine;
using cfg; // 引用配置命名空间
using GameFramework;
using System;

namespace Game.HotUpdate
{
    public class GlobalInventoryManager : Singleton<GlobalInventoryManager>, IGameInventoryService
    {
        private readonly Dictionary<int, long> _inventory = new Dictionary<int, long>();
        public event Action<int, long, long> OnItemChanged;

        // ... (常量定义可保留也可移除，现在逻辑主要依赖配表) ...

        protected override void Awake()
        {
            base.Awake();
            GameInventoryBridge.Service = this;
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
            // 2. 没有存档时，读取【GameConfig.InitialResources】进行初始化
            else
            {
                Debug.Log("[GlobalInventoryManager] 未发现存档，读取配置表初始化资源...");

                var tables = ConfigManager.Instance.Tables;
                if (tables != null && tables.TbGameConfig.DataList.Count > 0)
                {
                    // 【修复】直接获取 DataList 的第一个元素，忽略 Key 是多少
                    var gameCfg = tables.TbGameConfig.DataList[0];

                    if (gameCfg != null && gameCfg.InitialResources != null)
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

        // ... (IGameInventoryService 接口实现保持不变) ...

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

            // 校验物品ID是否有效
            if (GetConfig(itemId) == null)
            {
                Debug.LogError($"[GlobalInventoryManager] 无法添加物品，ID {itemId} 在道具表中不存在！");
                return;
            }

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