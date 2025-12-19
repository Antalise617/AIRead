using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening; // 项目包含DOTween

namespace GameHotfix.Guide
{
    public class GuidePanel : MonoBehaviour
    {
        public GuideMask guideMask;
        public Image maskImage; // 黑色半透明背景
        public TextMeshProUGUI contentText;
        public RectTransform handIcon;
        public Button overlayBtn; // 全屏按钮，用于非强引导时的点击继续

        private System.Action _onStepComplete;

        public void Init()
        {
            // 初始化点击空白处跳过逻辑（如果是弱引导）
            overlayBtn.onClick.AddListener(() => {
                // 处理点击任意处继续的逻辑
            });
        }

        public void Refresh(RectTransform target, string text, bool showHand, Vector2 handOffset)
        {
            // 1. 设置遮罩目标
            guideMask.targetRect = target;

            // 2. 设置文本
            if (contentText != null) contentText.text = text;

            // 3. 设置手势
            if (showHand && target != null)
            {
                handIcon.gameObject.SetActive(true);
                // 将手势放在目标位置 + 偏移
                // 注意：这里涉及坐标系转换，最简单是把handIcon设为非子物体跟随，或者统一转到GuidePanel下
                PositionHand(target, handOffset);
                PlayHandAnimation();
            }
            else
            {
                handIcon.gameObject.SetActive(false);
            }

            // 4. 视觉挖孔 (简单做法：使用由4个Image拼成的遮罩，或者使用带有Stencil Buffer的Shader)
            // 如果项目中没有Shader，建议使用 "InvertedMask" 脚本（Unity社区常用方案）
        }

        private void PositionHand(RectTransform target, Vector2 offset)
        {
            // 简单实现：将目标的世界坐标转为本地坐标
            Vector3 targetWorldPos = target.position;
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
            handIcon.DOKill();
            // 上下浮动效果
            handIcon.DOLocalMoveY(handIcon.localPosition.y + 20, 0.5f).SetLoops(-1, LoopType.Yoyo);
        }
    }
}