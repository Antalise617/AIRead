using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using GameFramework.Managers; // 【新增】引用 UIPanel 所在的命名空间

namespace GameHotfix.Guide
{
    // 【修改】将 MonoBehaviour 改为 UIPanel
    public class GuidePanel : UIPanel
    {
        // 建议：使用 UIPanel 的 AutoBind 特性，或者保持手动赋值均可
        // 如果使用 AutoBind，可以在字段上加 [UIBind("路径")]

        public GuideMask guideMask;
        public Image maskImage;
        public TextMeshProUGUI contentText;
        public RectTransform handIcon;
        public Button overlayBtn;

        // 【修改】UIPanel 有自己的生命周期，建议重写 OnInit 而不是自定义 Init
        protected override void OnInit()
        {
            base.OnInit(); // 保持好习惯

            // 初始化点击空白处跳过逻辑
            if (overlayBtn != null)
            {
                overlayBtn.onClick.AddListener(() => {
                    // 处理点击任意处继续的逻辑
                });
            }
        }

        // 注意：原先的 Init() 方法现在被 OnInit() 替代了，
        // 只要通过 UIManager.ShowPanelAsync 加载，它会自动调用 Initialize() -> OnInit()

        public void Refresh(RectTransform target, string text, bool showHand, Vector2 handOffset)
        {
            // ... (原有逻辑保持不变)

            // 1. 设置遮罩目标
            if (guideMask != null) guideMask.targetRect = target;

            // 2. 设置文本
            if (contentText != null) contentText.text = text;

            // 3. 设置手势
            if (showHand && target != null)
            {
                if (handIcon != null)
                {
                    handIcon.gameObject.SetActive(true);
                    PositionHand(target, handOffset);
                    PlayHandAnimation();
                }
            }
            else
            {
                if (handIcon != null) handIcon.gameObject.SetActive(false);
            }
        }

        private void PositionHand(RectTransform target, Vector2 offset)
        {
            if (target == null) return;

            // 简单实现：将目标的世界坐标转为本地坐标
            Vector3 targetWorldPos = target.position;
            // 注意：这里可能需要指定相机，如果是 Overlay 模式传 null 没问题
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, targetWorldPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                transform as RectTransform,
                screenPos,
                null,
                out Vector2 localPos
            );
            handIcon.anchoredPosition = localPos + offset;
        }

        private void PlayHandAnimation()
        {
            if (handIcon == null) return;
            handIcon.DOKill();
            handIcon.DOLocalMoveY(handIcon.localPosition.y + 20, 0.5f).SetLoops(-1, LoopType.Yoyo);
        }
    }
}