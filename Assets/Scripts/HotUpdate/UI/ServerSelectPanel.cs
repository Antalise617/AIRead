using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SuperScrollView;
using System.Collections.Generic;
using GameFramework.Managers;
using GameFramework.UI;
using Cysharp.Threading.Tasks;
using GameFramework.Events;

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

        protected override void OnInit()
        {
            base.OnInit();
            m_btn_StartGameButton.onClick.RemoveAllListeners();
            m_btn_StartGameButton.onClick.AddListener(OnStartGameClick);

            m_btn_SelectServerButton.onClick.AddListener(() => m_obj_SelectServerPanel.SetActive(true));
            m_btn_CloseButton.onClick.AddListener(() => m_obj_SelectServerPanel.SetActive(false));

            m_obj_SelectServerPanel.SetActive(false);
            m_txt_ServerName.text = "正在获取服务器...";
        }

        protected override void OnShow()
        {
            base.OnShow();

            // 1. 主动拉取：获取列表
            RefreshServerList();

            // 2. 主动拉取：获取当前已选中的服务器（如果有）
            // 因为 NetworkManager 可能在登录成功时就已经选好了默认服
            if (NetworkManager.Instance != null && NetworkManager.Instance.CurrentSelectedServer != null)
            {
                UpdateUI(NetworkManager.Instance.CurrentSelectedServer);
            }
            else
            {
                m_txt_ServerName.text = "请选择服务器";
            }
        }

        private void OnEnable()
        {
            if (NetworkManager.Instance != null)
            {
                EventManager.Instance.Subscribe<LoginSuccessEvent>(OnLoginSuccess);
                EventManager.Instance.Subscribe<GameDataReceivedEvent>(OnJoinGameSuccess);
                EventManager.Instance.Subscribe<ServerSelectedEvent>(OnServerSelected);
            }
        }

        private void OnDisable()
        {
            if (EventManager.Instance != null)
            {
                EventManager.Instance.Unsubscribe<LoginSuccessEvent>(OnLoginSuccess);
                EventManager.Instance.Unsubscribe<GameDataReceivedEvent>(OnJoinGameSuccess);
                EventManager.Instance.Unsubscribe<ServerSelectedEvent>(OnServerSelected);
            }
        }

        // --- 事件回调 ---

        private async void OnLoginSuccess(LoginSuccessEvent evt)
        {
            await UniTask.SwitchToMainThread();
            // 登录成功只意味着列表数据准备好了，UI 刷新列表即可
            RefreshServerList();

            // 注意：不再需要在这里 SelectDefaultServer，
            // 因为 NetworkManager 会在登录成功的同时设置好 DefaultServer 并触发 ServerSelectedEvent。
            // 如果 NetworkManager 先触发事件，UI 打开时 OnShow 会通过主动拉取(Step 2)同步状态。
            // 如果 UI 是开着的，OnServerSelected 会负责更新。
        }

        private void OnJoinGameSuccess(GameDataReceivedEvent evt)
        {
            Hide();
        }

        private void OnServerSelected(ServerSelectedEvent evt)
        {
            // 被动响应：Manager 通知我们服务器变了
            UpdateUI(evt.ServerInfo);
        }

        // --- 核心逻辑 ---

        private void UpdateUI(ServerDTO server)
        {
            if (server == null) return;
            if (m_txt_ServerName) m_txt_ServerName.text = server.name;
            if (m_obj_SelectServerPanel) m_obj_SelectServerPanel.SetActive(false);
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

        private void OnStartGameClick()
        {
            NetworkManager.Instance.JoinGame();
        }

        private LoopGridViewItem OnGetItemByRowColumn(LoopGridView gridView, int itemIndex, int row, int column)
        {
            if (itemIndex < 0 || itemIndex >= _serverList.Count) return null;
            LoopGridViewItem item = gridView.NewListViewItem("ServerItem");
            ServerItem itemScript = item.GetComponent<ServerItem>();
            if (itemScript == null) itemScript = item.gameObject.AddComponent<ServerItem>();

            // 点击列表项 -> 此时 UI 没有任何业务逻辑，只负责转发指令给 Manager
            itemScript.Init(_serverList[itemIndex], (server) => {
                NetworkManager.Instance.SwitchServer(server);
            });
            return item;
        }
    }
}