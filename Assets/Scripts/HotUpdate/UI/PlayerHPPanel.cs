using GameFramework.Managers;
using GameFramework.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHPPanel : UIPanel
{
    [UIBind] private Slider HPBar;
    [UIBind] private TextMeshProUGUI NameText;

    private Transform _targetTransform;
    private Camera _mainCam;
    private Vector3 _offset = new Vector3(0, 2.2f, 0); // 血条在头顶的高度

    protected override void OnInit()
    {
        base.OnInit();
        _mainCam = Camera.main;
    }

    public void Setup(Transform target, string name)
    {
        _targetTransform = target;
        if (NameText != null) NameText.text = name;
        if (HPBar != null) HPBar.value = 1f;
    }

    private void LateUpdate()
    {
        if (_targetTransform == null)
        {
            // 如果目标消失（例如死亡），可以隐藏自己或销毁
            if (gameObject.activeSelf) Hide();
            return;
        }

        if (_mainCam == null) _mainCam = Camera.main;

        // ★★★ 核心逻辑：世界坐标 -> 屏幕坐标 ★★★
        Vector3 worldPos = _targetTransform.position + _offset;
        Vector3 screenPos = _mainCam.WorldToScreenPoint(worldPos);

        // 处理相机背后的情况 (防止血条出现在屏幕反面)
        if (screenPos.z < 0)
        {
            canvasGroup.alpha = 0;
        }
        else
        {
            canvasGroup.alpha = 1;
            transform.position = screenPos;
        }
    }
}