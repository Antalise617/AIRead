using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Entities;
using GameFramework.UI;
using GameFramework.Managers;
using GameFramework.Core;

namespace Game.HotUpdate
{
    public class ProgressBarWidget : UIFollowPanel
    {
        [Header("Controls")]
        [UIBind] public TextMeshProUGUI m_tmp_ProgressBarText;
        [UIBind] public Slider m_slider_CountdownSlider;
        [UIBind] public TextMeshProUGUI m_tmp_CountdownText;
        [UIBind] public Button m_btn_BuildingRecycleButton;

        private CanvasGroup _canvasGroup;
        private long _startTime;
        private long _endTime;
        private int _currentState;
        private bool _isRunning;

        public override void Initialize()
        {
            base.Initialize();
            this.offset = new Vector3(0, 5.0f, 0);

            _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup == null) _canvasGroup = gameObject.AddComponent<CanvasGroup>();

            if (m_btn_BuildingRecycleButton != null)
                m_btn_BuildingRecycleButton.onClick.AddListener(OnRecycleClick);
        }

        public void RefreshData(Entity targetEntity, int state, long startTime, long endTime)
        {
            // 绑定新的目标实体（UIFollowPanel 的基础功能）
            Bind(targetEntity);

            _currentState = state;
            _startTime = startTime;
            _endTime = endTime;
            _isRunning = true;

            if (m_tmp_ProgressBarText != null)
            {
                m_tmp_ProgressBarText.text = state == 3 ? "销毁中..." : "建造中...";
            }

            if (m_btn_BuildingRecycleButton != null)
            {
                m_btn_BuildingRecycleButton.gameObject.SetActive(true);
            }

            SetVisible(true);
            UpdateUI();
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();

            // 如果实体不存在，自我回收
            if (_targetEntity == Entity.Null || !_entityManager.Exists(_targetEntity))
            {
                CloseSelf();
                return;
            }

            if (_isRunning)
            {
                UpdateUI();
            }
        }

        private void UpdateUI()
        {
            long currentServerTime = TimeManager.Instance.ServerTime; // 使用 TimeManager
            long totalDuration = _endTime - _startTime;
            long remaining = _endTime - currentServerTime;

            if (remaining <= 0)
            {
                remaining = 0;
                _isRunning = false;
                OnFinished();
            }

            if (m_slider_CountdownSlider != null)
            {
                if (totalDuration > 0)
                {
                    float progress = 1f - ((float)remaining / totalDuration);
                    m_slider_CountdownSlider.value = Mathf.Clamp01(progress);
                }
                else
                {
                    m_slider_CountdownSlider.value = 1f;
                }
            }

            if (m_tmp_CountdownText != null)
            {
                System.TimeSpan ts = System.TimeSpan.FromSeconds(remaining);
                m_tmp_CountdownText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", ts.Hours, ts.Minutes, ts.Seconds);
            }
        }

        private void OnRecycleClick()
        {
            Debug.Log($"点击了操作按钮, 当前状态: {_currentState}");
        }

        private void OnFinished()
        {
            Debug.Log("倒计时结束");
        }

        private void SetVisible(bool visible)
        {
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = visible ? 1 : 0;
                _canvasGroup.blocksRaycasts = visible;
            }
            else
            {
                gameObject.SetActive(visible);
            }
        }

        // [修复核心] 改为调用 UIManager 的回收方法，而不是 HidePanel
        public void CloseSelf()
        {
            _isRunning = false;
            UIManager.Instance.RecycleWidget(this);
        }
    }
}