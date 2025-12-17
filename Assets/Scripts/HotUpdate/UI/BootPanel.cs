using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;
using GameFramework.Managers;
using GameFramework.UI; // 引用 UIBindAttribute

public class BootPanel : UIPanel
{
    // === 自动绑定 ===
    [UIBind] private Slider ProgressBar;      // 进度条 Slider
    [UIBind] private TextMeshProUGUI ProgressText; // 进度文字
    [UIBind] private Button StartBtn;         // 开始按钮
    [UIBind] private GameObject ContentRoot;  // 可选：用于控制整体显示

    private UniTaskCompletionSource _startClickTcs;

    protected override void OnInit()
    {
        base.OnInit();
        // 初始隐藏按钮
        StartBtn.gameObject.SetActive(false);
        StartBtn.onClick.AddListener(() => {
            _startClickTcs?.TrySetResult(); // 点击时完成 Task
        });
    }

    // 更新进度条
    public void UpdateProgress(float progress)
    {
        if (ProgressBar != null) ProgressBar.value = progress;
        if (ProgressText != null) ProgressText.text = $"{(int)(progress * 100)}%";
    }

    // 显示开始按钮并等待点击
    public async UniTask WaitForStartClickAsync()
    {
        StartBtn.gameObject.SetActive(true);
        _startClickTcs = new UniTaskCompletionSource();
        await _startClickTcs.Task;
    }
}