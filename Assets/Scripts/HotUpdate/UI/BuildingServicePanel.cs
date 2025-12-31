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
using Cysharp.Threading.Tasks;

public class BuildingServicePanel : UIPanel
{
    // --- 顶部基础信息 ---
    [UIBind] private TextMeshProUGUI m_tmp_BuildState;
    [UIBind] private TextMeshProUGUI m_tmp_BuildingName;
    [UIBind] private Image m_img_BuildingIcon;
    [UIBind] private TextMeshProUGUI m_tmp_IslandBonusInfo;
    [UIBind] private TextMeshProUGUI m_tmp_EmployeeBonusInfo;
    [UIBind] private TextMeshProUGUI m_tmp_PowerInfo;
    [UIBind] private Button m_btn_CloseButton;

    // --- 核心信息区域 (Info_1 ~ Info_6) ---
    [UIBind] private GameObject m_obj_Info;

    // --- 【新增】服务专属信息 ---
    [UIBind] private TextMeshProUGUI m_tmp_VisitorInfo; // 显示: "当前游客: 5/20"

    // 内部类：信息块封装
    private class InfoBlock
    {
        public GameObject Root;
        public Image Icon;
        public TextMeshProUGUI Name;
        public TextMeshProUGUI Text;
        private string _currentIconPath;

        public void SetActive(bool active) => Root.SetActive(active);

