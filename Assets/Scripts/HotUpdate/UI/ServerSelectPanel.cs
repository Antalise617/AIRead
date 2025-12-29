using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SuperScrollView;
using System.Collections.Generic;
using GameFramework.Managers;
using GameFramework.UI;
using GameFramework.Core;
using Cysharp.Threading.Tasks; // 必须引用，用于线程切换

namespace HotUpdate.UI
{
    public class ServerSelectPanel : UIPanel
    {
        [Header("--- Main Controls ---")]
        [UIBind] public Button m_btn_StartGameButton;

        [Header("--- Server Selector ---")]
        [UIBind] public Button m_btn_SelectServerButton;
        [UIBind] public Image m_img_ServerState;
        [UIBind] public TextMeshProUGUI m_txt_ServerName;

        [Header("--- Select Panel ---")]
        [UIBind] public GameObject m_obj_SelectServerPanel;
        [UIBind] public Button m_btn_CloseButton;
        [UIBind] public LoopGridView m_obj_Root;

        private List<ServerDTO> _serverList = null;
        private ServerDTO _currentSelectedServer = null;

        private void Start()
        {
            m_btn_StartGameButton.onClick.AddListener(OnStartGameClick);
            m_btn_SelectServerButton.onClick.AddListener(OnOpenServerListClick);
            m_btn_CloseButton.onClick.AddListener(OnCloseServerListClick);

            m_obj_SelectServerPanel.SetActive(false);

            // 尝试初始刷新（如果还没登录成功，列表为空，会显示"正在获取..."）
            RefreshServerList();
            SelectDefaultServer();
        }

        private void OnEnable()
        {
            if (NetworkManager.Instance != null)
            {
                NetworkManager.Instance.OnJoinGameSuccess += OnJoinGameSuccess;
                // [新增] 监听登录成功事件（获取服务器列表的时机）
                NetworkManager.Instance.OnLoginSuccess += OnLoginSuccess;
            }
        }

        private void OnDisable()
        {
            if (NetworkManager.Instance != null)
            {
                NetworkManager.Instance.OnJoinGameSuccess -= OnJoinGameSuccess;
                // [新增] 取消监听
                NetworkManager.Instance.OnLoginSuccess -= OnLoginSuccess;
            }
        }

        // [核心修复] 登录成功后的回调
        private async void OnLoginSuccess()
        {
            // 网络回调可能在后台线程，强制切回主线程操作 UI
            await UniTask.SwitchToMainThread();

            Debug.Log("[ServerSelectPanel] 收到登录成功通知，自动刷新服务器列表...");
            RefreshServerList();
            SelectDefaultServer();

            // 如果此时玩家正打开着选择列表，也要强制刷新列表视图
            if (m_obj_SelectServerPanel.activeSelf && m_obj_Root != null)
            {
                m_obj_Root.RefreshAllShownItem();
            }
        }

        private void OnJoinGameSuccess(GamesDTO data)
        {
            Debug.Log("[ServerSelectPanel] 进入游戏成功，关闭面板...");
            Hide();
        }

        private void OnStartGameClick()
        {
            if (_currentSelectedServer == null) return;

            Debug.Log($"[ServerSelectPanel] 点击开始游戏 (ServerID: {_currentSelectedServer.server_id})");
            m_btn_StartGameButton.interactable = false; // 防止重复点击

            if (GameStateManager.Instance != null)
                GameStateManager.Instance.ChangeState(GameState.Loading);

            if (NetworkManager.Instance != null)
                NetworkManager.Instance.SendJoinGameRequest(_currentSelectedServer.server_id);
        }

        public void RefreshServerList()
        {
            if (NetworkManager.Instance != null)
                _serverList = NetworkManager.Instance.CachedServerList;

            if (_serverList == null) _serverList = new List<ServerDTO>();

            // 只有当有数据时才去操作 LoopGridView，防止报错
            if (_serverList.Count > 0)
            {
                if (m_obj_Root.ItemTotalCount == 0)
                {
                    m_obj_Root.InitGridView(_serverList.Count, OnGetItemByRowColumn);
                }
                else
                {
                    m_obj_Root.SetListItemCount(_serverList.Count);
                    m_obj_Root.RefreshAllShownItem();
                }
            }
        }

        private void SelectDefaultServer()
        {
            if (_serverList != null && _serverList.Count > 0)
            {
                // 有数据，默认选中第一个
                UpdateSelectedServerUI(_serverList[0]);
            }
            else
            {
                // 无数据，显示加载状态
                if (m_txt_ServerName != null) m_txt_ServerName.text = "正在获取服务器...";
                m_btn_StartGameButton.interactable = false;

                // 也可以把状态灯改成灰色或隐藏
                if (m_img_ServerState != null) m_img_ServerState.color = Color.gray;
            }
        }

        private void UpdateSelectedServerUI(ServerDTO server)
        {
            _currentSelectedServer = server;
            m_btn_StartGameButton.interactable = true;

            if (m_txt_ServerName != null) m_txt_ServerName.text = server.name;

            // 1:流畅(绿) 2:拥挤(黄) 3:爆满/维护(红)
            if (m_img_ServerState != null)
            {
                if (server.state == 1) m_img_ServerState.color = Color.green;
                else if (server.state == 3) m_img_ServerState.color = Color.red;
                else m_img_ServerState.color = Color.yellow;
            }
        }

        private void OnServerItemClicked(ServerDTO node)
        {
            UpdateSelectedServerUI(node);
            m_obj_SelectServerPanel.SetActive(false);
        }

        private void OnOpenServerListClick()
        {
            m_obj_SelectServerPanel.SetActive(true);
            // 打开面板时再次刷新显示，确保数据是最新的
            if (_serverList != null && _serverList.Count > 0)
            {
                m_obj_Root.RefreshAllShownItem();
            }
        }

        private void OnCloseServerListClick()
        {
            m_obj_SelectServerPanel.SetActive(false);
        }

        LoopGridViewItem OnGetItemByRowColumn(LoopGridView gridView, int itemIndex, int row, int column)
        {
            if (itemIndex < 0 || itemIndex >= _serverList.Count) return null;

            LoopGridViewItem item = gridView.NewListViewItem("ServerItem");
            ServerItem itemScript = item.GetComponent<ServerItem>();

            if (itemScript == null)
                itemScript = item.gameObject.AddComponent<ServerItem>();

            itemScript.Init(_serverList[itemIndex], OnServerItemClicked);

            return item;
        }
    }
}