using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using GameFramework.Managers;
using Unity.Mathematics; // 确保引用 mathematics

namespace GameFramework.Core
{
    // 确保它继承自 UIPanel，这样 UIManager 才能统一管理
    public abstract class UIFollowPanel : UIPanel
    {
        protected Entity _targetEntity;
        protected EntityManager _entityManager;
        protected Camera _mainCamera;
        protected RectTransform _rectTransform;

        [Header("跟随配置")]
        public Vector3 offset = new Vector3(0, 2.5f, 0);

        // [新增] 记录自己的资源路径，用于“借书还书”时的凭证
        public string AssetPath { get; set; }

        public override void Initialize()
        {
            base.Initialize(); // 这一步会执行 UIBind
            _rectTransform = GetComponent<RectTransform>();
            _mainCamera = Camera.main;
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        public virtual void Bind(Entity target)
        {
            _targetEntity = target;
            this.Show();
            // 立即刷新一次位置，防止第一帧在原点闪烁
            UpdatePosition();
        }

        protected virtual void LateUpdate()
        {
            UpdatePosition();
        }

        private void UpdatePosition()
        {
            // 1. 检查实体是否存活
            if (_targetEntity == Entity.Null || !_entityManager.Exists(_targetEntity))
            {
                OnTargetLost();
                return;
            }

            // 2. 检查是否有 Transform 组件 (Entities 1.0+)
            if (_mainCamera != null && _entityManager.HasComponent<LocalToWorld>(_targetEntity))
            {
                var ltw = _entityManager.GetComponentData<LocalToWorld>(_targetEntity);
                float3 worldPos = ltw.Position;

                // 转换到屏幕坐标
                Vector3 screenPos = _mainCamera.WorldToScreenPoint((Vector3)worldPos + offset);

                // 3. 简单的视锥剔除 (在相机背面时隐藏)
                if (screenPos.z > 0)
                {
                    if (canvasGroup.alpha == 0) canvasGroup.alpha = 1;
                    _rectTransform.position = screenPos;
                }
                else
                {
                    canvasGroup.alpha = 0; // 隐藏但不是 Deactive
                }
            }
        }

        // [重写] 目标丢失时，不再是 HidePanel，而是 Recycle
        protected virtual void OnTargetLost()
        {
            UIManager.Instance.RecycleWidget(this);
        }

        // 供外部手动关闭
        public void CloseWidget()
        {
            OnTargetLost();
        }
    }
}