        public void SetContent(string name, string value, string iconPath = null)
        {
            if (Name != null) Name.text = name;
            if (Text != null) Text.text = value;
            SetActive(true);

            if (!string.IsNullOrEmpty(iconPath) && Icon != null && _currentIconPath != iconPath)
            {
                _currentIconPath = iconPath;
                ResourceManager.Instance.LoadAssetAsync<Sprite>(iconPath)
                    .ContinueWith(s => { if (Icon != null) Icon.sprite = s; }).Forget();
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

        // 初始化信息块 Info_1 ~ Info_6
        if (m_obj_Info != null)
        {
            for (int i = 1; i <= 6; i++)
            {
                Transform tf = m_obj_Info.transform.Find($"Info_{i}");
                if (tf != null)
                {
                    _infoBlocks.Add(new InfoBlock
                    {
                        Root = tf.gameObject,
                        Icon = tf.Find("m_img_InfoIcon")?.GetComponent<Image>(),
                        Name = tf.Find("m_tmp_InfoName")?.GetComponent<TextMeshProUGUI>(),
                        Text = tf.Find("m_tmp_InfoText")?.GetComponent<TextMeshProUGUI>()
                    });
                }
                else
                {
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
    // 静态信息刷新 (打开时一次)
    // ========================================================================
    private void RefreshStaticInfo()
    {
        if (_entityManager.HasComponent<BuildingComponent>(_currentEntity))
        {
            var buildingComp = _entityManager.GetComponentData<BuildingComponent>(_currentEntity);
            m_tmp_BuildingName.text = buildingComp.Name.ToString();
        }

        foreach (var block in _infoBlocks) block.SetActive(false);

        // Info_1: 岗位
        if (_entityManager.HasComponent<ProductionComponent>(_currentEntity))
        {
            var prod = _entityManager.GetComponentData<ProductionComponent>(_currentEntity);
            _infoBlocks[0].SetContent("岗位", $"0 / {prod.JobSlots}", null);
        }
        else
        {
            _infoBlocks[0].SetContent("岗位", "-", null);
        }

        // Info_2: 繁荣度
        if (_entityManager.HasComponent<ProsperityComponent>(_currentEntity))
        {
            int val = _entityManager.GetComponentData<ProsperityComponent>(_currentEntity).Value;
            _infoBlocks[1].SetContent("繁荣度", val.ToString(), null);
        }

        // Info_3: 储量 (显示商品库存)
        _infoBlocks[2].SetContent("商品储量", "0 / 0", null);

        // Info_4 ~ 6: 产出物品 (即该店售卖的商品)
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

        for (int i = 0; i < 3; i++)
        {
            int blockIndex = 3 + i;
            if (blockIndex >= _infoBlocks.Count) break;

            if (i < count)
            {
                var itemData = outputs[i];
                var itemCfg = GlobalInventoryManager.Instance.GetConfig(itemData.ItemId);
                string itemName = itemCfg != null ? itemCfg.Name : "商品";
                string iconKey = itemData.ItemId.ToString();

                // 显示: "生产耗时: 10s"
                string infoText = $"{itemData.CountPerCycle}个 / {prod.ProductionInterval}s";
                _infoBlocks[blockIndex].SetContent(itemName, infoText, iconKey);
            }
            else
            {
                _infoBlocks[blockIndex].SetActive(false);
            }
        }
    }

    // ========================================================================
    // 动态信息刷新 (每帧)
    // ========================================================================
    private void RefreshDynamicInfo()
    {
        bool isPowered = CheckPowerStatus();

        // 1. 刷新游客信息 (服务专属)
        UpdateVisitorInfo();

        // 2. 刷新生产/服务状态
        if (_entityManager.HasComponent<ProductionComponent>(_currentEntity))
        {
            var prod = _entityManager.GetComponentData<ProductionComponent>(_currentEntity);

            // 计算当前商品库存总和
            int currentTotal = 0;
            if (_entityManager.HasBuffer<ProductionOutputElement>(_currentEntity))
            {
                var outputs = _entityManager.GetBuffer<ProductionOutputElement>(_currentEntity);
                for (int i = 0; i < outputs.Length; i++) currentTotal += outputs[i].CurrentStorage;
            }

            // 更新 Info_3 (储量)
            string colorTag = currentTotal >= prod.MaxReserves ? "<color=#FFFFFF>" : "<color=#FFFFFF>";
            string storageText = $"{colorTag}{currentTotal}</color> / {prod.MaxReserves}";
            if (_infoBlocks.Count > 2) _infoBlocks[2].Text.text = storageText;

            // 更新顶部状态文本 (优先级: 缺电 > 正在服务 > 满仓/缺料/生产)
            UpdateStateText(prod, isPowered, currentTotal);
        }
    }

    private void UpdateVisitorInfo()
    {
        if (m_tmp_VisitorInfo == null) return;

        if (_entityManager.HasComponent<ServiceComponent>(_currentEntity))
        {
            var srv = _entityManager.GetComponentData<ServiceComponent>(_currentEntity);

            int currentCount = 0;
            if (_entityManager.HasBuffer<ServiceQueueElement>(_currentEntity))
            {
                var queue = _entityManager.GetBuffer<ServiceQueueElement>(_currentEntity);
                currentCount = queue.Length;
            }

            // 显示格式: 当前游客: 3 / 10
            string color = currentCount >= srv.MaxVisitorCapacity ? "#FF0000" : "#FFFFFF";
            m_tmp_VisitorInfo.text = $"当前游客: <color={color}>{currentCount}</color> / {srv.MaxVisitorCapacity}";
        }
        else
        {
            m_tmp_VisitorInfo.text = "非营业中";
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

        if (m_tmp_PowerInfo != null) m_tmp_PowerInfo.text = "无需电力";
        return true;
    }

    private void UpdateStateText(ProductionComponent prod, bool isPowered, int currentStorage)
    {
        if (m_tmp_BuildState == null) return;

        // 1. 检查电力
        if (!isPowered)
        {
            SetStateText("等待电力", Color.red);
            return;
        }

        // 2. 检查服务状态 (正在接待优先显示)
        bool isServing = false;
        if (_entityManager.HasComponent<ServiceComponent>(_currentEntity))
        {
            var srv = _entityManager.GetComponentData<ServiceComponent>(_currentEntity);
            if (srv.IsServing)
            {
                SetStateText($"正在服务... ({srv.ServiceTimer:F1}s)", Color.green);
                isServing = true;
            }
        }
        if (isServing) return;

        // 3. 检查生产状态 (后厂)
        if (!prod.IsActive)
        {
            SetStateText("暂停营业", Color.gray);
        }
        else if (currentStorage >= prod.MaxReserves)
        {
            // 仓库满了，但没游客来消费
            SetStateText("商品充足", Color.yellow);
        }
        else if (CheckIsMissingIngredients())
        {
            SetStateText("缺少原料", Color.red);
        }
        else
        {
            // 正常备货中
            float progress = math.clamp(prod.Timer / prod.ProductionInterval, 0f, 1f);
            SetStateText($"备货中 {(progress * 100):F0}%", Color.cyan); // 区别于工厂的绿色，用青色表示备货
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