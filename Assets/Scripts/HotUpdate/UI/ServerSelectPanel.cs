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
                NetworkManager.Instance.OnLoginSuccess += OnLoginSuccess;
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

        private async void OnLoginSuccess()
        {
            await UniTask.SwitchToMainThread();
            Debug.Log("[ServerSelectPanel] 登录成功，更新服务器列表 UI");
            RefreshServerList();
            SelectDefaultServer();
        }

        private void OnJoinGameSuccess(GamesDTO data)
        {
            Debug.Log("[ServerSelectPanel] 加入游戏成功，关闭选服界面");
            Hide();
        }

        private void OnStartGameClick()
        {
            if (_currentSelectedServer == null) return;

            Debug.Log($"[ServerSelectPanel] 点击开始，请求进入服务器: ID {_currentSelectedServer.server_id}");

            // 发送 JoinGame 请求
            // 此时 NetworkManager 内部已经通过 SwitchServer 切换到了目标 IP
            var joinReq = new JoinGameDTO { server_id = _currentSelectedServer.server_id };
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

        private void SelectDefaultServer()
        {
            if (_serverList != null && _serverList.Count > 0) UpdateUI(_serverList[0]);
        }

        private void UpdateUI(ServerDTO server)
        {
            _currentSelectedServer = server;
            if (m_txt_ServerName) m_txt_ServerName.text = server.name;

            // =========================================================
            // [关键修改] 选中服务器时，通知 NetworkManager 切换底层 IP/端口
            // =========================================================
            if (NetworkManager.Instance != null)
            {
                NetworkManager.Instance.SwitchServer(server);
            }
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