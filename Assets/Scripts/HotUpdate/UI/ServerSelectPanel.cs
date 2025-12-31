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

        // [新增] 定义默认要选中的服务器索引 (0 = 列表第1个)
        // 你可以随时修改这个值，或者从本地配置(PlayerPrefs)读取上次选中的服
        private const int DEFAULT_SERVER_INDEX = 2;

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

            // [修改] 调用新方法，传入想要自动选中的索引
            SelectServerByIndex(DEFAULT_SERVER_INDEX);
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

            // [保持修复] 使用 RequestData 封装参数
            var requestData = new RequestData();
            requestData.AddField("server_id", _currentSelectedServer.server_id);

            // 发送请求
            NetworkManager.Instance.SendGameRequest("/player/joinGame", requestData);
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

        // [新增] 根据索引选中服务器
        private void SelectServerByIndex(int index)
        {
            if (_serverList == null || _serverList.Count == 0) return;

            // 1. 越界检查：如果配置的索引超出了列表范围，强制选第 1 个 (index 0)
            if (index < 0 || index >= _serverList.Count)
            {
                Debug.LogWarning($"[ServerSelectPanel] 目标索引 {index} 超出范围 (0-{_serverList.Count - 1})，自动重置为 0");
                index = 0;
            }

            // 2. 执行选中逻辑
            UpdateUI(_serverList[index]);

            Debug.Log($"[ServerSelectPanel] 自动选中服务器: {_serverList[index].name} (Index: {index})");
        }

        private void UpdateUI(ServerDTO server)
        {
            _currentSelectedServer = server;

            // [核心逻辑] 更新 UI 显示文本
            if (m_txt_ServerName) m_txt_ServerName.text = server.name;

            // 选中服务器时，通知 NetworkManager 切换底层 IP/端口
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