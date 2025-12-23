using System.Collections.Generic;
using cfg; // 引入 Luban 配置命名空间
using GameFramework.Core;
using GameFramework.ECS.Components;
using GameFramework.Managers;
using GameFramework.UI;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
using SuperScrollView; // 确保程序集已引用

namespace GameFramework.HotUpdate.UI
{
    /// <summary>
    /// 建造选择面板 - 基于 Super ScrollView LoopGridView 重构
    /// </summary>
    public class ConstructPanel : UIPanel
    {
        // === UI 绑定部分 ===
        [UIBind] private Button m_btn_IslandButton;
        [UIBind] private Button m_btn_BuildingButton;
        [UIBind] private Button m_btn_BridgeButton;
        [UIBind] private Button m_btn_CloseButton;

        // 使用 LoopGridView 实现网格排列
        [UIBind] private LoopGridView m_obj_GridRoot;

        // === 内部变量 ===
        private PlacementType _currentType = PlacementType.Island;
        private System.Collections.IList _currentDataList; // 缓存当前分类的数据源

        protected override void OnInit()
        {
            base.OnInit();

            // 注册页签切换事件
            m_btn_IslandButton.onClick.AddListener(() => SwitchCategory(PlacementType.Island));
            m_btn_BuildingButton.onClick.AddListener(() => SwitchCategory(PlacementType.Building));
            m_btn_BridgeButton.onClick.AddListener(() => SwitchCategory(PlacementType.Bridge));

            // 注册关闭事件
            m_btn_CloseButton.onClick.AddListener(Hide);

            // === 初始化无限网格列表 ===
            // 参数1：初始数量；参数2：4参数回调函数
            // 注意：请确保在 Inspector 中配置了名为 "ConstructItem" 的 Item Config
            m_obj_GridRoot.InitGridView(0, OnGetItemByIndex);
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
            RefreshData();
        }

        /// <summary>
        /// 刷新数据源并驱动 UI 更新
        /// </summary>
        private void RefreshData()
        {
            var tables = ConfigManager.Instance.Tables;
            if (tables == null) return;

            // 1. 获取 Luban 对应数据列表
            switch (_currentType)
            {
                case PlacementType.Island:
                    _currentDataList = tables.TbIsland.DataList;
                    LogIslandCosts(tables); // 执行建造消耗 Log 打印逻辑
                    break;
                case PlacementType.Building:
                    _currentDataList = tables.TbBuild.DataList;
                    break;
                case PlacementType.Bridge:
                    _currentDataList = tables.TbBridgeConfig.DataList;
                    break;
            }

            // 2. 更新网格总数并刷新可见项
            int count = _currentDataList?.Count ?? 0;
            m_obj_GridRoot.SetListItemCount(count, true);
            m_obj_GridRoot.RefreshAllShownItem();
        }

        /// <summary>
        /// LoopGridView 的渲染回调函数
        /// 参数列表必须匹配：(LoopGridView, itemIndex, rowIndex, colIndex)
        /// </summary>
        private LoopGridViewItem OnGetItemByIndex(LoopGridView gridView, int itemIndex, int rowIndex, int colIndex)
        {
            if (_currentDataList == null || itemIndex < 0 || itemIndex >= _currentDataList.Count)
                return null;

            // 1. 获取 Item 实例
            // 确保这里的 "ConstructItem" 与 Inspector 中 Item Config List 的 Item Name 完全一致
            LoopGridViewItem item = gridView.NewListViewItem("ConstructButton");
            if (item == null) return null;

            // 2. 获取视图脚本
            var itemView = item.GetComponent<ConstructItemViewScript>();
            if (itemView == null)
            {
                Debug.LogError($"[ConstructPanel] Prefab 身上缺少 ConstructItemViewScript！");
                return item;
            }

            // 3. 解析 Luban 数据
            var itemData = _currentDataList[itemIndex];
            int id = 0;
            string name = "Unknown";

            if (itemData is Island island) { id = island.Id; name = island.Name; }
            else if (itemData is Build building) { id = building.Id; name = building.Name; }
            else if (itemData is BridgeConfig bridge) { id = bridge.Id; name = $"桥梁 {bridge.Id}"; }

            // 4. 设置数据
            itemView.SetData(id, name, () => OnItemClicked(id));

            // 【新增】强制设置网格项在数据列表中的索引，防止显示错乱
            item.ItemIndex = itemIndex;

            return item;
        }

        private void OnItemClicked(int objectId)
        {
            Debug.Log($"[ConstructPanel] 选中物体 ID: {objectId}");
            TriggerPlacementSystem(objectId, _currentType); // 触发放置系统
            Hide();
        }

        /// <summary>
        /// 基于 Luban 嵌套列表结构打印岛屿建造资源消耗
        /// </summary>
        private void LogIslandCosts(cfg.Tables tables)
        {
            if (tables.TbGameConfig.DataList.Count > 0)
            {
                // Luban 嵌套列表类型：List<List<int>>
                var config = tables.TbGameConfig.DataList[0];
                var costs = config.IslandConstructionCosts;

                string debugLog = "[ConstructPanel] 建造岛屿所需物品：";
                foreach (System.Collections.Generic.List<int> cost in costs)
                {
                    if (cost.Count >= 2)
                    {
                        // cost[0] 为物品ID, cost[1] 为所需数量
                        debugLog += $" [物品ID: {cost[0]}, 数量: {cost[1]}]";
                    }
                }
                Debug.Log(debugLog);
            }
        }

        /// <summary>
        /// 触发 AOT 侧的 ECS 放置系统
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
    }
}