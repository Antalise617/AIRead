using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;
using Unity.Mathematics;
using GameFramework.UI;
using GameFramework.ECS.Components;
using GameFramework.Managers;
using cfg;
using GameFramework.Core;
using GameFramework.ECS.Systems;
using TMPro;
using Game.HotUpdate;
using Cysharp.Threading.Tasks; // 引用 UniTask

public class BuildingProductionPanel : UIPanel
{
    // --- 顶部基础信息 ---
    [UIBind] private TextMeshProUGUI m_tmp_BuildState;
    [UIBind] private TextMeshProUGUI m_tmp_BuildingName;
    [UIBind] private Image m_img_BuildingIcon;
    [UIBind] private TextMeshProUGUI m_tmp_IslandBonusInfo;
    [UIBind] private TextMeshProUGUI m_tmp_EmployeeBonusInfo;
    [UIBind] private TextMeshProUGUI m_tmp_PowerInfo;
    [UIBind] private Button m_btn_CloseButton;

    // --- 核心信息区域 ---
    [UIBind] private GameObject m_obj_Info; // 信息块的父节点

    // ========================================================================
    // 内部类：信息块 (InfoBlock) - 增加异步加载图标逻辑
    // ========================================================================
    private class InfoBlock
    {
        public GameObject Root;
        public Image Icon;
        public TextMeshProUGUI Name;
        public TextMeshProUGUI Text;

        // 记录当前正在显示(或加载中)的图标路径，防止异步覆盖
        private string _currentIconPath;

        public void SetActive(bool active) => Root.SetActive(active);

        public void SetContent(string name, string value, string iconPath = null)
        {
            if (Name != null) Name.text = name;
            if (Text != null) Text.text = value;
            SetActive(true);

            // 如果传了图标路径，则异步加载；否则保持 Prefab 原样
            if (!string.IsNullOrEmpty(iconPath) && Icon != null)
            {
                // 避免重复加载同一个图标
                if (_currentIconPath == iconPath) return;

                _currentIconPath = iconPath;
                LoadIconAsync(iconPath).Forget();
            }
        }

        private async UniTaskVoid LoadIconAsync(string path)
        {
            // 使用 ResourceManager 加载 Sprite (Addressable)
            var sprite = await ResourceManager.Instance.LoadAssetAsync<Sprite>(path);

            // 加载完成后，检查是否还需要显示这个图标（防止快速切换导致错乱）
            if (sprite != null && Icon != null && _currentIconPath == path)
            {
                Icon.sprite = sprite;
            }
        }
    }

    private List<InfoBlock> _infoBlocks = new List<InfoBlock>();
    private Entity _currentEntity;
    private EntityManager _entityManager;
    private bool _isEntitySet = false;

    protected override void OnInit()
    {
        base.OnInit();
        m_btn_CloseButton.onClick.AddListener(Hide);
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        // 初始化 6 个信息块 (Info_1 ~ Info_6)
        if (m_obj_Info != null)
        {
            for (int i = 1; i <= 6; i++)
            {
                string blockName = $"Info_{i}";
                Transform tf = m_obj_Info.transform.Find(blockName);
                if (tf != null)
                {
                    var block = new InfoBlock
                    {
                        Root = tf.gameObject,
                        Icon = tf.Find("m_img_InfoIcon")?.GetComponent<Image>(),
                        Name = tf.Find("m_tmp_InfoName")?.GetComponent<TextMeshProUGUI>(),
                        Text = tf.Find("m_tmp_InfoText")?.GetComponent<TextMeshProUGUI>()
                    };
                    _infoBlocks.Add(block);
                }
                else
                {
                    Debug.LogWarning($"[BuildingProductionPanel] 未找到信息块节点: {blockName}");
                    _infoBlocks.Add(new InfoBlock { Root = new GameObject("Dummy") });
                }
            }
        }
    }

    public void SetBuildingEntity(Entity entity)
    {
        if (entity == Entity.Null || !_entityManager.Exists(entity))
        {
            Hide();
            return;
        }

        _currentEntity = entity;
        _isEntitySet = true;

        RefreshStaticInfo();
    }

    private void Update()
    {
        if (_isEntitySet && _entityManager.Exists(_currentEntity))
        {
            RefreshDynamicInfo();
        }
        else if (_isEntitySet)
        {
            Hide();
        }
    }

    // ========================================================================
    // 静态信息刷新
    // ========================================================================
    private void RefreshStaticInfo()
    {
        if (_entityManager.HasComponent<BuildingComponent>(_currentEntity))
        {
            var buildingComp = _entityManager.GetComponentData<BuildingComponent>(_currentEntity);
            m_tmp_BuildingName.text = buildingComp.Name.ToString();

            // 如果建筑本身也有图标，也可以在这里加载
            // string iconId = buildingComp.ConfigId.ToString();
            // ResourceManager.Instance.LoadAssetAsync<Sprite>(iconId).ContinueWith(s => m_img_BuildingIcon.sprite = s).Forget();
        }

        foreach (var block in _infoBlocks) block.SetActive(false);

        // --- Info_1: 员工 (使用默认图标，iconPath 传 null) ---
        if (_entityManager.HasComponent<ProductionComponent>(_currentEntity))
        {
            var prod = _entityManager.GetComponentData<ProductionComponent>(_currentEntity);
            _infoBlocks[0].SetContent("岗位", $"0 / {prod.JobSlots}", null);
        }
        else
        {
            _infoBlocks[0].SetContent("岗位", "-", null);
        }

        // --- Info_2: 繁荣度 (使用默认图标) ---
        if (_entityManager.HasComponent<ProsperityComponent>(_currentEntity))
        {
            int val = _entityManager.GetComponentData<ProsperityComponent>(_currentEntity).Value;
            _infoBlocks[1].SetContent("繁荣度", val.ToString(), null);
        }
        else
        {
            _infoBlocks[1].SetContent("繁荣度", "0", null);
        }

        // --- Info_3: 储量 (使用默认图标，数值后续在Update刷新) ---
        _infoBlocks[2].SetContent("储量", "0 / 0", null);

        // --- Info_4 ~ Info_6: 产出物品列表 (动态加载图标) ---
        RefreshProductListInfo();
    }

