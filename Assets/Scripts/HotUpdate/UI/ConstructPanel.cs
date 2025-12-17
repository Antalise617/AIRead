using System.Collections.Generic;
using cfg; // 引入配置表命名空间
using GameFramework.Core;
using GameFramework.ECS.Components;
using GameFramework.Managers;
using GameFramework.UI;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework.HotUpdate.UI
{
    /// <summary>
    /// 建造选择面板
    /// </summary>
    public class ConstructPanel : UIPanel
    {
        // === UI 绑定部分 ===
        [UIBind] private Button _btnIsland;

        [UIBind] private Button _btnBuilding;

        [UIBind] private Button _btnBridge;

        [UIBind] private Button _btnClose;

        [UIBind] private GameObject _itemGridRoot;

        // === 内部变量 ===
        private List<ConstructItemView> _itemViews = new List<ConstructItemView>();
        private PlacementType _currentType = PlacementType.Island;
        private const int MaxItems = 18;

        protected override void OnInit()
        {
            base.OnInit();

            // 注册大类切换事件
            _btnIsland.onClick.AddListener(() => SwitchCategory(PlacementType.Island));
            _btnBuilding.onClick.AddListener(() => SwitchCategory(PlacementType.Building));
            _btnBridge.onClick.AddListener(() => SwitchCategory(PlacementType.Bridge));

            // 注册关闭事件
            _btnClose.onClick.AddListener(Hide);

            // === 初始化 Item 列表 (修改部分) ===
            // 遍历 Grid 下的所有子节点，获取 Button 组件
            foreach (Transform child in _itemGridRoot.transform)
            {
                Button btn = child.GetComponent<Button>();

                // 只有挂载了 Button 组件的物体才被视为有效 Item
                if (btn != null)
                {
                    var itemView = new ConstructItemView(btn);
                    int index = _itemViews.Count;

                    // 使用存储的 Button 引用绑定点击事件
                    itemView.ItemButton.onClick.AddListener(() => OnItemClicked(index));

                    _itemViews.Add(itemView);
                }

                if (_itemViews.Count >= MaxItems) break;
            }

            Debug.Log($"[ConstructPanel] 初始化完成，有效按钮数量: {_itemViews.Count}");
        }

        protected override void OnShow()
        {
            base.OnShow();
            // 默认显示岛屿分类
            SwitchCategory(PlacementType.Island);
        }

        private void SwitchCategory(PlacementType type)
        {
            _currentType = type;
            RefreshList();
        }

        /// <summary>
        /// 刷新列表内容
        /// </summary>
        private void RefreshList()
        {
            var tables = ConfigManager.Instance.Tables;
            if (tables == null) return;

            // 获取对应数据列表
            System.Collections.IList dataList = null;

            switch (_currentType)
            {
                case PlacementType.Island:
                    dataList = tables.IslandCfg.DataList;
                    break;
                case PlacementType.Building:
                    dataList = tables.BuildingCfg.DataList;
                    break;
                case PlacementType.Bridge:
                    dataList = tables.BridgeCfg.DataList;
                    break;
            }

            if (dataList == null) return;

            for (int i = 0; i < _itemViews.Count; i++)
            {
                if (i < dataList.Count)
                {
                    // 有数据：显示并设置
                    _itemViews[i].SetActive(true);

                    var itemData = dataList[i];
                    int id = 0;
                    string name = "Unknown";

                    // 使用模式匹配获取属性，避免 dynamic 问题
                    if (itemData is Island_Config island)
                    {
                        id = island.Id;
                        name = island.Name;
                    }
                    else if (itemData is Building_Config building)
                    {
                        id = building.Id;
                        name = building.Name;
                    }
                    else if (itemData is Bridge_Config bridge)
                    {
                        id = bridge.Id;
                        name = $"桥梁 {bridge.Id}"; // 桥梁表可能没有 Name 字段
                    }

                    _itemViews[i].SetData(id, name);
                }
                else
                {
                    // 无数据：隐藏
                    _itemViews[i].SetActive(false);
                }
            }
        }

        /// <summary>
        /// 点击具体物品
        /// </summary>
        private void OnItemClicked(int index)
        {
            if (index < 0 || index >= _itemViews.Count) return;

            int objectId = _itemViews[index].DataId;
            Debug.Log($"[ConstructPanel] 选中物体 ID: {objectId}");

            TriggerPlacementSystem(objectId, _currentType);
            Hide();
        }

        /// <summary>
        /// 触发 ECS 放置系统
        /// </summary>
        private void TriggerPlacementSystem(int objectId, PlacementType type)
        {
            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null) return;

            var entityManager = world.EntityManager;
            var query = entityManager.CreateEntityQuery(typeof(PlacementStateComponent));

            if (query.CalculateEntityCount() > 0)
            {
                var entity = query.GetSingletonEntity();
                var state = entityManager.GetComponentData<PlacementStateComponent>(entity);

                state.IsActive = true;
                state.Type = type;
                state.CurrentObjectId = objectId;
                state.RotationIndex = 0;

                entityManager.SetComponentData(entity, state);
            }
        }

        // =========================================================
        // 内部类：Item 视图包装器 (已修改为直接持有 Button)
        // =========================================================
        private class ConstructItemView
        {
            // 直接记录 Button 引用
            public Button ItemButton;
            public TextMeshProUGUI NameText;
            public int DataId;

            public ConstructItemView(Button button)
            {
                ItemButton = button;
                // 尝试在 Button 的子节点中查找 TextMeshProUGUI
                NameText = button.GetComponentInChildren<TextMeshProUGUI>();
            }

            public void SetActive(bool active)
            {
                // 通过 Button 引用设置 GameObject 的显隐
                ItemButton.gameObject.SetActive(active);
            }

            public void SetData(int id, string name)
            {
                DataId = id;
                if (NameText != null)
                {
                    NameText.text = name;
                }
            }
        }
    }
}