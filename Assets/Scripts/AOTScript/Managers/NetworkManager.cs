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

        private string _accessToken = "";
        public List<ServerDTO> CachedServerList { get; private set; } = new List<ServerDTO>();
        public GamesDTO CurrentGameData { get; private set; } // 缓存游戏数据

        private StringBuilder _messageBuffer = new StringBuilder();
        private readonly Queue<Action> _mainThreadActions = new Queue<Action>();

        public bool IsConnected => _isConnected;

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
            lock (_mainThreadActions)
            {
                while (_mainThreadActions.Count > 0)
                {
                    try
                    {
                        _mainThreadActions.Dequeue()?.Invoke();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"[NetworkManager] 主线程回调执行异常: {ex}");
                    }
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
            catch (Exception e) { Debug.LogError($"[NetworkManager] 连接失败: {e.Message}"); }
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
                    SendLoginRequest("test_user", "123456");
                    _stream.BeginRead(_receiveBuffer, 0, _receiveBuffer.Length, OnReceiveCallback, null);
                }
            }
            catch (Exception e) { _isConnected = false; Debug.LogError($"[NetworkManager] 连接回调异常: {e.Message}"); }
        }

        private void OnReceiveCallback(IAsyncResult ar)
        {
            try
            {
                if (!_isConnected || _stream == null) return;
                int bytesRead = _stream.EndRead(ar);
                if (bytesRead <= 0)
                {
                    Close();
                    Debug.LogWarning("[NetworkManager] 服务器主动断开连接 (BytesRead <= 0)");
                    return;
                }

                string rawMsg = Encoding.UTF8.GetString(_receiveBuffer, 0, bytesRead);

                // [关键调试] 打印所有收到的原始内容，排查是否粘包或无响应
                Debug.Log($"[NetworkManager] 收到原始数据 ({bytesRead} bytes):\n{rawMsg}");

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
        // 4. 消息解析逻辑 (增强容错版)
        // ===================================================================================
        private void ProcessMessage(string newPart)
        {
            _messageBuffer.Append(newPart);
            string fullData = _messageBuffer.ToString();

            // 1. 尝试标准 HTTP 解析
            int headerEndIndex = fullData.IndexOf("\r\n\r\n");

            if (headerEndIndex != -1)
            {
                string headers = fullData.Substring(0, headerEndIndex);
                string jsonBody = fullData.Substring(headerEndIndex + 4);

                // 简单的完整性检查：检查花括号是否匹配
                if (IsJsonComplete(jsonBody))
                {
                    // 打印响应头，检查是否有 404/500 错误
                    if (!headers.Contains("200 OK")) Debug.LogWarning($"[NetworkManager] 非200响应头:\n{headers}");

                    _messageBuffer.Clear();
                    HandleJsonLogic(jsonBody);
                }
                else
                {
                    Debug.Log($"[NetworkManager] 包体不完整，等待后续数据... 当前长度: {jsonBody.Length}");
                }
            }
            else
            {
                // 2. [容错] 如果不是 HTTP 协议，或者头部分割符丢失，直接尝试解析 JSON
                // 很多简单的 TCP 服务器只发送 JSON 字符串，不带 HTTP 头
                string trimmed = fullData.Trim();
                if (trimmed.StartsWith("{") && IsJsonComplete(trimmed))
                {
                    Debug.Log("[NetworkManager] 检测到纯 JSON 格式（无 HTTP 头），尝试直接解析...");
                    _messageBuffer.Clear();
                    HandleJsonLogic(trimmed);
                }
                else if (fullData.Length > 10000)
                {
                    // 防止缓冲区无限膨胀
                    Debug.LogError("[NetworkManager] 缓冲区过大且未找到有效包，强制清理！");
                    _messageBuffer.Clear();
                }
            }
        }

        private bool IsJsonComplete(string json)
        {
            if (string.IsNullOrEmpty(json)) return false;
            string trimmed = json.TrimEnd();
            return trimmed.EndsWith("}");
        }

        private void HandleJsonLogic(string jsonBody)
        {
            Debug.Log($"[NetworkManager] 开始业务解析: {jsonBody.Substring(0, Mathf.Min(jsonBody.Length, 100))}...");

            // A. 登录响应
            if (jsonBody.Contains("\"access_token\"") && jsonBody.Contains("\"server_list\""))
            {
                try
                {
                    var response = JsonUtility.FromJson<ServerResponse<UserLoginResultDTO>>(jsonBody);
                    if (response != null && response.status == "success")
                    {
                        _accessToken = response.result.access_token;
                        CachedServerList = response.result.server_list;
                        Debug.Log($"[NetworkManager] 登录成功! Token获取成功。");
                        OnLoginSuccess?.Invoke();
                    }
                }
                catch (Exception ex) { Debug.LogError($"Login解析失败: {ex.Message}"); }
            }
            // B. 加入游戏响应 (兼容多种字段判断)
            else if (jsonBody.Contains("\"Player\"") || (jsonBody.Contains("\"Building\"") && jsonBody.Contains("\"Tile\"")))
            {
                try
                {
                    var response = JsonUtility.FromJson<ServerResponse<GamesDTO>>(jsonBody);
                    if (response != null && response.status == "success")
                    {
                        Debug.Log($"<color=green>[NetworkManager] 加入游戏成功!</color> 玩家: {response.result.Player?.name}");
                        CurrentGameData = response.result;

                        // 检查事件订阅情况
                        if (OnJoinGameSuccess == null) Debug.LogWarning("[NetworkManager] 警告：没有脚本订阅 OnJoinGameSuccess 事件！");
                        else Debug.Log("[NetworkManager] 触发 OnJoinGameSuccess 事件...");

                        OnJoinGameSuccess?.Invoke(response.result);
                    }
                    else
                    {
                        Debug.LogError($"[NetworkManager] 加入游戏业务失败: {response?.message}");
                    }
                }
                catch (Exception ex) { Debug.LogError($"JoinGame解析失败: {ex.Message}"); }
            }
            // C. 错误处理
            else if (jsonBody.Contains("\"error\""))
            {
                Debug.LogWarning($"[NetworkManager] 服务器返回明确错误: {jsonBody}");
            }
            else
            {
                Debug.LogWarning($"[NetworkManager] 未知消息格式: {jsonBody}");
            }
        }

        // ===================================================================================
        // 5. 请求发送逻辑
        // ===================================================================================
        public void SendLoginRequest(string username, string password)
        {
            var dto = new UserLoginDTO { username = username, password = password };
            SendPostRequest("/user/login", JsonUtility.ToJson(dto), false);
        }

        public void SendJoinGameRequest(int serverId)
        {
            var dto = new JoinGameDTO { server_id = serverId };
            string jsonBody = JsonUtility.ToJson(dto);
            Debug.Log($"[NetworkManager] 发送选服请求: {jsonBody}");
            SendPostRequest("/player/joinGame", jsonBody, true);
        }

        private void SendPostRequest(string url, string jsonBody, bool needAuth)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"POST {url} HTTP/1.1\r\n");
            sb.Append($"Host: {SERVER_IP}:{SERVER_PORT}\r\n");
            sb.Append("Content-Type: application/json\r\n");

            if (needAuth && !string.IsNullOrEmpty(_accessToken))
                sb.Append($"Authorization: Bearer {_accessToken}\r\n");

            sb.Append($"Content-Length: {Encoding.UTF8.GetByteCount(jsonBody)}\r\n");
            sb.Append("Connection: Keep-Alive\r\n\r\n");
            sb.Append(jsonBody);

            byte[] bytes = Encoding.UTF8.GetBytes(sb.ToString());
            Send(bytes);
        }

        public void Send(byte[] data)
        {
            if (!_isConnected || _stream == null)
            {
                Debug.LogError("[NetworkManager] 发送失败：未连接到服务器！");
                return;
            }
            try
            {
                _stream.Write(data, 0, data.Length);
            }
            catch (Exception e)
            {
                Close();
                Debug.LogError($"[NetworkManager] 发送异常: {e.Message}");
            }
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
// 6. 数据结构定义
// ===================================================================================

[Serializable]
    public class ServerResponse<T>
    {
        public string status;
        public string message;
        public string error;
        public T result;
    }

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

    [Serializable]
    public class UserLoginResultDTO
    {
        public string access_token;
        public int expires_in;
        public List<ServerDTO> server_list;
    }

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
        public int level;
        public int tile_index;
        public int is_fixed;
        public int posX;
        public int posY;
        public int posZ;
        public int state;
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