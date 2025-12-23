using UnityEngine;
using UnityEngine.UI;
using GameFramework.Core;
using GameFramework.Managers;
using Cysharp.Threading.Tasks; // 必须引用，用于 await
using GameFramework.ECS.Systems;
using GameFramework.UI;
using Game.HotUpdate.UI; // 必须引用，为了识别 IslandInfoPanel 类型
using Unity.Entities;    // 引用 Entity

namespace Game.HotUpdate
{
    public class ClickIslandWidget : UIFollowPanel
    {
        [UIBind] public Button m_btn_IslandInfoButton;      // 岛屿信息
        [UIBind] public Button m_btn_IslandDestroyButton;   // 拆除岛屿
        [UIBind] public Button m_btn_IslandUpLevelButton;   // 岛屿升级

        public override void Initialize()
        {
            base.Initialize();

            // 绑定按钮事件
            m_btn_IslandInfoButton.onClick.AddListener(OnInfoClick);
            m_btn_IslandDestroyButton.onClick.AddListener(OnDeleteClick);
            m_btn_IslandUpLevelButton.onClick.AddListener(OnUpgradeClick);
        }

        // -----------------------------------------------------------------------
        // 修改重点：改为 async void，并使用 await 获取面板实例
        // -----------------------------------------------------------------------
        private async void OnInfoClick()
        {
            // 1. 异步等待面板打开，并获取返回的实例
            IslandInfoPanel panel = await UIManager.Instance.ShowPanelAsync<IslandInfoPanel>("IslandInfoPanel");

            // 2. 检查面板是否成功获取
            if (panel != null)
            {
                // _targetEntity 是父类 UIFollowPanel 中的成员，代表当前跟随/选中的实体
                if (_targetEntity != Entity.Null)
                {
                    // 3. 调用面板的初始化方法，传入实体
                    panel.SetIsland(_targetEntity);
                }
            }
            else
            {
                Debug.LogError("无法打开 IslandInfoPanel");
            }

            // 4. 关闭当前的小控件（气泡）
            CloseSelf();
        }

        private void OnDeleteClick()
        {
            // 假设我们能通过组件获取岛屿的 ID 和大小（实际项目中应从实体的组件数据中读取）
            // gridSystem.UnregisterIsland(anchorPos, size, airspace, islandId);

            // 确保实体有效再销毁
            if (_entityManager.Exists(_targetEntity))
            {
                // 直接销毁实体 (注意：正式项目中建议通过 System 或 CommandBuffer 销毁，或调用 Unregister 逻辑)
                _entityManager.DestroyEntity(_targetEntity);
            }

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
            // 获取当前 Widget 的名称并隐藏 (例如 "ClickIslandWidget")
            // 注意：确保你的 Prefab 名字和传入 HidePanel 的 Key 一致
            // 如果你的 UIManager 逻辑是需要去掉 (Clone) 后缀的，请保持原样，否则直接传 name 即可
            string panelKey = this.gameObject.name.Replace("(Clone)", "");
            UIManager.Instance.HidePanel(panelKey);
        }
    }
}