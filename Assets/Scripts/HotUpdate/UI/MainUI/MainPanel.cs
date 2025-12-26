using GameFramework.Managers;
using GameFramework.UI;
using GameFramework.Core;
using GameFramework.Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks; // 引用 UniTask
using GameFramework.HotUpdate.UI; // 引用 ConstructPanel 所在的命名空间

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

    // 活动按钮部分
    [UIBind] private Button m_btn_ActivityGift;
    [UIBind] private Button m_btn_7DayCheckIn;

    // 功能按钮
    [UIBind] private Button m_btn_PlacementSystem;

    protected override void OnInit()
    {
        base.OnInit();
        if (m_btn_PlacementSystem != null)
        {
            m_btn_PlacementSystem.onClick.AddListener(OnPlacementSystemClicked);
        }
        else
        {
            Debug.LogError("[MainPanel] m_btn_PlacementSystem 未绑定。");
        }
        RefreshAllResources();
    }

    /// <summary>
    /// 点击建造按钮回调
    /// </summary>
    private async void OnPlacementSystemClicked()
    {
        Debug.Log("[MainPanel] 打开建造面板");
        await UIManager.Instance.ShowPanelAsync<ConstructPanel>("ConstructPanel");
    }

    protected void OnDestroy()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.Unsubscribe<ResourceChangedEvent>(OnResourceChanged);
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
    /// 事件回调：当资源发生变化时自动调用
    /// </summary>
    private void OnResourceChanged(ResourceChangedEvent evt)
    {
        switch (evt.Type)
        {
            case ResourceType.Wood:
                if (m_tmp_WoodNumText != null)
                    m_tmp_WoodNumText.text = evt.NewValue.ToString();
                break;

            case ResourceType.Stone:
                if (m_tmp_RockNumText != null)
                    m_tmp_RockNumText.text = evt.NewValue.ToString();
                break;

            case ResourceType.Gold:
                if (m_tmp_GoldCoinNumText != null)
                    m_tmp_GoldCoinNumText.text = evt.NewValue.ToString();
                break;
        }
    }

    /// <summary>
    /// 主动刷新所有资源显示
    /// </summary>
    private void RefreshAllResources()
    {
        var resMgr = GameResourceManager.Instance;
        if (resMgr == null)
        {
            Debug.Log("GameResourceManager未初始化");
            return;
        }

        int wood = resMgr.GetResource(ResourceType.Wood);
        int stone = resMgr.GetResource(ResourceType.Stone);
        int gold = resMgr.GetResource(ResourceType.Gold);
        int oil = 0; // 假设石油逻辑还没接

        SetResourcesNum(wood, stone, oil, gold);
    }

    private void SetResourcesNum(int wood, int rock, int oil, int goldCoin)
    {
        if (m_tmp_WoodNumText != null)
            m_tmp_WoodNumText.text = wood.ToString();

        if (m_tmp_RockNumText != null)
            m_tmp_RockNumText.text = rock.ToString();

        if (m_tmp_OilNumText != null)
            m_tmp_OilNumText.text = oil.ToString();

        if (m_tmp_GoldCoinNumText != null)
            m_tmp_GoldCoinNumText.text = goldCoin.ToString();
    }
}