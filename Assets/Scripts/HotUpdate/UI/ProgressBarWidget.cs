using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Entities;
using GameFramework.UI;
using GameFramework.Managers; // 假设 TimeManager 在此命名空间
using GameFramework.Core;

namespace Game.HotUpdate
{
    public class ProgressBarWidget : UIFollowPanel
    {
        [Header("Controls")]
        [UIBind] public TextMeshProUGUI m_tmp_ProgressBarText; // 标题文本
        [UIBind] public Slider m_slider_CountdownSlider;         // 进度条
        [UIBind] public TextMeshProUGUI m_tmp_CountdownText;     // 剩余时间
        [UIBind] public Button m_btn_BuildingRecycleButton;      // 回收/取消按钮

        private CanvasGroup _canvasGroup;

        // 时间相关
        private long _startTime; // 服务器开始时间戳（秒）
        private long _endTime;   // 服务器结束时间戳（秒）
        private int _currentState; // 0:建造中, 3:销毁中

        private bool _isRunning;

        public override void Initialize()
        {
            base.Initialize();
            this.offset = new Vector3(0, 5.0f, 0); // 调整高度偏移

            _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup == null) _canvasGroup = gameObject.AddComponent<CanvasGroup>();

            if (m_btn_BuildingRecycleButton != null)
                m_btn_BuildingRecycleButton.onClick.AddListener(OnRecycleClick);
        }

        /// <summary>
        /// 初始化或更新进度条数据
        /// </summary>
        public void RefreshData(Entity targetEntity, int state, long startTime, long endTime)
        {
            _targetEntity = targetEntity;
            _currentState = state;
            _startTime = startTime;
            _endTime = endTime;
            _isRunning = true;

            // 根据状态设置标题
            if (m_tmp_ProgressBarText != null)
            {
                m_tmp_ProgressBarText.text = state == 3 ? "销毁中..." : "建造中...";
            }

            // 如果处于建造状态，通常不显示回收按钮，或者回收按钮用于“加速”
            if (m_btn_BuildingRecycleButton != null)
            {
                // 示例：仅在销毁时显示回收/取消，或者根据需求定义
                m_btn_BuildingRecycleButton.gameObject.SetActive(true);
            }

            SetVisible(true);
            UpdateUI(); // 立即刷新一次
        }

        protected override void LateUpdate()
        {
            base.LateUpdate(); // 处理跟随

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
            // 获取当前服务器时间（假设 TimeManager 提供此功能，如果没有则需自行实现）
            // 如果 TimeManager.Instance.ServerTime 返回的是毫秒，请注意单位转换
            long currentServerTime = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            long totalDuration = _endTime - _startTime;
            long remaining = _endTime - currentServerTime;

            if (remaining <= 0)
            {
                remaining = 0;
                _isRunning = false;
                OnFinished();
            }

            // 1. 更新进度条 (0 -> 1)
            if (m_slider_CountdownSlider != null)
            {
                if (totalDuration > 0)
                {
                    // 建造：从 0 到 1
                    float progress = 1f - ((float)remaining / totalDuration);
                    m_slider_CountdownSlider.value = Mathf.Clamp01(progress);
                }
                else
                {
                    m_slider_CountdownSlider.value = 1f;
                }
            }

            // 2. 更新文字
            if (m_tmp_CountdownText != null)
            {
                // 格式化时间 HH:mm:ss
                System.TimeSpan ts = System.TimeSpan.FromSeconds(remaining);
                m_tmp_CountdownText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", ts.Hours, ts.Minutes, ts.Seconds);
            }
        }

        private void OnRecycleClick()
        {
            // 这里可以处理 加速 或 取消 的逻辑
            Debug.Log($"点击了操作按钮, 当前状态: {_currentState}");
            // 例如：调用加速接口
            // NetworkManager.Instance.Send_TileUpLevelSpeed(...);
        }

        private void OnFinished()
        {
            // 倒计时结束，前端可以先隐藏，等待服务器状态同步（State变为1）后再彻底移除
            // 或者在这里主动请求 /tile/upLevelSpeedEnd 接口通知服务器
            Debug.Log("倒计时结束，等待状态同步...");
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

        public void CloseSelf()
        {
            UIManager.Instance.HidePanel(this.gameObject.name.Replace("(Clone)", ""));
        }
    }
}