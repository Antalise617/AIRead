using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;
using GameFramework.Core;
using GameFramework.ECS.Components;
using GameFramework.Managers;
using GameFramework.ECS.Systems;
using GameFramework.UI;

namespace Game.HotUpdate
{
    public class LittleProgressBarWidget : UIFollowPanel
    {
        [UIBind] public Slider m_slider_LittleProgressSlider;
        [UIBind] public Button m_btn_ReceiveButton;

        private CanvasGroup _canvasGroup;

        public override void Initialize()
        {
            base.Initialize();

            this.offset = new Vector3(0, 3.5f, 0);

            _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup == null) _canvasGroup = gameObject.AddComponent<CanvasGroup>();

            if (m_btn_ReceiveButton != null)
            {
                m_btn_ReceiveButton.onClick.AddListener(OnReceiveClick);
            }

            SetVisible(false);
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();

            if (_targetEntity == Entity.Null || !_entityManager.Exists(_targetEntity))
            {
                CloseSelf();
                return;
            }

            UpdateProgressData();
        }

        private void UpdateProgressData()
        {
            float progress = 0f;
            bool shouldShow = false;
            // bool canReceive = false; // [修改] 不再需要此变量控制显隐

            // --- 情况 A: 生产型建筑 ---
            if (_entityManager.HasComponent<ProductionComponent>(_targetEntity))
            {
                var prod = _entityManager.GetComponentData<ProductionComponent>(_targetEntity);

                if (_entityManager.HasBuffer<ProductionOutputElement>(_targetEntity))
                {
                    var outputs = _entityManager.GetBuffer<ProductionOutputElement>(_targetEntity);

                    int currentTotalStorage = 0;
                    foreach (var item in outputs)
                    {
                        currentTotalStorage += item.CurrentStorage;
                    }

                    if (prod.MaxReserves > 0)
                    {
                        progress = (float)currentTotalStorage / prod.MaxReserves;
                    }

                    if (prod.IsActive)
                    {
                        shouldShow = true;
                    }
                }
            }
            // --- 情况 B: 服务型建筑 ---
            else if (_entityManager.HasComponent<ServiceComponent>(_targetEntity))
            {
                var serv = _entityManager.GetComponentData<ServiceComponent>(_targetEntity);

                if (serv.IsActive && serv.IsServing && serv.ServiceTimer > 0 && serv.ServiceTime > 0)
                {
                    progress = 1.0f - (serv.ServiceTimer / serv.ServiceTime);
                    shouldShow = true;
                }
            }

            // --- 更新 UI ---
            if (shouldShow)
            {
                SetVisible(true);
                if (m_slider_LittleProgressSlider != null)
                {
                    m_slider_LittleProgressSlider.value = Mathf.Clamp01(progress);
                }

                // [核心修改] 强制显示收取按钮，或者保持其默认状态
                if (m_btn_ReceiveButton != null)
                {
                    m_btn_ReceiveButton.gameObject.SetActive(true);

                    // 可选优化：如果没有东西可收，可以将按钮设为不可交互(变灰)，给玩家反馈
                    // 既然您要求“保持显示”，这里只确保它不被隐藏
                    // m_btn_ReceiveButton.interactable = (progress > 0); 
                }
            }
            else
            {
                SetVisible(false);
            }
        }

        private void OnReceiveClick()
        {
            if (_targetEntity == Entity.Null || !_entityManager.Exists(_targetEntity)) return;

            // 如果当前其实没有东西可收，CollectProduction 会返回 0，不会有副作用
            int collectedCount = ProductionSystem.CollectProduction(_entityManager, _targetEntity);

            if (collectedCount > 0)
            {
                Debug.Log($"[LittleProgressBarWidget] 收取了 {collectedCount} 个物品");
                UpdateProgressData(); // 刷新进度条
            }
            else
            {
                // 可选：如果点击了但没收到东西，可以弹一个提示 "当前没有产出"
                // Debug.Log("仓库是空的！");
            }
        }

        public void SetProgressManual(float ratio)
        {
            SetVisible(true);
            if (m_slider_LittleProgressSlider != null)
            {
                m_slider_LittleProgressSlider.value = ratio;
            }
        }

        private void SetVisible(bool visible)
        {
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = visible ? 1 : 0;
                _canvasGroup.interactable = visible;
                _canvasGroup.blocksRaycasts = visible;
            }
            else
            {
                gameObject.SetActive(visible);
            }
        }

        private void CloseSelf()
        {
            UIManager.Instance.HidePanel("LittleProgressBarWidget");
        }
    }
}