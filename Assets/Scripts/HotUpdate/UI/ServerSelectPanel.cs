using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SuperScrollView;
using System.Collections.Generic;
using GameFramework.Managers;
using GameFramework.UI;
using Cysharp.Threading.Tasks;

namespace HotUpdate.UI
{
    public class ServerSelectPanel : UIPanel
    {
        [Header("--- Main Controls ---")]
        [UIBind] public Button m_btn_StartGameButton;
        [UIBind] public Button m_btn_SelectServerButton;
        [UIBind] public TextMeshProUGUI m_txt_ServerName;
        [UIBind] public GameObject m_obj_SelectServerPanel;
        [UIBind] public LoopGridView m_obj_Root;
        [UIBind] public Button m_btn_CloseButton;

        private List<ServerDTO> _serverList = null;
        private ServerDTO _currentSelectedServer = null;

        protected override void OnInit()
        {
            base.OnInit();
            m_btn_StartGameButton.onClick.RemoveAllListeners();
            m_btn_StartGameButton.onClick.AddListener(OnStartGameClick);

            m_btn_SelectServerButton.onClick.AddListener(() => m_obj_SelectServerPanel.SetActive(true));
            m_btn_CloseButton.onClick.AddListener(() => m_obj_SelectServerPanel.SetActive(false));

            m_obj_SelectServerPanel.SetActive(false);
            if (m_txt_ServerName != null) m_txt_ServerName.text = "请选择服务器";
        }

        protected override void OnShow()
        {
            base.OnShow();
            // 如果已经有缓存列表，直接刷新
            RefreshServerList();
        }

        private void OnEnable()
        {
            if (NetworkManager.Instance != null)
            {
                // 1. 监听登录成功 -> 刷新列表
                NetworkManager.Instance.OnLoginSuccess += OnLoginSuccess;
                // 2. 监听游戏数据 -> 进入游戏
                NetworkManager.Instance.OnGameDataReceived += OnJoinGameSuccess;
            }
        }

        private void OnDisable()
        {
            if (NetworkManager.Instance != null)
            {
                NetworkManager.Instance.OnLoginSuccess -= OnLoginSuccess;
                NetworkManager.Instance.OnGameDataReceived -= OnJoinGameSuccess;
            }
        }

        // 处理登录成功：只负责刷新列表
        private async void OnLoginSuccess()
        {
            await UniTask.SwitchToMainThread();
            Debug.Log("[ServerSelectPanel] 登录成功，更新服务器列表 UI");
            RefreshServerList();
            SelectDefaultServer();
        }

        // 处理进入游戏成功：关闭面板，逻辑转交 GameWorldLoader
        private void OnJoinGameSuccess(GamesDTO data)
        {
            Debug.Log("[ServerSelectPanel] 加入游戏成功，关闭选服界面");
            Hide();
        }

        // 点击开始游戏：调用专用的 SendGameRequest
        private void OnStartGameClick()
        {
            if (_currentSelectedServer == null) return;

            Debug.Log($"[ServerSelectPanel] 点击开始，请求进入服务器: ID {_currentSelectedServer.server_id}");

            var joinReq = new JoinGameDTO { server_id = _currentSelectedServer.server_id };

            // 使用通用的 SendGameRequest
            NetworkManager.Instance.SendGameRequest("/player/joinGame", joinReq);
        }

        public void RefreshServerList()
        {
            if (NetworkManager.Instance == null) return;
            _serverList = NetworkManager.Instance.CachedServerList;

            if (_serverList != null && _serverList.Count > 0 && m_obj_Root != null)
            {
                if (m_obj_Root.ItemTotalCount == 0) m_obj_Root.InitGridView(_serverList.Count, OnGetItemByRowColumn);
                else { m_obj_Root.SetListItemCount(_serverList.Count); m_obj_Root.RefreshAllShownItem(); }
            }
        }

        // ... (省略 SelectDefaultServer 和 OnGetItemByRowColumn 的实现，保持原样即可) ...
        private void SelectDefaultServer()
        {
            if (_serverList != null && _serverList.Count > 0) UpdateUI(_serverList[0]);
        }

        private void UpdateUI(ServerDTO server)
        {
            _currentSelectedServer = server;
            if (m_txt_ServerName) m_txt_ServerName.text = server.name;
        }

        private LoopGridViewItem OnGetItemByRowColumn(LoopGridView gridView, int itemIndex, int row, int column)
        {
            if (itemIndex < 0 || itemIndex >= _serverList.Count) return null;
            LoopGridViewItem item = gridView.NewListViewItem("ServerItem");
            ServerItem itemScript = item.GetComponent<ServerItem>();
            if (itemScript == null) itemScript = item.gameObject.AddComponent<ServerItem>();

            itemScript.Init(_serverList[itemIndex], (s) => {
                UpdateUI(s);
                m_obj_SelectServerPanel.SetActive(false);
            });
            return item;
        }
    }
}