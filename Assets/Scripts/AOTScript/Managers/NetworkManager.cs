using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace GameFramework.Managers
{
    public class NetworkManager : Singleton<NetworkManager>
    {
        // ===================================================================================
        // 1. 配置
        // ===================================================================================
        private const string SERVER_IP = "192.168.10.116";
        private const int SERVER_PORT = 8009;

        // ===================================================================================
        // 2. 游戏数据缓存
        // ===================================================================================
        private string _accessToken = "";
        public List<ServerDTO> CachedServerList { get; private set; } = new List<ServerDTO>();
        public GamesDTO CurrentGameData { get; private set; }

        public bool IsConnected => true; // 短连接模式始终视为可用

        // 事件
        public event Action OnLoginSuccess;
        public event Action<GamesDTO> OnJoinGameSuccess;

        // ===================================================================================
        // 3. 初始化与请求接口
        // ===================================================================================
        public void Initialize()
        {
            Debug.Log("[NetworkManager] 初始化...");
            SendLoginRequest("test_user", "123456");
        }

        public async void SendLoginRequest(string username, string password)
        {
            var dto = new UserLoginDTO { username = username, password = password };
            await SendRequestAsync("/user/login", JsonUtility.ToJson(dto), false);
        }

        public async void SendJoinGameRequest(int serverId)
        {
            var dto = new JoinGameDTO { server_id = serverId };
            Debug.Log($"[NetworkManager] 请求进入服务器 ID: {serverId}");
            await SendRequestAsync("/player/joinGame", JsonUtility.ToJson(dto), true);
        }

        // ===================================================================================
        // 4. 核心发送逻辑 (修复：循环读取完整响应)
        // ===================================================================================
        private async UniTask SendRequestAsync(string url, string jsonBody, bool needAuth)
        {
            using (TcpClient client = new TcpClient())
            {
                try
                {
                    // 1. 建立连接
                    await client.ConnectAsync(SERVER_IP, SERVER_PORT);
                    using (NetworkStream stream = client.GetStream())
                    {
                        // 2. 构造 HTTP 请求包
                        StringBuilder sb = new StringBuilder();
                        sb.Append($"POST {url} HTTP/1.1\r\n");
                        sb.Append($"Host: {SERVER_IP}:{SERVER_PORT}\r\n");
                        sb.Append("Content-Type: application/json\r\n");
                        if (needAuth && !string.IsNullOrEmpty(_accessToken))
                        {
                            sb.Append($"Authorization: Bearer {_accessToken}\r\n");
                        }
                        byte[] bodyBytes = Encoding.UTF8.GetBytes(jsonBody);
                        sb.Append($"Content-Length: {bodyBytes.Length}\r\n");
                        sb.Append("Connection: close\r\n\r\n"); // 关键：告诉服务器发完就关

                        // 3. 发送请求头和包体
                        byte[] headerBytes = Encoding.UTF8.GetBytes(sb.ToString());
                        await stream.WriteAsync(headerBytes, 0, headerBytes.Length);
                        await stream.WriteAsync(bodyBytes, 0, bodyBytes.Length);

                        // 4. [修复] 循环接收直到数据流结束
                        using (MemoryStream ms = new MemoryStream())
                        {
                            byte[] buffer = new byte[8192];
                            int bytesRead = 0;

                            // 只要连接没断开且有数据，就一直读
                            // 或者是 Connection: close 模式下，服务器关闭流时 ReadAsync 返回 0
                            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                            {
                                ms.Write(buffer, 0, bytesRead);
                            }

                            byte[] totalData = ms.ToArray();
                            if (totalData.Length > 0)
                            {
                                string rawResponse = Encoding.UTF8.GetString(totalData);
                                // Debug.Log($"[NetworkManager] 接收完整包大小: {totalData.Length} bytes");
                                ParseResponse(rawResponse);
                            }
                            else
                            {
                                Debug.LogWarning("[NetworkManager] 服务器响应为空");
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"[NetworkManager] 网络请求异常: {e.Message}");
                }
            }
        }

        // ===================================================================================
        // 5. 响应解析
        // ===================================================================================
        private void ParseResponse(string rawMsg)
        {
            // 分离 HTTP 头和包体
            string jsonBody = rawMsg;
            int headerEnd = rawMsg.IndexOf("\r\n\r\n");
            if (headerEnd != -1)
            {
                jsonBody = rawMsg.Substring(headerEnd + 4);
            }

            jsonBody = jsonBody.Trim();
            if (string.IsNullOrEmpty(jsonBody)) return;

            // 简单的完整性校验 (应对极少数极其特殊的粘包情况，虽然短连接通常不会有)
            if (!jsonBody.EndsWith("}"))
            {
                Debug.LogError($"[NetworkManager] JSON 数据不完整，结尾: {jsonBody.Substring(Math.Max(0, jsonBody.Length - 20))}");
                return;
            }

            HandleJsonLogic(jsonBody);
        }

        private void HandleJsonLogic(string jsonBody)
        {
            try
            {
                // A. 登录成功
                if (jsonBody.Contains("\"access_token\""))
                {
                    var response = JsonUtility.FromJson<ServerResponse<UserLoginResultDTO>>(jsonBody);
                    if (response != null && response.status == "success")
                    {
                        _accessToken = response.result.access_token;
                        CachedServerList = response.result.server_list;
                        Debug.Log("[NetworkManager] 登录成功");
                        OnLoginSuccess?.Invoke();
                    }
                }
                // B. 加入游戏成功
                else if (jsonBody.Contains("\"Player\"") || jsonBody.Contains("\"Building\""))
                {
                    var response = JsonUtility.FromJson<ServerResponse<GamesDTO>>(jsonBody);
                    if (response != null && response.status == "success")
                    {
                        Debug.Log($"[NetworkManager] 加入游戏成功! 数据量: {jsonBody.Length} chars");
                        CurrentGameData = response.result;
                        OnJoinGameSuccess?.Invoke(response.result);
                    }
                    else
                    {
                        Debug.LogError($"[NetworkManager] 业务错误: {response?.message}");
                    }
                }
                // C. 错误处理
                else if (jsonBody.Contains("\"error\""))
                {
                    Debug.LogWarning($"[NetworkManager] 服务器返回错误: {jsonBody}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[NetworkManager] JSON 解析失败: {ex.Message}\nContent Preview: {jsonBody.Substring(0, Mathf.Min(jsonBody.Length, 200))}");
            }
        }
    }

    // ===================================================================================
    // DTO 定义 (保持不变)
    // ===================================================================================
    [Serializable] public class ServerResponse<T> { public string status; public string message; public string error; public T result; }
    [Serializable] public class UserLoginDTO { public string username; public string password; }
    [Serializable] public class JoinGameDTO { public int server_id; }
    [Serializable] public class UserLoginResultDTO { public string access_token; public int expires_in; public List<ServerDTO> server_list; }
    [Serializable] public class ServerDTO { public int server_id; public string name; public string ip; public string port; public int is_open; public int state; public long create_time; }

    [Serializable]
    public class GamesDTO
    {
        public PlayerDTO Player;
        public List<TileDTO> Tile;
        public List<BuildingDTO> Building;
        public List<ItemDTO> Item;
    }

    [Serializable] public class PlayerDTO { public string _id; public string user_id; public int server_id; public string name; public string player_icon; public int thriving; public long engry_time; }
    [Serializable] public class TileDTO { public string _id; public int tile_id; public int tile_type; public int level; public int is_fixed; public int posX; public int posY; public int posZ; public int state; }
    [Serializable] public class BuildingDTO { public string _id; public int building_id; public int BuildType; public int level; public int posX; public int posY; public int posZ; public int rotate; public int state; }
    [Serializable] public class ItemDTO { public string _id; public string player_id; public int item_id; public int count; public int used; }
}