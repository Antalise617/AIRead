using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System;

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
    }
}