using GameFramework.Core;
using GameFramework.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // 如果使用 TextMeshPro，请保留；否则改为 Text

public class IslandInfoPanel : UIPanel
{
    [Header("Basic Info")]
    [SerializeField] private TextMeshProUGUI islandNameText;
    [SerializeField] private TextMeshProUGUI islandBonusText;

    [Header("Grid Settings")]
    [SerializeField] private Transform gridParent; // 9x9图片的父节点
    [SerializeField] private Sprite spriteOutOfBounds; // 图片1：不在岛屿范围
    [SerializeField] private Sprite spriteEmpty;       // 图片2：岛屿范围但未占用
    [SerializeField] private Sprite spriteOccupied;    // 图片3：被建筑占用

    private Dictionary<string, Image> gridImages = new Dictionary<string, Image>();
    private const int MaxGridSize = 9;

    private void Awake()
    {
        InitGridReferences();
    }

    /// <summary>
    /// 初始化网格引用，通过名字 "(x,y)" 快速定位 Image
    /// </summary>
    private void InitGridReferences()
    {
        if (gridParent == null) return;

        foreach (Transform child in gridParent)
        {
            Image img = child.GetComponent<Image>();
            if (img != null)
            {
                // 假设名字格式严格为 (x,y)
                gridImages[child.name] = img;
            }
        }
    }
}