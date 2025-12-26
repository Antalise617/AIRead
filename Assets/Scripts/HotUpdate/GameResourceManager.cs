using UnityEngine;
using System.Collections.Generic;
using GameFramework.Events; // 引用事件系统
using System;

namespace GameFramework.Managers
{
    public class GameResourceManager : Singleton<GameResourceManager>
    {
        // 缓存资源数据
        private Dictionary<ResourceType, int> _resourceCache = new Dictionary<ResourceType, int>();

        private const string PREFS_PREFIX = "GameRes_";

        protected override void Awake()
        {
            base.Awake();
            LoadAllResources();
        }

        /// <summary>
        /// 加载所有资源
        /// </summary>
        private void LoadAllResources()
        {
            foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
            {
                string key = GetSaveKey(type);
                int value = PlayerPrefs.GetInt(key, 1000);
                _resourceCache[type] = value;

                // 新增：打印加载后的资源数量
                Debug.Log($"[GameResourceManager] Resource Loaded: {type} = {value}");
            }
            Debug.Log("[GameResourceManager] All Resources Loaded.");
        }

        public int GetResource(ResourceType type)
        {
            return _resourceCache.GetValueOrDefault(type, 0);
        }

        /// <summary>
        /// 修改资源数量
        /// </summary>
        public bool ChangeResource(ResourceType type, int amount)
        {
            if (!_resourceCache.ContainsKey(type)) _resourceCache[type] = 0;

            int current = _resourceCache[type];

            // 检查消耗是否足够
            if (amount < 0 && current + amount < 0)
            {
                Debug.LogWarning($"[Resource] 不足: {type}, 需要 {-amount}, 当前 {current}");
                return false;
            }

            int newValue = current + amount;
            _resourceCache[type] = newValue;

            // 1. 保存
            SaveResource(type, newValue);

            // 2. ★ 发布事件 (使用你的 EventManager.Publish)
            var evt = new ResourceChangedEvent(type, newValue, amount);
            EventManager.Instance.Publish(evt);

            Debug.Log($"[Resource] {type} changed: {amount}. Now: {newValue}");
            return true;
        }

        private void SaveResource(ResourceType type, int value)
        {
            PlayerPrefs.SetInt(GetSaveKey(type), value);
        }

        private string GetSaveKey(ResourceType type)
        {
            return $"{PREFS_PREFIX}{type}";
        }

        // 调试用：重置
        public void ResetAllData()
        {
            PlayerPrefs.DeleteAll();
            LoadAllResources();

            // 通知 UI 清零
            foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
            {
                EventManager.Instance.Publish(new ResourceChangedEvent(type, 0, 0));
            }
        }
    }
    // 资源类型枚举保持不变
    public enum ResourceType
    {
        Wood = 0,   // 木头
        Stone = 1,  // 石头
        Gold = 2,   // 金币
        Food = 3    // 食物
    }

    // ★ 关键修改：实现 IGameEvent 接口
    public struct ResourceChangedEvent : IGameEvent
    {
        public ResourceType Type;
        public int NewValue;
        public int Delta; // 变化量

        public ResourceChangedEvent(ResourceType type, int newValue, int delta)
        {
            Type = type;
            NewValue = newValue;
            Delta = delta;
        }
    }
}