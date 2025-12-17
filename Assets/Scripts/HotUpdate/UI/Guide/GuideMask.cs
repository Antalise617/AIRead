using UnityEngine;
using UnityEngine.UI;

namespace GameHotfix.Guide
{
    /// <summary>
    /// 挂载在黑色半透明遮罩Image上
    /// </summary>
    public class GuideMask : MonoBehaviour, ICanvasRaycastFilter
    {
        [Header("目标镂空区域的RectTransform")]
        public RectTransform targetRect;

        // 遮罩类型（圆形逻辑需额外计算距离，此处演示矩形）
        public bool isCircle = false;

        // 缓存摄像机
        private Camera _uiCamera;
        private RectTransform _maskRect;

        private void Awake()
        {
            _maskRect = GetComponent<RectTransform>();
            // 假设你的UI是Overlay或Camera模式，这里需要根据框架获取
            // 简单起见，如果Canvas是ScreenSpaceOverlay，camera传null
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            {
                _uiCamera = canvas.worldCamera;
            }
        }

        /// <summary>
        /// 核心逻辑：判断点击是否有效
        /// 返回 true 表示拦截点击（即点在遮罩上）
        /// 返回 false 表示穿透（即点在镂空处）
        /// </summary>
        public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            if (targetRect == null || !targetRect.gameObject.activeInHierarchy)
                return true; // 没目标时完全遮挡

            // 将屏幕点击点转换为目标Rect的局部坐标
            bool isInside = RectTransformUtility.ScreenPointToLocalPointInRectangle(
                targetRect,
                sp,
                eventCamera,
                out Vector2 localPoint
            );

            if (isInside)
            {
                // 检查是否在Rect范围内
                if (targetRect.rect.Contains(localPoint))
                {
                    return false; // 在目标区域内，允许穿透
                }
            }

            return true; // 拦截
        }

        // 用于更新视觉遮罩（如果使用Shader挖孔，这里可以传递参数给Material）
        public Vector4 GetTargetBounds()
        {
            if (targetRect == null) return Vector4.zero;

            // 这里通常需要编写逻辑将Target的世界坐标转为ViewPort坐标传给Shader
            // 为了简化，这里仅提供点击穿透逻辑
            return Vector4.zero;
        }
    }
}