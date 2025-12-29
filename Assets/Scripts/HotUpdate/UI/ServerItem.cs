using UnityEngine;
using UnityEngine.UI;
using System;
using GameFramework.Managers;
using TMPro;

namespace HotUpdate.UI
{
    public class ServerItem : MonoBehaviour
    {
        [Header("UI Components")]
        public TextMeshProUGUI ButtonNameText;  // 用来显示服务器名称的文本组件
        public Button ClickButton;   // 点击按钮

        private ServerDTO _data;
        private Action<ServerDTO> _onClickCallback;

        public void Init(ServerDTO data, Action<ServerDTO> onClick)
        {
            _data = data;
            _onClickCallback = onClick;

            // 只需要显示服务器名称 (保留ID前缀以便区分)
            if (ButtonNameText != null)
            {
                ButtonNameText.text = $"{data.server_id}. {data.name}";
            }

            // 绑定点击事件
            if (ClickButton != null)
            {
                ClickButton.onClick.RemoveAllListeners();
                ClickButton.onClick.AddListener(OnItemClicked);
            }
        }

        private void OnItemClicked()
        {
            _onClickCallback?.Invoke(_data);
        }
    }
}