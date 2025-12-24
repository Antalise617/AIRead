using GameFramework.Managers;
using GameFramework.UI;
using GameFramework.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using GameFramework.HotUpdate.UI;
using Game.HotUpdate;

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

    // 【修改】在 UI 内部定义需要监听和显示的资源 ID (与配表 Item_道具表 对应)
    private const int RES_ID_GOLD = 2;      // 金币
    private const int RES_ID_WOOD = 20001;  // 木材
    private const int RES_ID_STONE = 20002; // 石材
    private const int RES_ID_OIL = 50001;   // 原油

    protected override void OnInit()
    {
        base.OnInit();

        // 绑定按钮事件
        if (m_btn_PlacementSystem != null)
        {
            m_btn_PlacementSystem.onClick.AddListener(OnPlacementSystemClicked);
        }

        // 监听库存变化事件
        GlobalInventoryManager.Instance.OnItemChanged += OnInventoryItemChanged;

        // 初始化显示
        RefreshAllResources();
    }

    protected void OnDestroy()
    {
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
        RefreshAllResources();
    }

    /// <summary>
    /// 库存发生变化时的回调
    /// </summary>
    private void OnInventoryItemChanged(int itemId, long change, long total)
    {
        // 【修改】使用本地常量判断
        switch (itemId)
        {
            case RES_ID_WOOD:
                if (m_tmp_WoodNumText != null) m_tmp_WoodNumText.text = total.ToString();
                break;
            case RES_ID_STONE:
                if (m_tmp_RockNumText != null) m_tmp_RockNumText.text = total.ToString();
                break;
            case RES_ID_OIL:
                if (m_tmp_OilNumText != null) m_tmp_OilNumText.text = total.ToString();
                break;
            case RES_ID_GOLD:
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

        // 【修改】使用本地常量获取数据
        if (m_tmp_WoodNumText != null)
            m_tmp_WoodNumText.text = invMgr.GetItemCount(RES_ID_WOOD).ToString();

        if (m_tmp_RockNumText != null)
            m_tmp_RockNumText.text = invMgr.GetItemCount(RES_ID_STONE).ToString();

        if (m_tmp_OilNumText != null)
            m_tmp_OilNumText.text = invMgr.GetItemCount(RES_ID_OIL).ToString();

        if (m_tmp_GoldCoinNumText != null)
            m_tmp_GoldCoinNumText.text = invMgr.GetItemCount(RES_ID_GOLD).ToString();
    }

    private async void OnPlacementSystemClicked()
    {
        Debug.Log("[MainPanel] 打开建造面板");
        await UIManager.Instance.ShowPanelAsync<ConstructPanel>("ConstructPanel");
    }
}