using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ConstructItemViewScript : MonoBehaviour
{
    public TextMeshProUGUI NameText;
    public Button ItemButton;

    /// <summary>
    /// 设置数据并绑定点击
    /// </summary>
    public void SetData(int id, string name, Action onClick)
    {
        if (NameText != null)
        {
            NameText.text = name;
        }

        if (ItemButton != null)
        {
            // 关键：在循环列表中必须先移除旧监听，防止重复绑定
            ItemButton.onClick.RemoveAllListeners();
            ItemButton.onClick.AddListener(() => onClick?.Invoke());
        }
    }
}