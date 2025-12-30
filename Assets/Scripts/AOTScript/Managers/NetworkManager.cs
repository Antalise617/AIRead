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

        // [配置] 登录服固定地址 (仅用于账号验证和获取列表)
        private const string LOGIN_SERVER_IP = "192.168.10.116";
        private const int LOGIN_SERVER_PORT = 8009;

        // [状态] 当前实际连接的目标 IP 和 端口 (会随选服改变)
        private string _currentIp = LOGIN_SERVER_IP;
        private int _currentPort = LOGIN_SERVER_PORT;

        private string _accessToken = "";

        // 缓存的服务器列表
        public List<ServerDTO> CachedServerList { get; private set; } = new List<ServerDTO>();
        // 当前游戏数据
        public GamesDTO CurrentGameData { get; private set; }

        public bool IsConnected => true;

        // ===================================================================================
        // 2. 事件定义
        // ===================================================================================
        public event Action OnLoginSuccess;
        public event Action<GamesDTO> OnGameDataReceived;

        // ===================================================================================
        // 3. 公开业务接口
        // ===================================================================================

        public void Initialize()
        {
            Debug.Log("[NetworkManager] 初始化，自动开始登录流程...");
            // 自动登录
            SendLogin("1930616510", "lzhlzh617");
        }

        /// <summary>
        /// [核心修改] 切换服务器接口
        /// 在UI面板选择服务器时调用，改变后续请求的目标IP
        /// </summary>
        public void SwitchServer(ServerDTO targetServer)
        {
            if (targetServer == null)
            {
                Debug.LogError("[NetworkManager] 试图切换到一个空的服务器配置！");
                return;
            }

            // 1. 更新当前目标IP
            _currentIp = targetServer.ip;

            // 2. 更新端口 (注意DTO中可能是字符串，需要转换)
            if (int.TryParse(targetServer.port, out int port))
            {
                _currentPort = port;
            }
            else
            {
                Debug.LogError($"[NetworkManager] 服务器端口解析失败: {targetServer.port}，将使用默认端口 80");
                _currentPort = 80;
            }

            Debug.Log($"[NetworkManager] ===> 已切换至服务器: {targetServer.name} ({_currentIp}:{_currentPort}) <===");
        }

        /// <summary>
        /// 1. 专用登录接口：始终连接到 LOGIN_SERVER_IP
        /// </summary>
        public async void SendLogin(string username, string password)
        {
            // [重置] 每次登录前，强制将网络目标重置回登录服
            _currentIp = LOGIN_SERVER_IP;
            _currentPort = LOGIN_SERVER_PORT;

            var dto = new UserLoginDTO { username = username, password = password };
            string jsonBody = JsonUtility.ToJson(dto);

            Debug.Log($"[NetworkManager] 发送登录请求至 {_currentIp}:{_currentPort}...");

            var response = await PostAsync<UserLoginResultDTO>("/user/login", jsonBody, false);

            if (response != null && response.status == "success")
            {
                _accessToken = response.result.access_token;
                CachedServerList = response.result.server_list;
                Debug.Log($"[NetworkManager] 登录成功，获取到 {CachedServerList.Count} 个服务器");

                // [调试] 打印详细列表
                if (CachedServerList != null)
                {
                    Debug.Log("---------- [可用服务器列表] ----------");
                    foreach (var server in CachedServerList)
                    {
                        string statusStr = server.state == 1 ? "良好" : (server.state == 2 ? "拥堵" : "爆满");
                        Debug.Log($"[ID:{server.server_id}] {server.name} | IP: {server.ip} | Port: {server.port} | 状态: {statusStr}");
                    }
                    Debug.Log("------------------------------------");
                }

                // 触发登录成功事件，UI层监听到后会刷新列表
                OnLoginSuccess?.Invoke();
            }
            else
            {
                Debug.LogError($"[NetworkManager] 登录失败: {response?.message}");
            }
        }

        /// <summary>
        /// 2. 通用游戏请求接口：发送到当前选定的 _currentIp
        /// </summary>
        public async void SendGameRequest(string url, object dto)
        {
            // 安全检查：防止未选服直接进入游戏
            if (_currentIp == LOGIN_SERVER_IP && url.Contains("joinGame"))
            {
                Debug.LogWarning("[NetworkManager] 警告：你正在向 [登录服] 发送 JoinGame 请求！请检查 ServerSelectPanel 是否正确调用了 SwitchServer。");
            }

            string jsonBody = JsonUtility.ToJson(dto);
            Debug.Log($"[NetworkManager] 发送业务请求: {url} -> {_currentIp}:{_currentPort}");

            var response = await PostAsync<GamesDTO>(url, jsonBody, true);

            if (response != null && response.status == "success")
            {
                Debug.Log($"[NetworkManager] 请求成功 ({url})，更新游戏数据");
                CurrentGameData = response.result;
                OnGameDataReceived?.Invoke(response.result);
            }
            else
            {
                Debug.LogError($"[NetworkManager] 请求失败 ({url}): {response?.message}");
            }
        }

        // ===================================================================================
        // 4. 底层网络实现
        // ===================================================================================
        private async UniTask<ServerResponse<T>> PostAsync<T>(string url, string jsonBody, bool needAuth)
        {
            using (TcpClient client = new TcpClient())
            {
                try
                {
                    // [核心] 使用动态变量连接
                    await client.ConnectAsync(_currentIp, _currentPort);

                    using (NetworkStream stream = client.GetStream())
                    {
                        // 构造 HTTP 头
                        StringBuilder sb = new StringBuilder();
                        sb.Append($"POST {url} HTTP/1.1\r\n");
                        // Host 头也动态修改
                        sb.Append($"Host: {_currentIp}:{_currentPort}\r\n");
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

                                // =========================================================
                                // [新增] 打印每一次请求返回的原始数据 (包含 Header 和 Body)
                                // =========================================================
                                Debug.Log($"[NetworkManager] 收到响应 [{url}]:\n{rawResponse}");

                                return ParseServerResponse<T>(rawResponse);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"[NetworkManager] 网络异常 [{_currentIp}:{_currentPort}]: {e.Message}");
                }
            }
            return null;
        }

        private ServerResponse<T> ParseServerResponse<T>(string rawMsg)
        {
            try
            {
                string jsonBody = rawMsg;
                int headerEnd = rawMsg.IndexOf("\r\n\r\n");
                if (headerEnd != -1) jsonBody = rawMsg.Substring(headerEnd + 4);
                jsonBody = jsonBody.Trim();

                if (string.IsNullOrEmpty(jsonBody)) return null;

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
    // DTO 定义
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
        public string ip;   // 游戏服IP
        public string port; // 游戏服端口
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

        // [原有字段]
        public int state;

        // [新增字段] 配合后端数据结构
        public long start_time;  // 开始时间
        public long end_time;    // 结束时间
        public long create_time; // 创建时间
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
        public int rotate;
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