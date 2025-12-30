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
        // 1. 配置与状态
        // ===================================================================================
        private const string SERVER_IP = "192.168.10.116";
        private const int SERVER_PORT = 8009;

        private string _accessToken = "";
        public List<ServerDTO> CachedServerList { get; private set; } = new List<ServerDTO>();
        public GamesDTO CurrentGameData { get; private set; }

        public bool IsConnected => true;

        // ===================================================================================
        // 2. 事件定义 (分离登录和游戏数据事件)
        // ===================================================================================
        public event Action OnLoginSuccess;             // 登录成功，UI应刷新服务器列表
        public event Action<GamesDTO> OnGameDataReceived; // 收到游戏数据(JoinGame, Sync等)，UI应进入游戏

        // ===================================================================================
        // 3. 公开业务接口 (明确区分)
        // ===================================================================================

        public void Initialize()
        {
            Debug.Log("[NetworkManager] 初始化，自动开始登录流程...");
            // 自动登录
            SendLogin("test_user", "123456");
        }

        /// <summary>
        /// 1. 专用登录接口：返回 UserLoginResultDTO (含服务器列表)
        /// </summary>
        public async void SendLogin(string username, string password)
        {
            var dto = new UserLoginDTO { username = username, password = password };
            string jsonBody = JsonUtility.ToJson(dto);

            Debug.Log($"[NetworkManager] 发送登录请求...");

            // 发送请求，指定返回类型为 UserLoginResultDTO
            var response = await PostAsync<UserLoginResultDTO>("/user/login", jsonBody, false);

            if (response != null && response.status == "success")
            {
                _accessToken = response.result.access_token;
                CachedServerList = response.result.server_list;
                Debug.Log($"[NetworkManager] 登录成功，获取到 {CachedServerList.Count} 个服务器");

                // 触发登录成功事件
                OnLoginSuccess?.Invoke();
            }
            else
            {
                Debug.LogError($"[NetworkManager] 登录失败: {response?.message}");
            }
        }

        /// <summary>
        /// 2. 通用游戏请求接口：返回 GamesDTO (如 JoinGame, createBuilding 等)
        /// </summary>
        /// <param name="url">接口地址，如 /player/joinGame</param>
        /// <param name="dto">请求参数对象</param>
        public async void SendGameRequest(string url, object dto)
        {
            string jsonBody = JsonUtility.ToJson(dto);
            Debug.Log($"[NetworkManager] 发送业务请求: {url}");

            // 发送请求，指定返回类型为 GamesDTO
            var response = await PostAsync<GamesDTO>(url, jsonBody, true);

            if (response != null && response.status == "success")
            {
                Debug.Log($"[NetworkManager] 请求成功 ({url})，更新游戏数据");
                CurrentGameData = response.result;

                // 触发游戏数据更新事件
                OnGameDataReceived?.Invoke(response.result);
            }
            else
            {
                Debug.LogError($"[NetworkManager] 请求失败 ({url}): {response?.message}");
            }
        }

        // ===================================================================================
        // 4. 底层网络实现 (泛型化，处理粘包和反序列化)
        // ===================================================================================
        private async UniTask<ServerResponse<T>> PostAsync<T>(string url, string jsonBody, bool needAuth)
        {
            using (TcpClient client = new TcpClient())
            {
                try
                {
                    await client.ConnectAsync(SERVER_IP, SERVER_PORT);
                    using (NetworkStream stream = client.GetStream())
                    {
                        // 构造 HTTP 头
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
                        sb.Append("Connection: close\r\n\r\n");

                        // 发送
                        byte[] headerBytes = Encoding.UTF8.GetBytes(sb.ToString());
                        await stream.WriteAsync(headerBytes, 0, headerBytes.Length);
                        await stream.WriteAsync(bodyBytes, 0, bodyBytes.Length);

                        // 接收
                        using (MemoryStream ms = new MemoryStream())
                        {
                            byte[] buffer = new byte[8192];
                            int bytesRead;
                            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                            {
                                ms.Write(buffer, 0, bytesRead);
                            }

                            byte[] totalData = ms.ToArray();
                            if (totalData.Length > 0)
                            {
                                string rawResponse = Encoding.UTF8.GetString(totalData);
                                return ParseServerResponse<T>(rawResponse);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"[NetworkManager] 网络异常: {e.Message}");
                }
            }
            return null;
        }

        private ServerResponse<T> ParseServerResponse<T>(string rawMsg)
        {
            try
            {
                // 分离 HTTP 头
                string jsonBody = rawMsg;
                int headerEnd = rawMsg.IndexOf("\r\n\r\n");
                if (headerEnd != -1) jsonBody = rawMsg.Substring(headerEnd + 4);
                jsonBody = jsonBody.Trim();

                if (string.IsNullOrEmpty(jsonBody)) return null;

                // 反序列化泛型结构
                return JsonUtility.FromJson<ServerResponse<T>>(jsonBody);
            }
            catch (Exception e)
            {
                Debug.LogError($"[NetworkManager] JSON解析错误: {e.Message}");
                return null;
            }
        }
    }

    // ===================================================================================
    // DTO 定义 (保持)
    // ===================================================================================
    [Serializable]
    public class ServerResponse<T>
    {
        public string status;
        public string message;
        public string error;
        public T result;
    }

    [Serializable] public class UserLoginDTO { public string username; public string password; }
    [Serializable] public class UserLoginResultDTO { public string access_token; public int expires_in; public List<ServerDTO> server_list; }

    [Serializable]
    public class ServerDTO
    {
        public int server_id;
        public string name;
        public string ip;
        public string port;
        public int is_open;
        public int state;
        public long create_time;
    }

    [Serializable] public class JoinGameDTO { public int server_id; }

    [Serializable]
    public class GamesDTO
    {
        public PlayerDTO Player;
        public List<TileDTO> Tile;
        public List<BuildingDTO> Building;
        public List<ItemDTO> Item;
        // 其他字段按需添加...
    }

    [Serializable]
    public class PlayerDTO
    {
        public string _id;
        public string user_id;
        public int server_id;
        public string name;
        public string player_icon;
        public int thriving;
        public long engry_time;
    }

    [Serializable]
    public class TileDTO
    {
        public string _id;
        public int tile_id;
        public int tile_type;
        public int level;
        public int is_fixed;
        public int posX;
        public int posY;
        public int posZ;
        public int state;
        // ... 其他字段
    }

    [Serializable]
    public class BuildingDTO
    {
        public string _id;
        public int building_id;
        public int BuildType;
        public int level;
        public int posX;
        public int posY;
        public int posZ;
        public int rotate; // [关键修复] 之前缺失的字段
        public int state;
        public long start_time;
        public long end_time;
        public int PowerState;
    }

    [Serializable]
    public class ItemDTO
    {
        public string _id;
        public string player_id;
        public int item_id;
        public int count;
        public int used;
    }
}