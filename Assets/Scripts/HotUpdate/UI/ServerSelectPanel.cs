using UnityEngine;
using SuperScrollView;
using System.Collections.Generic;
using GameFramework.Managers;
using GameFramework.UI;

namespace HotUpdate.UI
{
    public class ServerSelectPanel : UIPanel
    {
        [Header("SuperScrollView")]
        [UIBind] public LoopGridView m_obj_Root;

        // [修改] 使用 ServerDTO 类型
        private List<ServerDTO> _serverList = null;

        private void Start()
        {
            RefreshServerList();
        }

        public void RefreshServerList()
        {
            if (NetworkManager.Instance != null)
            {
                _serverList = NetworkManager.Instance.CachedServerList;
            }

            if (_serverList == null) _serverList = new List<ServerDTO>();

            // 重置列表
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

        LoopGridViewItem OnGetItemByRowColumn(LoopGridView gridView, int itemIndex, int row, int column)
        {
            if (itemIndex < 0 || itemIndex >= _serverList.Count)
            {
                return null;
            }

            // [修改] 获取数据
            ServerDTO itemData = _serverList[itemIndex];

            // 注意：Inspector 面板上的 ItemName 必须是 "ServerItemPrefab"
            LoopGridViewItem item = gridView.NewListViewItem("ServerItem");

            if (item == null)
            {
                return null;
            }

            ServerItem itemScript = item.GetComponent<ServerItem>();

            if (itemScript == null)
            {
                itemScript = item.gameObject.AddComponent<ServerItem>();
            }

            itemScript.Init(itemData, OnServerSelected);

            return item;
        }

        // [修改] 参数类型改为 ServerDTO
        private void OnServerSelected(ServerDTO node)
        {
            Debug.Log($"[ServerSelectPanel] 玩家选择了服务器: [{node.server_id}] {node.name}，发起请求...");

            // 1. 关闭面板
            Hide();

            // 2. 发送加入游戏请求
            if (NetworkManager.Instance != null)
            {
                NetworkManager.Instance.SendJoinGameRequest(node.server_id);
            }
            else
            {
                Debug.LogError("NetworkManager 实例为空！");
            }
        }
    }
}