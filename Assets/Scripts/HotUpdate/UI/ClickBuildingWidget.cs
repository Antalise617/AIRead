using UnityEngine.UI;
using GameFramework.Core;
using GameFramework.Managers;
using Cysharp.Threading.Tasks;
using GameFramework.ECS.Systems; // 需要访问网格系统进行拆除
using GameFramework.UI;

namespace Game.HotUpdate
{
    public class ClickBuildingWidget : UIFollowPanel
    {
        [UIBind] public Button m_btn_BuildingInfoButton;      // 建筑信息
        [UIBind] public Button m_btn_BuildingRecycleButton;    // 收起建筑
        [UIBind] public Button m_btn_BuildingUpLevelButton;   // 建筑升级

        public override void Initialize()
        {
            base.Initialize();

            // 绑定按钮事件
            m_btn_BuildingInfoButton.onClick.AddListener(OnInfoClick);
            m_btn_BuildingRecycleButton.onClick.AddListener(OnDeleteClick);
            m_btn_BuildingUpLevelButton.onClick.AddListener(OnUpgradeClick);
        }

        private void OnInfoClick()
        {

            // 升级功能暂无
            UnityEngine.Debug.Log("功能暂未实现");
            // 1. 打开信息面板
            UIManager.Instance.ShowPanelAsync<BuildingProductionPanel>("BuildingProductionPanel").Forget();
            // 2. 关闭当前小控件
            CloseSelf();
        }

        private void OnDeleteClick()
        {
            // 获取网格系统执行拆除逻辑
            //var gridSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<GridSystem>();

            // 假设我们能通过组件获取岛屿的 ID 和大小（实际项目中应从实体的组件数据中读取）
            // gridSystem.UnregisterIsland(anchorPos, size, airspace, islandId);

            // 直接销毁实体
            _entityManager.DestroyEntity(_targetEntity);

            // 关闭当前小控件
            CloseSelf();
        }

        private void OnUpgradeClick()
        {
            // 升级功能暂无
            UnityEngine.Debug.Log("[ClickIslandWidget] 升级功能暂未实现");
        }

        private void CloseSelf()
        {
            UIManager.Instance.HidePanel("ClickIslandWidget");
        }
    }
}