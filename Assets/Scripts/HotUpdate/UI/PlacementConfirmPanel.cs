using UnityEngine;
using UnityEngine.UI;
using GameFramework.UI;
using GameFramework.Core; // 引用 AOT 定义的接口
using GameFramework.ECS.Systems; // 引用 AOT 定义的 System
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

        // 实现接口属性
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

            // 获取 ECS 系统引用 (Hotfix 可以引用 AOT)
            var world = World.DefaultGameObjectInjectionWorld;
            if (world != null)
            {
                _placementSystem = world.GetExistingSystemManaged<PlacementSystem>();
            }
        }

        private void OnConfirmClick()
        {
            if (_placementSystem != null) _placementSystem.ConfirmPlacement();
            Hide();
        }

        private void OnCancelClick()
        {
            if (_placementSystem != null) _placementSystem.CancelPlacement();
            Hide();
        }

        private void OnRotateClick()
        {
            if (_placementSystem != null) _placementSystem.RotatePreview();
        }

        // 实现接口方法
        public void UpdatePosition(Vector3 worldPosition)
        {
            if (Camera.main == null || _parentCanvas == null) return;

            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, worldPosition);
            screenPos.y += 100f;

            transform.position = screenPos;
        }
    }
}