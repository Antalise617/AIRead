using GameFramework.Managers;
using GameFramework.UI;
using GameFramework.Core;
// using GameFramework.Events; // 不再需要通用的 EventManager
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using GameFramework.HotUpdate.UI;
using Game.HotUpdate; // 引用 GlobalInventoryManager 所在的命名空间

class MainPanel : UIPanel
{
    // 玩家信息部分
    [UIBind] private TextMeshProUGUI m_tmp_PlayerName;
    [UIBind] private Image PlayerIcon;

    // 资源信息部分
    [UIBind] private TextMeshProUGUI m_tmp_WoodNumText;
    [UIBind] private TextMeshProUGUI m_tmp_RockNumText;
    [UIBind] private TextMeshProUGUI m_tmp_OilNumText;
    [UIBind] private TextMeshProUGUI m_tmp_GoldCoinNumText;

    // 按钮部分
    [UIBind] private Button m_btn_ActivityGift;
    [UIBind] private Button m_btn_7DayCheckIn;
    [UIBind] private Button m_btn_PlacementSystem;

    protected override void OnInit()
    {
        base.OnInit();

        // 绑定按钮事件
        if (m_btn_PlacementSystem != null)
        {
            m_btn_PlacementSystem.onClick.AddListener(OnPlacementSystemClicked);
        }

        // 【关键修改】监听库存变化事件
        GlobalInventoryManager.Instance.OnItemChanged += OnInventoryItemChanged;

        // 初始化显示
        RefreshAllResources();
    }

    protected void OnDestroy()
    {
        // 【关键修改】移除事件监听，防止内存泄漏
        if (GlobalInventoryManager.Instance != null)
        {
            GlobalInventoryManager.Instance.OnItemChanged -= OnInventoryItemChanged;
        }

        if (m_btn_PlacementSystem != null)
        {
            m_btn_PlacementSystem.onClick.RemoveListener(OnPlacementSystemClicked);
        }
    }

    protected override void OnShow()
    {
        base.OnShow();
        // 每次打开面板时也刷新一次，确保数据最新
        RefreshAllResources();
    }

    /// <summary>
    /// 【新增】库存发生变化时的回调
    /// </summary>
    /// <param name="itemId">物品ID</param>
    /// <param name="change">变化量</param>
    /// <param name="total">当前总量</param>
    private void OnInventoryItemChanged(int itemId, long change, long total)
    {
        // 根据 ID 更新对应的 UI 文本
        switch (itemId)
        {
            case GlobalInventoryManager.ITEM_ID_WOOD:
                if (m_tmp_WoodNumText != null) m_tmp_WoodNumText.text = total.ToString();
                break;
            case GlobalInventoryManager.ITEM_ID_STONE:
                if (m_tmp_RockNumText != null) m_tmp_RockNumText.text = total.ToString();
                break;
            case GlobalInventoryManager.ITEM_ID_OIL:
                if (m_tmp_OilNumText != null) m_tmp_OilNumText.text = total.ToString();
                break;
            case GlobalInventoryManager.ITEM_ID_GOLD:
                if (m_tmp_GoldCoinNumText != null) m_tmp_GoldCoinNumText.text = total.ToString();
                break;
        }
    }

    /// <summary>
    /// 主动刷新所有资源显示
    /// </summary>
    private void RefreshAllResources()
    {
        var invMgr = GlobalInventoryManager.Instance;
        if (invMgr == null) return;

        // 直接从 Inventory 获取数据
        if (m_tmp_WoodNumText != null)
            m_tmp_WoodNumText.text = invMgr.GetItemCount(GlobalInventoryManager.ITEM_ID_WOOD).ToString();

        if (m_tmp_RockNumText != null)
            m_tmp_RockNumText.text = invMgr.GetItemCount(GlobalInventoryManager.ITEM_ID_STONE).ToString();

        if (m_tmp_OilNumText != null)
            m_tmp_OilNumText.text = invMgr.GetItemCount(GlobalInventoryManager.ITEM_ID_OIL).ToString();

        if (m_tmp_GoldCoinNumText != null)
            m_tmp_GoldCoinNumText.text = invMgr.GetItemCount(GlobalInventoryManager.ITEM_ID_GOLD).ToString();
    }

    private async void OnPlacementSystemClicked()
    {
        Debug.Log("[MainPanel] 打开建造面板");
        await UIManager.Instance.ShowPanelAsync<ConstructPanel>("ConstructPanel");
    }
}