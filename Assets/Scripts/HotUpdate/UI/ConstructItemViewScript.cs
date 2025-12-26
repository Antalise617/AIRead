using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using GameFramework.Managers; // 引用 ResourceManager 所在的命名空间
using Cysharp.Threading.Tasks; // 引用 UniTask

public class ConstructItemViewScript : MonoBehaviour
{
    [Header("Basic Info")]
    public TextMeshProUGUI NameText;
    public Button ItemButton;

    [Header("Cost Display Group 1")]
    public GameObject CostGroup1; // 整个消耗组的父节点 (用于隐藏/显示)
    public Image CostIcon1;
    public TextMeshProUGUI CostText1;

    [Header("Cost Display Group 2")]
    public GameObject CostGroup2;
    public Image CostIcon2;
    public TextMeshProUGUI CostText2;

    [Header("Cost Display Group 3")]
    public GameObject CostGroup3;
    public Image CostIcon3;
    public TextMeshProUGUI CostText3;

    // 内部结构体方便遍历处理
    private struct CostUIElement
    {
        public GameObject Group;
        public Image Icon;
        public TextMeshProUGUI Amount;
    }

    private CostUIElement[] _costElements;

    private void Awake()
    {
        // 初始化数组以便后续遍历设置
        _costElements = new CostUIElement[]
        {
            new CostUIElement { Group = CostGroup1, Icon = CostIcon1, Amount = CostText1 },
            new CostUIElement { Group = CostGroup2, Icon = CostIcon2, Amount = CostText2 },
            new CostUIElement { Group = CostGroup3, Icon = CostIcon3, Amount = CostText3 }
        };
    }

    /// <summary>
    /// 设置数据并绑定点击
    /// </summary>
    /// <param name="id">物品/建筑 ID</param>
    /// <param name="name">显示名称</param>
    /// <param name="costs">消耗列表 (List<List<int>> 结构: [[itemId, count], [itemId, count]])，可为 null</param>
    /// <param name="onClick">点击回调</param>
    public void SetData(int id, string name, List<List<int>> costs, Action onClick)
    {
        // 1. 设置名称
        if (NameText != null)
        {
            NameText.text = name;
        }

        // 2. 绑定点击事件
        if (ItemButton != null)
        {
            ItemButton.onClick.RemoveAllListeners();
            ItemButton.onClick.AddListener(() => onClick?.Invoke());
        }

        // 3. 设置消耗显示
        UpdateCostDisplay(costs);
    }

    private void UpdateCostDisplay(List<List<int>> costs)
    {
        // 如果没有消耗数据，隐藏所有组
        if (costs == null || costs.Count == 0)
        {
            foreach (var element in _costElements)
            {
                if (element.Group != null) element.Group.SetActive(false);
            }
            return;
        }

        // 遍历 UI 槽位
        for (int i = 0; i < _costElements.Length; i++)
        {
            var ui = _costElements[i];
            if (ui.Group == null) continue;

            // 如果当前索引小于消耗数据的数量，则显示并设置
            if (i < costs.Count)
            {
                var costData = costs[i];
                // Luban 数据结构约定：costData[0] = itemId, costData[1] = count
                if (costData != null && costData.Count >= 2)
                {
                    int itemId = costData[0];
                    int amount = costData[1];

                    ui.Group.SetActive(true);

                    // 设置数量文本
                    if (ui.Amount != null)
                    {
                        ui.Amount.text = amount.ToString();
                    }

                    // 异步加载并设置图标
                    if (ui.Icon != null)
                    {
                        LoadIconAsync(itemId, ui.Icon).Forget();
                    }
                }
                else
                {
                    ui.Group.SetActive(false); // 数据异常则隐藏
                }
            }
            else
            {
                // 超出数据范围的 UI 隐藏
                ui.Group.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 异步加载物品图标
    /// 假设 Addressables Key 就是物品 ID (例如 "1001", "2001") 或者是 "Icon_1001" 这种格式
    /// 请根据您实际的 Addressables 命名规则修改 key 的生成方式
    /// </summary>
    private async UniTaskVoid LoadIconAsync(int itemId, Image targetImage)
    {
        // 假设图标的 Addressable Key 是 "ItemIcon_{ID}" 或者直接是 ID 字符串
        // 这里暂时假设 Key 就是 ID 的字符串形式，或者是 "Icon_ID"
        // 建议统一一种命名规范，例如 "Icon_1001"
        string addressableKey = itemId.ToString();
        // string addressableKey = $"Icon_{itemId}"; // 备选方案

        try
        {
            // 调用 ResourceManager 加载 Sprite
            // 注意：ResourceManager.LoadAssetAsync<T> 默认返回 Object，需要确保它是 Sprite 类型
            Sprite iconSprite = await ResourceManager.Instance.LoadAssetAsync<Sprite>(addressableKey);

            if (targetImage != null && iconSprite != null)
            {
                targetImage.sprite = iconSprite;
            }
        }
        catch (Exception e)
        {
            // 可以在这里设置一个默认图标，防止加载失败留空
            Debug.LogWarning($"[ConstructItem] Failed to load icon for item {itemId}: {e.Message}");
        }
    }
}