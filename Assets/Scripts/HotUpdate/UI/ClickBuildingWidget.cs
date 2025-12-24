using UnityEngine.UI;
using UnityEngine;
using GameFramework.Core;
using GameFramework.Managers;
using Cysharp.Threading.Tasks;
using GameFramework.ECS.Systems;
using GameFramework.UI;
using Unity.Entities;
using GameFramework.ECS.Components; // 必须引用，用于访问 BuildingComponent

namespace Game.HotUpdate
{
    public class ClickBuildingWidget : UIFollowPanel
    {
        [UIBind] public Button m_btn_BuildingInfoButton;      // 建筑信息
        [UIBind] public Button m_btn_BuildingRecycleButton;   // 收起建筑
        [UIBind] public Button m_btn_BuildingUpLevelButton;   // 建筑升级

        public override void Initialize()
        {
            base.Initialize();

            // 绑定按钮事件
            m_btn_BuildingInfoButton.onClick.AddListener(OnInfoClick);
            m_btn_BuildingRecycleButton.onClick.AddListener(OnDeleteClick);
            m_btn_BuildingUpLevelButton.onClick.AddListener(OnUpgradeClick);
        }

        // ========================================================================
        // 【核心修改】根据建筑类型分发信息面板
        // ========================================================================
        private async void OnInfoClick()
        {
            // 1. 校验实体有效性
            if (_targetEntity == Entity.Null || !_entityManager.Exists(_targetEntity))
            {
                CloseSelf();
                return;
            }

            // 2. 获取建筑类型
            int buildingType = 0;
            if (_entityManager.HasComponent<BuildingComponent>(_targetEntity))
            {
                var buildComp = _entityManager.GetComponentData<BuildingComponent>(_targetEntity);
                buildingType = buildComp.BuildingType;
            }
            else
            {
                Debug.LogError("[ClickBuildingWidget] 目标实体缺少 BuildingComponent，无法判断类型");
                CloseSelf();
                return;
            }

            // 3. 根据类型打开对应面板
            // 对应 zsEnum.BuildingType: 1:Core, 2:Supply, 3:Output, 4:Service, 5:Experience
            switch (buildingType)
            {
                case 3: // Output (产出/工厂类)
                    await OpenProductionPanel();
                    break;

                case 4: // Service (服务/商店类)
                    await OpenServicePanel();
                    break;

                case 1: // Core (核心类)
                case 2: // Supply (供给类)
                case 5: // Experience (体验类)
                default:
                    Debug.Log($"[ClickBuildingWidget] 建筑类型 {buildingType} 的信息面板暂未实现 (TODO)");
                    break;
            }

            // 4. 关闭当前的悬浮小控件
            CloseSelf();
        }

        // --- 辅助方法：打开生产面板 ---
        private async UniTask OpenProductionPanel()
        {
            var panel = await UIManager.Instance.ShowPanelAsync<BuildingProductionPanel>("BuildingProductionPanel");
            if (panel != null)
            {
                panel.SetBuildingEntity(_targetEntity);
            }
        }

        // --- 辅助方法：打开服务面板 ---
        private async UniTask OpenServicePanel()
        {
            var panel = await UIManager.Instance.ShowPanelAsync<BuildingServicePanel>("BuildingServicePanel");
            if (panel != null)
            {
                panel.SetBuildingEntity(_targetEntity);
            }
        }

        // ========================================================================

        private void OnDeleteClick()
        {
            // 简单的销毁逻辑，实际项目中可能需要调用 GridSystem.Unregister...
            if (_entityManager.Exists(_targetEntity))
            {
                _entityManager.DestroyEntity(_targetEntity);
            }
            CloseSelf();
        }

        private void OnUpgradeClick()
        {
            Debug.Log("[ClickBuildingWidget] 升级功能暂未实现");
        }

        private void CloseSelf()
        {
            UIManager.Instance.HidePanel("ClickBuildingWidget");
        }
    }
}