using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SuperScrollView;
using System.Collections.Generic;
using GameFramework.Managers;
using GameFramework.UI;
using GameFramework.Core;

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
            RefreshServerList();
            SelectDefaultServer();
        }

        private void OnEnable()
        {
            if (NetworkManager.Instance != null)
                NetworkManager.Instance.OnJoinGameSuccess += OnJoinGameSuccess;
        }

        private void OnDisable()
        {
            if (NetworkManager.Instance != null)
                NetworkManager.Instance.OnJoinGameSuccess -= OnJoinGameSuccess;
        }

        private void OnJoinGameSuccess(GamesDTO data)
        {
            Debug.Log("[ServerSelectPanel] 收到进入成功事件，正在关闭面板...");
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

        // ... (其他 List 相关代码与之前一致，保持不变) ...

        public void RefreshServerList()
        {
            if (NetworkManager.Instance != null) _serverList = NetworkManager.Instance.CachedServerList;
            if (_serverList == null) _serverList = new List<ServerDTO>();
            if (m_obj_Root.ItemTotalCount == 0) m_obj_Root.InitGridView(_serverList.Count, OnGetItemByRowColumn);
            else { m_obj_Root.SetListItemCount(_serverList.Count); m_obj_Root.RefreshAllShownItem(); }
        }

        private void SelectDefaultServer()
        {
            if (_serverList != null && _serverList.Count > 0) UpdateSelectedServerUI(_serverList[0]);
            else { m_txt_ServerName.text = "暂无服务器"; m_btn_StartGameButton.interactable = false; }
        }

        private void UpdateSelectedServerUI(ServerDTO server)
        {
            _currentSelectedServer = server;
            m_btn_StartGameButton.interactable = true;
            if (m_txt_ServerName != null) m_txt_ServerName.text = server.name;
            if (m_img_ServerState != null) m_img_ServerState.color = server.state == 1 ? Color.green : (server.state == 3 ? Color.red : Color.yellow);
        }

        private void OnServerItemClicked(ServerDTO node)
        {
            UpdateSelectedServerUI(node);
            m_obj_SelectServerPanel.SetActive(false);
        }

        private void OnOpenServerListClick() { m_obj_SelectServerPanel.SetActive(true); m_obj_Root.RefreshAllShownItem(); }
        private void OnCloseServerListClick() { m_obj_SelectServerPanel.SetActive(false); }

        LoopGridViewItem OnGetItemByRowColumn(LoopGridView gridView, int itemIndex, int row, int column)
        {
            if (itemIndex < 0 || itemIndex >= _serverList.Count) return null;
            LoopGridViewItem item = gridView.NewListViewItem("ServerItem");
            ServerItem itemScript = item.GetComponent<ServerItem>();
            if (itemScript == null) itemScript = item.gameObject.AddComponent<ServerItem>();
            itemScript.Init(_serverList[itemIndex], OnServerItemClicked);
            return item;
        }
    }
}