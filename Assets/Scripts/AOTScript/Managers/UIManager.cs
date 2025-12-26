using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System;
using GameFramework.Core;

namespace GameFramework.Managers
{
    public enum UILayer
    {
        Background = 0,
        Normal = 1,
        Popup = 2,
        System = 3
    }

    public class UIManager : Singleton<UIManager>
    {
        private Canvas _uiCanvas;
        private Dictionary<UILayer, Transform> _layerParents = new();
        private Dictionary<string, UIPanel> _panels = new();

        // [新增] Widget 对象池字典: Key = 资源路径
        private Dictionary<string, Stack<UIFollowPanel>> _widgetPools = new();

        // [新增] 活跃 Widget 列表 (可选，用于调试或统一隐藏)
        private HashSet<UIFollowPanel> _activeWidgets = new();

        // [新增 1] 初始化完成标志
        public bool IsInitialized { get; private set; } = false;

        public async UniTask InitializeAsync()
        {
            var canvasPrefab = await ResourceManager.Instance.LoadAssetAsync<GameObject>("UICanvas");
            var canvasGO = Instantiate(canvasPrefab);
            canvasGO.name = "UI_Root";
            _uiCanvas = canvasGO.GetComponent<Canvas>();
            DontDestroyOnLoad(canvasGO);

            CreateLayers(canvasGO.transform);

            // [新增 2] 标记初始化完成
            IsInitialized = true;
            Debug.Log("UI系统初始化完成");
        }

        private void CreateLayers(Transform root)
        {
            foreach (UILayer layer in Enum.GetValues(typeof(UILayer)))
            {
                GameObject layerGO = new GameObject(layer.ToString());
                var rect = layerGO.AddComponent<RectTransform>();
                layerGO.transform.SetParent(root, false);

                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;

                _layerParents[layer] = layerGO.transform;
            }
        }

        public async UniTask<T> ShowPanelAsync<T>(string panelKey, UILayer layer = UILayer.Normal) where T : UIPanel
        {
            // [可选安全检查] 防止未初始化直接调用
            if (!IsInitialized)
            {
                Debug.LogWarning($"[UIManager] 系统未初始化，正在等待... (Panel: {panelKey})");
                await UniTask.WaitUntil(() => IsInitialized);
            }

            T panel;
            if (_panels.TryGetValue(panelKey, out var existingPanel))
            {
                panel = existingPanel as T;
                panel.transform.SetParent(_layerParents[layer], false);
            }
            else
            {
                var panelGO = await ResourceManager.Instance.InstantiateAsync(panelKey, _layerParents[layer]);
                panel = panelGO.GetComponent<T>();
                panel.Initialize();
                _panels[panelKey] = panel;
            }

            panel.transform.SetAsLastSibling();
            panel.Show();
            return panel;
        }

        public void HidePanel(string panelKey)
        {
            if (_panels.TryGetValue(panelKey, out var panel))
            {
                panel.Hide();
            }
        }

        /// <summary>
        /// 获取并显示一个跟随 Widget
        /// </summary>
        public async UniTask<T> ShowWidgetAsync<T>(string assetPath, Unity.Entities.Entity target, UILayer layer = UILayer.Normal) where T : UIFollowPanel
        {
            if (!IsInitialized) await UniTask.WaitUntil(() => IsInitialized);

            T widget = null;

            // 1. 尝试从池中取
            if (_widgetPools.TryGetValue(assetPath, out var pool) && pool.Count > 0)
            {
                // 校验池中对象是否为空 (防止 GameObject 被意外销毁)
                while (pool.Count > 0)
                {
                    var item = pool.Pop();
                    if (item != null)
                    {
                        widget = item as T;
                        break;
                    }
                }
            }

            // 2. 如果池里没有，实例化新的
            if (widget == null)
            {
                // 注意：Widget 建议默认放在 Normal 层或者新建一个 "World" 层
                var widgetGO = await ResourceManager.Instance.InstantiateAsync(assetPath, _layerParents[layer]);

                widget = widgetGO.GetComponent<T>();
                if (widget == null)
                {
                    Debug.LogError($"[UIManager] 资源 {assetPath} 上找不到组件 {typeof(T)}");
                    Destroy(widgetGO);
                    return null;
                }

                // 初始化 (UIBind 等)
                widget.Initialize();
                widget.AssetPath = assetPath; // [关键] 记录身份证
            }

            // 3. 重置状态并显示
            widget.transform.SetParent(_layerParents[layer], false); // 确保父节点正确
            widget.transform.SetAsLastSibling(); // 渲染在最上层

            _activeWidgets.Add(widget);

            // 4. 绑定实体 (这将触发 Show)
            widget.Bind(target);

            return widget;
        }

        /// <summary>
        /// 回收 Widget (由 UIFollowPanel 内部调用)
        /// </summary>
        public void RecycleWidget(UIFollowPanel widget)
        {
            if (widget == null) return;

            string path = widget.AssetPath;
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogWarning($"[UIManager] 试图回收一个没有 AssetPath 的 Widget: {widget.name}，直接销毁。");
                Destroy(widget.gameObject);
                return;
            }

            // 1. 隐藏
            widget.Hide();
            _activeWidgets.Remove(widget);

            // 2. 为了保持层级整洁，可以挂到一个隐藏的节点下 (可选)
            // widget.transform.SetParent(PoolRoot); 

            // 3. 入池
            if (!_widgetPools.TryGetValue(path, out var pool))
            {
                pool = new Stack<UIFollowPanel>();
                _widgetPools[path] = pool;
            }
            pool.Push(widget);
        }

        // 扩展：清空所有 Widget (例如切换场景时)
        public void ClearAllWidgets()
        {
            foreach (var widget in _activeWidgets)
            {
                if (widget != null) Destroy(widget.gameObject);
            }
            _activeWidgets.Clear();
            _widgetPools.Clear();
        }
    }
}