using UnityEngine;
using UnityEngine.UI;
using GameFramework.UI;
using GameFramework.Core;
using GameFramework.ECS.Systems;
using Unity.Entities;
using GameFramework.Managers;
using GameFramework.ECS.Components;

namespace GameFramework.HotUpdate.UI
{
    public class PlacementConfirmPanel : UIPanel, IPlacementConfirmPanel
    {
        [UIBind] public Button btnConfirm;
        [UIBind] public Button btnCancel;
        [UIBind] public Button btnRotate;

        private PlacementSystem _placementSystem;
        private Canvas _parentCanvas;

        public bool IsVisible => gameObject.activeSelf;

        protected override void OnInit()
        {
            base.OnInit();
            _parentCanvas = GetComponentInParent<Canvas>();

            btnConfirm.onClick.AddListener(OnConfirmClick);
            btnCancel.onClick.AddListener(OnCancelClick);
            if (btnRotate != null)
            {
                btnRotate.onClick.AddListener(OnRotateClick);
            }

            var world = World.DefaultGameObjectInjectionWorld;
            if (world != null)
            {
                _placementSystem = world.GetExistingSystemManaged<PlacementSystem>();
            }
        }

        private void OnConfirmClick()
        {
            if (_placementSystem != null)
            {
                // 【核心修改】检查 ConfirmPlacement 的返回值
                // 如果返回 true，说明建造结束，关闭面板
                // 如果返回 false (如桥梁)，说明继续建造，保持面板开启 (或等用户再次拖动时自动隐藏)
                bool isFinished = _placementSystem.ConfirmPlacement();
                if (isFinished)
                {
                    Hide();
                }
            }
            else
            {
                Hide();
            }
        }

        private void OnCancelClick()
        {
            // 取消按钮逻辑不变：强制取消并退出
            if (_placementSystem != null) _placementSystem.CancelPlacement();
            Hide();
        }

        private void OnRotateClick()
        {
            if (_placementSystem != null) _placementSystem.RotatePreview();
        }

        public void UpdatePosition(Vector3 worldPosition)
        {
            if (Camera.main == null || _parentCanvas == null) return;

            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, worldPosition);
            screenPos.y += 100f;

            transform.position = screenPos;
        }
    }
}