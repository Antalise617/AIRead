using UnityEngine;
using UnityEngine.UI;
using GameFramework.UI;
using GameFramework.Core;
using GameFramework.ECS.Systems;
using Unity.Entities;
using GameFramework.Managers;
using GameFramework.ECS.Components;
using Cysharp.Threading.Tasks; // [新增] 引入 UniTask

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

            // [修改] 使用 UniTask 的方法包装
            btnConfirm.onClick.AddListener(() => { OnConfirmClick().Forget(); });

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

        // [核心修改] 改为 async UniTaskVoid
        private async UniTaskVoid OnConfirmClick()
        {
            if (_placementSystem == null)
            {
                Hide();
                return;
            }

            // 1. 禁用按钮，防止在网络请求期间玩家重复点击
            btnConfirm.interactable = false;

            try
            {
                // 2. 等待 PlacementSystem 完成（发送请求 -> 等待服务器 -> 本地生成）
                // 注意：这里不再有返回值，因为 System 内部会根据结果决定是否关闭面板
                await _placementSystem.ConfirmPlacement();
            }
            finally
            {
                // 3. 恢复按钮状态（如果面板还没被 System 关闭的话）
                // 比如网络请求失败，或者放置的是桥梁（需要连续放置），面板还需保持开启
                if (this != null && gameObject != null)
                {
                    btnConfirm.interactable = true;
                }
            }

            // [注意] 这里不需要手动调用 Hide()。
            // 因为如果放置成功，PlacementSystem.CleanupPreview() 会通过接口调用本面板的 Hide()。
            // 如果放置失败（网络错误），面板应该保持开启，让玩家重试或取消。
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

        public void UpdatePosition(Vector3 worldPosition)
        {
            if (Camera.main == null || _parentCanvas == null) return;

            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, worldPosition);
            screenPos.y += 100f;

            transform.position = screenPos;
        }
    }
}