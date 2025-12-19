using UnityEngine;

namespace GameFramework.Core
{
    // 定义一个接口供 AOT 系统操作 UI
    public interface IPlacementConfirmPanel
    {
        void Show();
        void Hide();
        void UpdatePosition(Vector3 worldPosition);

        // 用于判断 UI 是否显示
        bool IsVisible { get; }
    }
}