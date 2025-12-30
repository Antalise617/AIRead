using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Entities;
using GameFramework.UI;
using GameFramework.Core;
using GameFramework.Managers;

namespace Game.HotUpdate
{
    public class ProgressBarWidget : UIFollowPanel
    {
        [Header("Controls")]
        [UIBind] public TextMeshProUGUI m_tmp_ProgressBarText; // 显示跟随物体的名字
        [UIBind] public Slider m_slider_CountdownSlider;         // 显示进度
        [UIBind] public TextMeshProUGUI m_tmp_CountdownText;          // 显示剩余时间文本
        [UIBind] public Button m_btn_BuildingRecycleButton;      // 回收按钮

        private CanvasGroup _canvasGroup;

        // 用于内部模拟进度的变量
        private float _totalTime;
        private float _timer;
        private bool _isRunning;

        public override void Initialize()
        {
            base.Initialize();

            // 设置头顶偏移量
            this.offset = new Vector3(0, 3.5f, 0);

            _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup == null) _canvasGroup = gameObject.AddComponent<CanvasGroup>();

            if (m_btn_BuildingRecycleButton != null)
            {
                m_btn_BuildingRecycleButton.onClick.AddListener(OnRecycleClick);
            }

            SetVisible(false);
        }

        /// <summary>
        /// 初始化数据并开始运行
        /// </summary>
        /// <param name="targetEntity">跟随的实体</param>
        /// <param name="buildingName">建筑名称</param>
        /// <param name="duration">总耗时</param>
        public void InitData(Entity targetEntity, string buildingName, float duration)
        {
            // [修复] 直接赋值给基类的 protected 字段，而不是调用不存在的 SetTarget 方法
            _targetEntity = targetEntity;

            if (m_tmp_ProgressBarText != null)
            {
                m_tmp_ProgressBarText.text = buildingName;
            }

            _totalTime = duration;
            _timer = duration;
            _isRunning = true;

            SetVisible(true);
            UpdateUI();
        }

        protected override void LateUpdate()
        {
            // 调用基类处理跟随逻辑 (WorldToScreenPoint)
            base.LateUpdate();

            // 检查实体是否存在
            if (_targetEntity == Entity.Null || !_entityManager.Exists(_targetEntity))
            {
                CloseSelf();
                return;
            }

            if (_isRunning)
            {
                _timer -= Time.deltaTime;
                if (_timer <= 0)
                {
                    _timer = 0;
                    _isRunning = false;
                    OnFinished();
                }
                UpdateUI();
            }
        }

        private void UpdateUI()
        {
            // 1. 更新进度条 (0 -> 1)
            if (m_slider_CountdownSlider != null)
            {
                // 倒计时逻辑：剩余时间/总时间
                float ratio = _totalTime > 0 ? (_totalTime - _timer) / _totalTime : 1f;
                m_slider_CountdownSlider.value = Mathf.Clamp01(ratio);
            }

            // 2. 更新剩余时间文本
            if (m_tmp_CountdownText != null)
            {
                m_tmp_CountdownText.text = $"{_timer:F1}s";
            }
        }

        private void OnRecycleClick()
        {
            Debug.Log("[ProgressBarWidget] 点击了回收按钮");
            // 在此添加具体回收逻辑
        }

        private void OnFinished()
        {
            Debug.Log("[ProgressBarWidget] 进度完成");
            // 完成后的逻辑，例如隐藏或关闭
            // SetVisible(false);
            // CloseSelf();
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
            if (UIManager.Instance != null)
            {
                UIManager.Instance.HidePanel("ProgressBarWidget");
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}