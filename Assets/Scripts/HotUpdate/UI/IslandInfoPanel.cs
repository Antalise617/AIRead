using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;
using Unity.Mathematics;
using GameFramework.UI;
using GameFramework.ECS.Components;
using GameFramework.Managers;
using cfg; // 确保引用了 Luban 生成的配置命名空间
using GameFramework.Core;
using GameFramework.ECS.Systems;
using TMPro;

namespace Game.HotUpdate.UI
{
    public class IslandInfoPanel : UIPanel
    {
        [UIBind] public TextMeshProUGUI m_tmp_IslandName;
        [UIBind] public TextMeshProUGUI m_tmp_BonusInfo;
        [UIBind] public Transform m_obj_SurfaceInfo;
        [UIBind] private Button m_btn_CloseButton;

        public Sprite _spriteOutOfRange; // 海洋/无效区域
        public Sprite _spriteUnbuilt;    // 可建造区域（空地）
        public Sprite _spriteBuilt;      // 已建造区域

        private Image[,] _gridImages = new Image[9, 9];
        private bool _isInitialized = false;

        protected override void OnInit()
        {
            base.OnInit();
            InitializeGridImages();
            m_btn_CloseButton.onClick.AddListener(Hide);
        }

        private void InitializeGridImages()
        {
            if (_isInitialized) return;

            foreach (Transform child in m_obj_SurfaceInfo)
            {
                string name = child.name; // "(0,0)"
                if (name.StartsWith("(") && name.EndsWith(")"))
                {
                    string content = name.Substring(1, name.Length - 2);
                    string[] parts = content.Split(',');

                    if (parts.Length == 2 &&
                        int.TryParse(parts[0], out int x) &&
                        int.TryParse(parts[1], out int y))
                    {
                        if (x >= 0 && x < 9 && y >= 0 && y < 9)
                        {
                            _gridImages[x, y] = child.GetComponent<Image>();
                        }
                    }
                }
            }
            _isInitialized = true;
        }

        public void SetIsland(Entity islandEntity)
        {
            if (islandEntity == Entity.Null) return;
            if (!_isInitialized) Initialize();

            var em = World.DefaultGameObjectInjectionWorld.EntityManager;

            if (!em.HasComponent<IslandComponent>(islandEntity) ||
                !em.HasComponent<IslandDataComponent>(islandEntity))
            {
                Debug.LogError("[IslandInfoPanel] Entity missing required components!");
                return;
            }

            var islandCmp = em.GetComponentData<IslandComponent>(islandEntity);
            var islandData = em.GetComponentData<IslandDataComponent>(islandEntity);

            // 读取配置表 (Luban)
            Island islandConfig = ConfigManager.Instance.Tables.TbIsland.Get(islandCmp.ConfigId);

            // 刷新文本信息 (这里包含你要求的别名转换)
            RefreshTextInfo(islandConfig, islandData);

            // 刷新网格状态
            RefreshGridVisuals(islandConfig, islandCmp, islandEntity);
        }

