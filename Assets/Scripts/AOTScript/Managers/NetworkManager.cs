using System;
using System.Collections.Generic;
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

        private TcpClient _tcpClient;
        private NetworkStream _stream;
        private byte[] _receiveBuffer = new byte[8192];
        private bool _isConnected = false;

        // 鉴权令牌
        private string _accessToken = "";

        // 缓存的服务器列表
        public List<ServerDTO> CachedServerList { get; private set; } = new List<ServerDTO>();

        // 消息处理缓冲区（解决粘包问题）
        private StringBuilder _messageBuffer = new StringBuilder();

        // 主线程回调队列
        private readonly Queue<Action> _mainThreadActions = new Queue<Action>();

        public bool IsConnected => _isConnected;

        // ===================================================================================
        // 2. 事件定义
        // ===================================================================================
        public event Action OnLoginSuccess;
        public event Action<GamesDTO> OnJoinGameSuccess;

        // ===================================================================================
        // 3. 生命周期与连接
        // ===================================================================================
        public void Initialize()
        {
            Debug.Log("[NetworkManager] 初始化中...");
            ConnectToServer();
        }

        private void Update()
        {
            // 在主线程处理网络回调
            lock (_mainThreadActions)
            {
                while (_mainThreadActions.Count > 0)
                {
                    _mainThreadActions.Dequeue()?.Invoke();
                }
            }
        }

        private void ConnectToServer()
        {
            if (_isConnected) return;
            Close();

            try
            {
                _tcpClient = new TcpClient();
                _tcpClient.BeginConnect(SERVER_IP, SERVER_PORT, OnConnectCallback, _tcpClient);
            }
            catch (Exception e)
            {
                Debug.LogError($"[NetworkManager] 连接失败: {e.Message}");
            }
        }

        private void OnConnectCallback(IAsyncResult ar)
        {
            try
            {
                TcpClient client = (TcpClient)ar.AsyncState;
                if (client == null) return;
                client.EndConnect(ar);

                if (client.Connected)
                {
                    _isConnected = true;
                    _stream = client.GetStream();
                    Debug.Log("[NetworkManager] 连接成功，自动发起登录...");

                    // 连接成功后自动发送登录请求
                    SendLoginRequest("test_user", "123456");

                    _stream.BeginRead(_receiveBuffer, 0, _receiveBuffer.Length, OnReceiveCallback, null);
                }
            }
            catch (Exception e)
            {
                _isConnected = false;
                Debug.LogError($"[NetworkManager] 连接回调异常: {e.Message}");
            }
        }

        private void OnReceiveCallback(IAsyncResult ar)
        {
            try
            {
                if (!_isConnected || _stream == null) return;
                int bytesRead = _stream.EndRead(ar);
                if (bytesRead <= 0) { Close(); return; }

                string rawMsg = Encoding.UTF8.GetString(_receiveBuffer, 0, bytesRead);

                lock (_mainThreadActions)
                {
                    _mainThreadActions.Enqueue(() => ProcessMessage(rawMsg));
                }

                _stream.BeginRead(_receiveBuffer, 0, _receiveBuffer.Length, OnReceiveCallback, null);
            }
            catch (Exception e)
            {
                Close();
                Debug.LogError($"[NetworkManager] 接收异常: {e.Message}");
            }
        }

        // ===================================================================================
        // 4. 消息解析逻辑
        // ===================================================================================
        private void ProcessMessage(string newPart)
        {
            _messageBuffer.Append(newPart);
            string fullData = _messageBuffer.ToString();

            // 简易 HTTP 拆包：查找 Header 和 Body 的分隔符
            int bodyIndex = fullData.IndexOf("\r\n\r\n");

            if (bodyIndex != -1)
            {
                string jsonBody = fullData.Substring(bodyIndex + 4);

                // 简易完整性检查（实际项目应解析 Content-Length）
                if (jsonBody.TrimEnd().EndsWith("}"))
                {
                    _messageBuffer.Clear();
                    Debug.Log($"[NetworkManager] 收到数据包 >>> {jsonBody}");
                    HandleJsonLogic(jsonBody);
                }
            }
        }

        private void HandleJsonLogic(string jsonBody)
        {
            // 策略：通过检测 JSON 字段特征来判断消息类型

            // A. 处理登录响应 (包含 access_token 和 server_list)
            if (jsonBody.Contains("\"access_token\"") && jsonBody.Contains("\"server_list\""))
            {
                try
                {
                    var response = JsonUtility.FromJson<ServerResponse<UserLoginResultDTO>>(jsonBody);
                    if (response != null && response.status == "success")
                    {
                        _accessToken = response.result.access_token;
                        CachedServerList = response.result.server_list;

                        Debug.Log($"[NetworkManager] 登录成功! Token: {_accessToken.Substring(0, 8)}... 获取到 {CachedServerList.Count} 个服务器");
                        OnLoginSuccess?.Invoke();

                        // [测试] 自动打开选服界面
                        if (GameFramework.Managers.UIManager.Instance != null)
                        {
                            GameFramework.Managers.UIManager.Instance.ShowPanelAsync<GameFramework.Managers.UIPanel>("ServerSelectPanel", GameFramework.Managers.UILayer.Normal).Forget();
                        }
                    }
                }
                catch (Exception ex) { Debug.LogError($"Login解析失败: {ex.Message}"); }
            }
            // B. 处理加入游戏响应 (包含 Player 数据)
            else if (jsonBody.Contains("\"Player\"") || (jsonBody.Contains("\"Building\"") && jsonBody.Contains("\"Tile\"")))
            {
                try
                {
                    var response = JsonUtility.FromJson<ServerResponse<GamesDTO>>(jsonBody);
                    if (response != null && response.status == "success")
                    {
                        Debug.Log($"<color=green>[NetworkManager] 加入游戏成功!</color> 玩家: {response.result.Player?.name}");
                        OnJoinGameSuccess?.Invoke(response.result);
                    }
                    else
                    {
                        Debug.LogError($"[NetworkManager] 加入游戏失败: {response?.message}");
                    }
                }
                catch (Exception ex) { Debug.LogError($"JoinGame解析失败: {ex.Message}"); }
            }
            // C. 错误处理
            else if (jsonBody.Contains("\"error\""))
            {
                Debug.LogWarning($"[NetworkManager] 服务器返回错误: {jsonBody}");
            }
        }

        // ===================================================================================
        // 5. 请求发送逻辑
        // ===================================================================================

        /// <summary>
        /// 发送登录请求 (/user/login)
        /// </summary>
        public void SendLoginRequest(string username, string password)
        {
            var dto = new UserLoginDTO { username = username, password = password };
            string jsonBody = JsonUtility.ToJson(dto);

            Debug.Log($"[NetworkManager] 发送登录请求: {jsonBody}");
            SendPostRequest("/user/login", jsonBody, false); // 登录不需要 Token
        }

        /// <summary>
        /// 发送加入游戏请求 (/player/joinGame)
        /// </summary>
        public void SendJoinGameRequest(int serverId)
        {
            var dto = new JoinGameDTO { server_id = serverId };
            string jsonBody = JsonUtility.ToJson(dto);

            Debug.Log($"[NetworkManager] 发送选服请求: {jsonBody}");
            SendPostRequest("/player/joinGame", jsonBody, true); // 需要 Token
        }

        /// <summary>
        /// 底层 POST 构造器
        /// </summary>
        private void SendPostRequest(string url, string jsonBody, bool needAuth)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"POST {url} HTTP/1.1\r\n");
            sb.Append($"Host: {SERVER_IP}:{SERVER_PORT}\r\n");
            sb.Append("Content-Type: application/json\r\n");

            if (needAuth && !string.IsNullOrEmpty(_accessToken))
            {
                sb.Append($"Authorization: Bearer {_accessToken}\r\n");
            }

            sb.Append($"Content-Length: {Encoding.UTF8.GetByteCount(jsonBody)}\r\n");
            sb.Append("Connection: Keep-Alive\r\n\r\n");
            sb.Append(jsonBody);

            Send(Encoding.UTF8.GetBytes(sb.ToString()));
        }

        public void Send(byte[] data)
        {
            if (!_isConnected || _stream == null) return;
            try { _stream.Write(data, 0, data.Length); }
            catch { Close(); }
        }

        public void Close()
        {
            _isConnected = false;
            _accessToken = "";
            if (_stream != null) { _stream.Close(); _stream = null; }
            if (_tcpClient != null) { _tcpClient.Close(); _tcpClient = null; }
            _messageBuffer.Clear();
        }

        private void OnDestroy() { Close(); }
    }

    // ===================================================================================
    // 6. 数据结构定义 (基于 Swagger.json)
    // ===================================================================================

    // 通用响应包装 (status, message, result)
    [Serializable]
    public class ServerResponse<T>
    {
        public string status;       // "success"
        public string message;
        public string error;
        public T result;            // 具体业务数据
    }

    // ---------------- REQUEST DTOs ----------------

    [Serializable]
    public class UserLoginDTO
    {
        public string username;
        public string password;
    }

    [Serializable]
    public class JoinGameDTO
    {
        public int server_id;
    }

    // ---------------- RESPONSE DTOs ----------------

    // 登录返回数据
    [Serializable]
    public class UserLoginResultDTO
    {
        public string access_token;
        public int expires_in;
        public List<ServerDTO> server_list;
    }

    // 服务器信息 (对应 Server)
    [Serializable]
    public class ServerDTO
    {
        public int server_id;
        public string name;
        public string ip;
        public string port;     // Swagger定义为 string
        public int is_open;     // 1开放 0不开放
        public int state;       // 1良好 2拥堵 3爆满
        public long create_time;
    }

    // 游戏全量数据 (对应 gamesDTO)
    [Serializable]
    public class GamesDTO
    {
        public PlayerDTO Player;
        public List<TileDTO> Tile;
        public List<BuildingDTO> Building;
        public List<ItemDTO> Item;
        // public List<QuestDTO> Quest; // 根据需要取消注释
        // public BuildingUnlockDTO BuildingUnlock;
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
        public int level;
        public int tile_index;
        public int is_fixed;
        public int posX;
        public int posY;
        public int posZ;
        public int state;
        // 其他字段按需添加...
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
        // 其他字段按需添加...
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