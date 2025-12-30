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

        private const string LOGIN_SERVER_IP = "192.168.10.116";
        private const int LOGIN_SERVER_PORT = 8009;

        private string _currentIp = LOGIN_SERVER_IP;
        private int _currentPort = LOGIN_SERVER_PORT;

        private string _accessToken = "";

        public List<ServerDTO> CachedServerList { get; private set; } = new List<ServerDTO>();
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
            SendLogin("1930616512", "lzhlzh617");
        }

        public void SwitchServer(ServerDTO targetServer)
        {
            if (targetServer == null)
            {
                Debug.LogError("[NetworkManager] 试图切换到一个空的服务器配置！");
                return;
            }

            _currentIp = targetServer.ip;

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

        public async void SendLogin(string username, string password)
        {
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
                OnLoginSuccess?.Invoke();
            }
            else
            {
                Debug.LogError($"[NetworkManager] 登录失败: {response?.message}");
            }
        }

        public async void SendGameRequest(string url, object dto)
        {
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

                // [核心修改] 检查并更新 Token
                CheckAndRefreshToken(response.result);

                CurrentGameData = response.result;
                OnGameDataReceived?.Invoke(response.result);
            }
            else
            {
                Debug.LogError($"[NetworkManager] 请求失败 ({url}): {response?.message}");
            }
        }

        // ===================================================================================
        // [新增] 泛型异步接口，供业务系统直接 await 使用 (PlacementSystem)
        // ===================================================================================
        public async UniTask<T> SendAsync<T>(string url, object requestDto)
        {
            if (_currentIp == LOGIN_SERVER_IP && url.Contains("create"))
            {
                Debug.LogWarning("[NetworkManager] 警告：正在向登录服发送创建请求！请检查是否已选服。");
            }

            string jsonBody = JsonUtility.ToJson(requestDto);
            Debug.Log($"[NetworkManager] SendAsync请求: {url} | Body: {jsonBody}");

            var response = await PostAsync<T>(url, jsonBody, true);

            if (response != null && response.status == "success")
            {
                // [核心修改] 如果返回类型是 GamesDTO，尝试更新 Token
                if (response.result is GamesDTO gamesDto)
                {
                    CheckAndRefreshToken(gamesDto);
                }
                return response.result;
            }
            else
            {
                Debug.LogError($"[NetworkManager] SendAsync失败 {url}: {response?.message}");
                return default;
            }
        }

        // 辅助方法：提取 Token
        private void CheckAndRefreshToken(GamesDTO data)
        {
            if (data != null && data.tokenResult != null && !string.IsNullOrEmpty(data.tokenResult.access_token))
            {
                _accessToken = data.tokenResult.access_token;
                Debug.Log($"[NetworkManager] Token 已自动刷新: {_accessToken.Substring(0, 10)}...");
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
                    await client.ConnectAsync(_currentIp, _currentPort);

                    using (NetworkStream stream = client.GetStream())
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append($"POST {url} HTTP/1.1\r\n");
                        sb.Append($"Host: {_currentIp}:{_currentPort}\r\n");
                        sb.Append("Content-Type: application/json\r\n");
                        if (needAuth && !string.IsNullOrEmpty(_accessToken))
                        {
                            sb.Append($"Authorization: Bearer {_accessToken}\r\n");
                        }
                        byte[] bodyBytes = Encoding.UTF8.GetBytes(jsonBody);
                        sb.Append($"Content-Length: {bodyBytes.Length}\r\n");
                        sb.Append("Connection: close\r\n\r\n");

                        byte[] headerBytes = Encoding.UTF8.GetBytes(sb.ToString());
                        await stream.WriteAsync(headerBytes, 0, headerBytes.Length);
                        await stream.WriteAsync(bodyBytes, 0, bodyBytes.Length);

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
        public string ip;
        public string port;
        public int is_open;
        public int state;
        public long create_time;
    }

    [Serializable] public class JoinGameDTO { public int server_id; }

    // [新增] Token结果DTO
    [Serializable]
    public class TokenResultDTO
    {
        public string access_token;
        public int expires_in;
    }

    [Serializable]
    public class GamesDTO
    {
        // [新增] 对应 Swagger 中的 tokenResult
        public TokenResultDTO tokenResult;

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

        // [新增] 必须添加这个字段，否则无法读取服务器返回的类型信息
        public int tile_type;

        public int tile_index;
        public int is_fixed;
        public int level;
        public int posX;
        public int posY;
        public int posZ;
        public int state;
        public long start_time;
        public long end_time;
        public long create_time;
        public int videoCount;
        public int continueState;
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
        public long startProduceTime;
        public long end_time;
        public int videoCount;
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

    [Serializable]
    public class BuildingCreateDTO
    {
        public int building_id;
        public int posX;
        public int posY;
        public int posZ;
        public int rotate;
    }

    [Serializable]
    public class TileCreateDTO
    {
        // [修正] Swagger定义为 tile_type，原代码为 tile_id
        public int tile_type;
        public int posX;
        public int posY;
        public int posZ;
    }
}