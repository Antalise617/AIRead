using System.Collections.Generic;
using System.Linq;
using cfg;
using cfg.zsEnum;
using GameFramework.Core;
using GameFramework.ECS.Components;
using GameFramework.Managers;
using GameFramework.UI;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
using SuperScrollView;

namespace GameFramework.HotUpdate.UI
{
    /// <summary>
    /// 建造选择面板 - 双网格显示版 (适配消耗显示)
    /// </summary>
    public class ConstructPanel : UIPanel
    {
        // === 一级页签 ===
        [UIBind] private Button m_btn_IslandButton;
        [UIBind] private Button m_btn_BuildingButton;
        [UIBind] private Button m_btn_BridgeButton;
        [UIBind] private Button m_btn_CloseButton;

        // === 二级页签 (建筑类型筛选) ===
        [UIBind] private Button m_btn_CenterButton;      // 核心类 (Core)
        [UIBind] private Button m_btn_ProvisionButton;   // 供给类 (Supply)
        [UIBind] private Button m_btn_ProductionButton;  // 产出类 (Output)
        [UIBind] private Button m_btn_ServiceButton;     // 服务类 (Service)
        [UIBind] private Button m_btn_ExperienceButton;  // 体验类 (Experience)

        // === 列表容器 (分页显示) ===
        [UIBind] private LoopGridView m_obj_GridRoot1; // 显示前4个
        [UIBind] private LoopGridView m_obj_GridRoot2; // 显示后4个

        // === 内部变量 ===
        private PlacementType _currentType = PlacementType.Island;
        private BuildingType _currentBuildingFilter = BuildingType.Core;
        private System.Collections.IList _currentDataList;

        private const int ITEMS_PER_GRID = 4;

        protected override void OnInit()
        {
            base.OnInit();

            // 1. 注册页签事件
            m_btn_IslandButton.onClick.AddListener(() => SwitchCategory(PlacementType.Island));
            m_btn_BuildingButton.onClick.AddListener(() => SwitchCategory(PlacementType.Building));
            m_btn_BridgeButton.onClick.AddListener(() => SwitchCategory(PlacementType.Bridge));
            m_btn_CloseButton.onClick.AddListener(Hide);

            // 2. 注册二级页签事件
            m_btn_CenterButton.onClick.AddListener(() => SwitchBuildingFilter(BuildingType.Core));
            m_btn_ProvisionButton.onClick.AddListener(() => SwitchBuildingFilter(BuildingType.Supply));
            m_btn_ProductionButton.onClick.AddListener(() => SwitchBuildingFilter(BuildingType.Output));
            m_btn_ServiceButton.onClick.AddListener(() => SwitchBuildingFilter(BuildingType.Service));
            m_btn_ExperienceButton.onClick.AddListener(() => SwitchBuildingFilter(BuildingType.Experience));

            // 3. 初始化网格
            m_obj_GridRoot1.InitGridView(0, OnGetItemByIndex_Grid1);
            m_obj_GridRoot2.InitGridView(0, OnGetItemByIndex_Grid2);
        }

        protected override void OnShow()
        {
            base.OnShow();
            SwitchCategory(PlacementType.Island);
        }

        private void SwitchCategory(PlacementType type)
        {
            _currentType = type;
            bool isBuilding = (type == PlacementType.Building);
            SetSubTabsVisible(isBuilding);

            if (isBuilding)
            {
                SwitchBuildingFilter(BuildingType.Core);
            }
            else
            {
                RefreshData();
            }
        }

        private void SwitchBuildingFilter(BuildingType filterType)
        {
            _currentBuildingFilter = filterType;
            RefreshData();
        }

        private void SetSubTabsVisible(bool visible)
        {
            m_btn_CenterButton.gameObject.SetActive(visible);
            m_btn_ProvisionButton.gameObject.SetActive(visible);
            m_btn_ProductionButton.gameObject.SetActive(visible);
            m_btn_ServiceButton.gameObject.SetActive(visible);
            m_btn_ExperienceButton.gameObject.SetActive(visible);
        }