        // =========================================================
        // 修改点：解析枚举 Alias 并设置文本
        // =========================================================
        private void RefreshTextInfo(Island config, IslandDataComponent data)
        {
            // 1. 设置岛屿名字
            m_tmp_IslandName.text = config.Name;

            // 2. 解析岛屿类型 (IslandType)
            string islandTypeName = "未知类型";
            switch ((cfg.zsEnum.IslandType)data.IslandType)
            {
                case cfg.zsEnum.IslandType.GrasslandIslands: islandTypeName = "草原岛屿"; break;
                case cfg.zsEnum.IslandType.ForestIsland: islandTypeName = "森林岛屿"; break;
                case cfg.zsEnum.IslandType.RockyIsland: islandTypeName = "岩石岛屿"; break;
                case cfg.zsEnum.IslandType.WaterIslands: islandTypeName = "水域岛屿"; break;
            }

            // 3. 解析加成类型别名 (BonusType)
            // 对应 __enums__.xlsx 中的定义
            string bonusAlias = "无加成";
            switch ((cfg.zsEnum.IslandBonusType)data.BonusType)
            {
                case cfg.zsEnum.IslandBonusType.Empty:
                    bonusAlias = "无加成";
                    break;
                case cfg.zsEnum.IslandBonusType.FarmProductionBonus:
                    bonusAlias = "农场产出加成";
                    break;
                case cfg.zsEnum.IslandBonusType.LumberyardOutputBonus:
                    bonusAlias = "伐木场产出加成";
                    break;
                case cfg.zsEnum.IslandBonusType.QuarryOutputBonus:
                    bonusAlias = "采石场产出加成";
                    break;
                default:
                    bonusAlias = "未知加成";
                    break;
            }

            // 4. 拼接最终显示文本
            string bonusDesc = "";

            if (data.BonusType != (int)cfg.zsEnum.IslandBonusType.Empty && data.BonusValue > 0)
            {
                // 假设 BonusValue 是万分比 (根据 IslandLevel 表备注)，这里转换为百分比显示
                float percentValue = data.BonusValue / 100.0f;
                bonusDesc += $"{bonusAlias} +{percentValue}%";
            }
            else
            {
                bonusDesc += $"加成: {bonusAlias}";
            }

            m_tmp_BonusInfo.text = bonusDesc;
        }

        private void RefreshGridVisuals(Island config, IslandComponent islandCmp, Entity islandEntity)
        {
            int sizeX = islandCmp.Size.x;
            int sizeY = islandCmp.Size.z;

            var gridSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<GridSystem>();
            if (gridSystem == null) return;

            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    Image targetImage = _gridImages[x, y];
                    if (targetImage == null) continue;

                    bool isValidRange = x < sizeX && y < sizeY;

                    if (!isValidRange)
                    {
                        targetImage.sprite = _spriteOutOfRange;
                        continue;
                    }

                    bool isOccupied = CheckGridOccupancy(islandEntity, x, y);

                    if (isOccupied)
                    {
                        targetImage.sprite = _spriteBuilt;
                    }
                    else
                    {
                        targetImage.sprite = _spriteUnbuilt;
                    }
                }
            }
        }

        private bool CheckGridOccupancy(Entity islandEntity, int x, int y)
        {
            var gridSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<GridSystem>();
            if (gridSystem == null || !gridSystem.WorldGrid.IsCreated)
            {
                return false;
            }

            int3 islandAnchorPos = int3.zero;
            var em = World.DefaultGameObjectInjectionWorld.EntityManager;

            if (em.HasComponent<GridPositionComponent>(islandEntity))
            {
                islandAnchorPos = em.GetComponentData<GridPositionComponent>(islandEntity).Value;
            }
            else if (em.HasComponent<Unity.Transforms.LocalTransform>(islandEntity))
            {
                var transform = em.GetComponentData<Unity.Transforms.LocalTransform>(islandEntity);
                float cellSize = 2.0f;

                // 使用 EntityQuery 而不是 SystemAPI，避免在 UI 线程报错
                var configQuery = em.CreateEntityQuery(typeof(GridConfigComponent));
                if (configQuery.HasSingleton<GridConfigComponent>())
                {
                    cellSize = configQuery.GetSingleton<GridConfigComponent>().CellSize;
                }

                islandAnchorPos = new int3(
                    (int)math.round(transform.Position.x / cellSize),
                    (int)math.round(transform.Position.y / cellSize),
                    (int)math.round(transform.Position.z / cellSize)
                );
            }

            // GridSystem 逻辑：建造层位于 AnchorPos.y + 1
            int3 targetGridPos = islandAnchorPos + new int3(x, 1, y);

            if (gridSystem.WorldGrid.TryGetValue(targetGridPos, out GridCellData cellData))
            {
                // 如果有建筑ID或者类型是Building，视为已建造
                if (!cellData.BuildingID.IsEmpty || cellData.Type == GridType.Building)
                {
                    return true;
                }
            }

            return false;
        }
    }
}