    private void RefreshProductListInfo()
    {
        if (!_entityManager.HasComponent<ProductionComponent>(_currentEntity)) return;

        var prod = _entityManager.GetComponentData<ProductionComponent>(_currentEntity);
        DynamicBuffer<ProductionOutputElement> outputs = default;

        if (_entityManager.HasBuffer<ProductionOutputElement>(_currentEntity))
            outputs = _entityManager.GetBuffer<ProductionOutputElement>(_currentEntity);

        int count = outputs.IsCreated ? outputs.Length : 0;
        float interval = prod.ProductionInterval;

        // 遍历 Info_4 ~ Info_6 (List 索引 3, 4, 5)
        for (int i = 0; i < 3; i++)
        {
            int blockIndex = 3 + i;
            if (blockIndex >= _infoBlocks.Count) break;

            if (i < count)
            {
                var itemData = outputs[i];
                var itemCfg = GlobalInventoryManager.Instance.GetConfig(itemData.ItemId);
                string itemName = itemCfg != null ? itemCfg.Name : "未知物品";

                // 【核心逻辑】图标 Key = 物品ID数字
                string iconKey = itemData.ItemId.ToString();

                string infoText = $"{itemData.CountPerCycle} / {interval}s";

                // 设置内容并触发异步加载
                _infoBlocks[blockIndex].SetContent(itemName, infoText, iconKey);
            }
            else
            {
                _infoBlocks[blockIndex].SetActive(false);
            }
        }
    }

    // ========================================================================
    // 动态信息刷新 (每帧执行)
    // ========================================================================
    private void RefreshDynamicInfo()
    {
        bool isPowered = CheckPowerStatus();

        if (_entityManager.HasComponent<ProductionComponent>(_currentEntity))
        {
            var prod = _entityManager.GetComponentData<ProductionComponent>(_currentEntity);

            // --- 刷新 Info_3: 储量 ---
            int currentTotal = 0;
            if (_entityManager.HasBuffer<ProductionOutputElement>(_currentEntity))
            {
                var outputs = _entityManager.GetBuffer<ProductionOutputElement>(_currentEntity);
                for (int i = 0; i < outputs.Length; i++) currentTotal += outputs[i].CurrentStorage;
            }

            string colorTag = currentTotal >= prod.MaxReserves ? "<color=#FF0000>" : "<color=#FFFFFF>";
            string storageText = $"{colorTag}{currentTotal}</color> / {prod.MaxReserves}";

            if (_infoBlocks.Count > 2)
            {
                _infoBlocks[2].Text.text = storageText;
            }

            UpdateBuildStateText(prod, isPowered, currentTotal);
        }
    }

    private bool CheckPowerStatus()
    {
        if (_entityManager.HasComponent<ElectricityComponent>(_currentEntity))
        {
            var elec = _entityManager.GetComponentData<ElectricityComponent>(_currentEntity);
            string statusColor = elec.IsPowered ? "#00FF00" : "#FF0000";
            string statusText = elec.IsPowered ? "供电正常" : "电力中断";
            if (m_tmp_PowerInfo != null)
                m_tmp_PowerInfo.text = $"能耗: {elec.PowerConsumption} ";
            return elec.IsPowered;
        }
        else
        {
            if (m_tmp_PowerInfo != null) m_tmp_PowerInfo.text = "无需电力";
            return true;
        }
    }

    private void UpdateBuildStateText(ProductionComponent prod, bool isPowered, int currentStorage)
    {
        if (m_tmp_BuildState == null) return;

        if (!prod.IsActive)
        {
            SetStateText("暂停生产", Color.gray);
        }
        else if (!isPowered)
        {
            SetStateText("等待电力", Color.red);
        }
        else if (currentStorage >= prod.MaxReserves)
        {
            SetStateText("仓库已满", Color.yellow);
        }
        else if (CheckIsMissingIngredients())
        {
            SetStateText("缺少原料", Color.red);
        }
        else
        {
            float progress = math.clamp(prod.Timer / prod.ProductionInterval, 0f, 1f);
            SetStateText($"生产中 {(progress * 100):F0}%", Color.green);
        }
    }

    private bool CheckIsMissingIngredients()
    {
        if (!_entityManager.HasBuffer<ProductionInputElement>(_currentEntity)) return false;
        var inputs = _entityManager.GetBuffer<ProductionInputElement>(_currentEntity);
        for (int i = 0; i < inputs.Length; i++)
        {
            if (!GlobalInventoryManager.Instance.HasItem(inputs[i].ItemId, inputs[i].Count)) return true;
        }
        return false;
    }

    private void SetStateText(string content, Color color)
    {
        m_tmp_BuildState.text = content;
        m_tmp_BuildState.color = color;
    }
}