        private void RefreshData()
        {
            var tables = ConfigManager.Instance.Tables;
            if (tables == null) return;

            // 获取数据源
            switch (_currentType)
            {
                case PlacementType.Island:
                    _currentDataList = tables.TbIsland.DataList;
                    break;

                case PlacementType.Building:
                    _currentDataList = tables.TbBuild.DataList
                        .Where(data => data.BuildingType == _currentBuildingFilter)
                        .ToList();
                    break;

                case PlacementType.Bridge:
                    _currentDataList = tables.TbBridgeConfig.DataList;
                    break;
            }

            // 分配数据到两个网格
            int totalCount = _currentDataList?.Count ?? 0;
            int count1 = Mathf.Min(totalCount, ITEMS_PER_GRID);
            int count2 = Mathf.Clamp(totalCount - ITEMS_PER_GRID, 0, ITEMS_PER_GRID);

            m_obj_GridRoot1.SetListItemCount(count1, resetPos: true);
            m_obj_GridRoot1.RefreshAllShownItem();

            m_obj_GridRoot2.SetListItemCount(count2, resetPos: true);
            m_obj_GridRoot2.RefreshAllShownItem();
        }

        // --- Grid 回调 ---
        private LoopGridViewItem OnGetItemByIndex_Grid1(LoopGridView gridView, int itemIndex, int rowIndex, int colIndex)
        {
            return GetItemByIndexInternal(gridView, itemIndex, 0);
        }

        private LoopGridViewItem OnGetItemByIndex_Grid2(LoopGridView gridView, int itemIndex, int rowIndex, int colIndex)
        {
            return GetItemByIndexInternal(gridView, itemIndex, ITEMS_PER_GRID);
        }

        /// <summary>
        /// 核心方法：创建 Item 并填充数据（包含消耗信息）
        /// </summary>
        private LoopGridViewItem GetItemByIndexInternal(LoopGridView gridView, int itemIndex, int dataOffset)
        {
            int realDataIndex = itemIndex + dataOffset;

            if (_currentDataList == null || realDataIndex < 0 || realDataIndex >= _currentDataList.Count)
                return null;

            LoopGridViewItem item = gridView.NewListViewItem("ConstructButton");
            if (item == null) return null;

            var itemView = item.GetComponent<ConstructItemViewScript>();
            if (itemView == null) return item;

            // 解析 Luban 数据
            var itemData = _currentDataList[realDataIndex];
            int id = 0;
            string name = "Unknown";
            List<List<int>> costs = null; // 存储消耗数据 [[id, count], ...]

            var tables = ConfigManager.Instance.Tables;

            if (itemData is Island island)
            {
                id = island.Id;
                name = island.Name;
                // 岛屿消耗通常在全局配置中定义
                if (tables.TbGameConfig.DataList.Count > 0)
                {
                    costs = tables.TbGameConfig.DataList[0].IslandConstructionCosts;
                }
            }
            else if (itemData is Build building)
            {
                id = building.Id;
                name = building.Name;

                // 尝试从 BuildingLevel 表中查找对应的建造消耗 (假设 Build.Id 对应 Level 1 的配置)
                // 注意：如果您的表结构不同（比如 ID 不直接对应），请根据实际情况修改 key
                var levelData = tables.TbBuildingLevel.GetOrDefault(id);
                if (levelData != null)
                {
                    costs = levelData.UpCost; // 使用升级/建造消耗字段
                }
            }
            else if (itemData is BridgeConfig bridge)
            {
                id = bridge.Id;
                name = bridge.Name;
                // 桥梁目前表中没有直接的消耗字段，如果需要可在此处手动构造或从其他表读取
                // costs = new List<List<int>> { new List<int> { 1001, 10 } }; // 示例：10个木头
            }

            // 【关键修改】传递 costs 数据给 Item
            itemView.SetData(id, name, costs, () => OnItemClicked(id));

            item.ItemIndex = itemIndex;
            return item;
        }

        private void OnItemClicked(int objectId)
        {
            Debug.Log($"[ConstructPanel] 选中物体 ID: {objectId}");

            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null) return;

            var entityManager = world.EntityManager;
            var query = entityManager.CreateEntityQuery(typeof(PlacementStateComponent));

            if (query.CalculateEntityCount() > 0)
            {
                var entity = query.GetSingletonEntity();
                var state = entityManager.GetComponentData<PlacementStateComponent>(entity);

                state.IsActive = true;
                state.Type = _currentType;
                state.CurrentObjectId = objectId;
                state.RotationIndex = 0;

                entityManager.SetComponentData(entity, state);
            }

            Hide();
        }
    }
}