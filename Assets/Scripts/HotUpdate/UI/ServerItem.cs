using UnityEngine;
using UnityEngine.UI;
using System;
using GameFramework.Managers;

namespace HotUpdate.UI
{
    public class ServerItem : MonoBehaviour
    {
        [Header("UI Components")]
        public Text NameText;
        public Text StatusText;
        public Text IpText;
        public Button ClickButton;
        public Image StatusIcon;

        private ServerDTO _data;
        private Action<ServerDTO> _onClickCallback;

        public void Init(ServerDTO data, Action<ServerDTO> onClick)
        {
            _data = data;
            _onClickCallback = onClick;

            // 设置服务器名称
            if (NameText != null) NameText.text = $"{data.server_id}. {data.name}";

            // 设置IP端口 (port 现在是 string)
            if (IpText != null) IpText.text = $"{data.ip}:{data.port}";

            // 设置状态显示
            if (StatusText != null)
            {
                // 默认状态
                string statusStr = "未知";
                Color statusColor = Color.gray;

                // Api.ts: is_open 1开放 0不开放
                if (data.is_open == 0)
                {
                    statusStr = "维护中";
                    statusColor = Color.gray;
                }
                else
                {
                    // Api.ts: state 1良好 2拥堵 3爆满
                    switch (data.state)
                    {
                        case 1: statusStr = "良好"; statusColor = Color.green; break;
                        case 2:
                            statusStr = "拥堵"; statusColor = new Color(1f, 0.5f, 0f); // 橙色
                            break;
                        case 3: statusStr = "爆满"; statusColor = Color.red; break;
                        default: statusStr = "流畅"; statusColor = Color.green; break;
                    }
                }

                StatusText.text = statusStr;
                StatusText.color = statusColor;

                // 同步图标颜色
                if (StatusIcon != null) StatusIcon.color = statusColor;
            }

            // 绑定点击
            ClickButton.onClick.RemoveAllListeners();
            ClickButton.onClick.AddListener(OnItemClicked);
        }

        private void OnItemClicked()
        {
            _onClickCallback?.Invoke(_data);
        }
    }
}