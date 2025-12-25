using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using GameFramework.Managers;

namespace GameFramework.Core
{
    /// <summary>
    /// 跟随实体的 UI 面板基类 (位于 AOT 层)
    /// </summary>
    public abstract class UIFollowPanel : UIPanel
    {
        protected Entity _targetEntity;
        protected EntityManager _entityManager;
        protected Camera _mainCamera;
        protected RectTransform _rectTransform;

        [Header("跟随配置")]
        public Vector3 offset = new Vector3(0, 2.5f, 0);

        public override void Initialize()
        {
            base.Initialize();
            _rectTransform = GetComponent<RectTransform>();
            _mainCamera = Camera.main;
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        // --- 核心方法：必须在基类中定义，子类可根据需要 override ---
        public virtual void Bind(Entity target)
        {
            _targetEntity = target;
            this.Show();
        }

        protected virtual void LateUpdate()
        {
            // 实体销毁检查
            if (_targetEntity == Entity.Null || !_entityManager.Exists(_targetEntity))
            {
                OnTargetLost();
                return;
            }

            // 坐标转换逻辑
            if (_entityManager.HasComponent<LocalTransform>(_targetEntity))
            {
                var transformData = _entityManager.GetComponentData<LocalTransform>(_targetEntity);
                Vector3 worldPos = transformData.Position;
                Vector3 screenPos = _mainCamera.WorldToScreenPoint(worldPos + offset);

                if (screenPos.z > 0)
                {
                    _rectTransform.position = screenPos;
                }
                else
                {
                    _rectTransform.position = new Vector3(-10000, -10000, 0);
                }
            }
        }

        protected virtual void OnTargetLost()
        {
            // 获取面板名称并隐藏
            string panelKey = this.gameObject.name.Replace("(Clone)", "");
            UIManager.Instance.HidePanel(panelKey);
        }
    }
}