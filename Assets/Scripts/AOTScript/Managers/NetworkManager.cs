using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json; // 必须引用！用于序列化字典
using GameFramework.Events;

namespace GameFramework.Managers
{
    public class NetworkManager : Singleton<NetworkManager>
    {
        private const string LOGIN_SERVER_IP = "192.168.10.116";
        private const int LOGIN_SERVER_PORT = 8009;

        private const int DEFAULT_SERVER_INDEX = 2;

        private string _currentIp = LOGIN_SERVER_IP;
        private int _currentPort = LOGIN_SERVER_PORT;
        private string _accessToken = "";

        public List<ServerDTO> CachedServerList { get; private set; } = new List<ServerDTO>();
        public GamesDTO CurrentGameData { get; private set; }
        public ServerDTO CurrentSelectedServer { get; private set; }
        public bool IsConnected => !string.IsNullOrEmpty(_accessToken); // 简单判断

        // ===================================================================================
        // 3. 公开业务接口
        // ===================================================================================
        /// <summary>
        /// 网络模块初始化
        /// </summary>
        public void Initialize()
        {
            Debug.Log("[NetworkManager] 初始化，自动开始登录流程");
            SendLogin("1930616513", "lzhlzh617");
        }

        /// <summary>
        /// [修改] 切换当前服务器
        /// 职责：更新本地状态 -> 切换底层IP -> 广播事件通知UI
        /// </summary>
        public void SwitchServer(ServerDTO targetServer)
        {
            if (targetServer == null)
            {
                Debug.LogError("[NetworkManager] 试图切换到一个空的服务器配置！");
                return;
            }

            // 1. 更新数据状态
            CurrentSelectedServer = targetServer;
            _currentIp = targetServer.ip;

            if (int.TryParse(targetServer.port, out int port))
            {
                _currentPort = port;
            }
            else
            {
                Debug.LogWarning($"[NetworkManager] 端口解析失败: {targetServer.port}，使用默认端口 80");
                _currentPort = 80;
            }

            Debug.Log($"[NetworkManager] ===> 切换服务器: {targetServer.name} ({_currentIp}:{_currentPort}) <===");

            // 2. [关键] 广播事件通知 UI 刷新
            EventManager.Instance.Publish(new ServerSelectedEvent(targetServer));
        }
        /// <summary>
        /// [新增] 进入游戏逻辑
        /// UI 只需调用此方法，无需关心 URL 和 参数封装
        /// </summary>
        public void JoinGame()
        {
            if (CurrentSelectedServer == null)
            {
                Debug.LogError("[NetworkManager] 未选择服务器，无法进入游戏！");
                return;
            }

            Debug.Log($"[NetworkManager] 请求进入服务器: {CurrentSelectedServer.name} (ID: {CurrentSelectedServer.server_id})");

            // 封装请求参数
            var requestData = new RequestData();
            requestData.AddField("server_id", CurrentSelectedServer.server_id);

            // 发送请求
            SendGameRequest("/player/joinGame", requestData);
        }
        /// <summary>
        /// 发送登录请求
        /// </summary>
        /// <param name="username">账号</param>
        /// <param name="password">密码</param>
        public async void SendLogin(string username, string password)
        {
            // 设置初始IP、PORT
            _currentIp = LOGIN_SERVER_IP;
            _currentPort = LOGIN_SERVER_PORT;

            // 封装数据
            var data = new RequestData();
            data.AddField("username", username);
            data.AddField("password", password);

            Debug.Log($"[NetworkManager] 发送登录请求至 {_currentIp}:{_currentPort}...");

            // 发送登录请求等待返回数据
            var response = await PostAsync<UserLoginResultDTO>("/user/login", data, false);
            if (response != null && response.status == "success")
            {
                _accessToken = response.result.access_token;
                CachedServerList = response.result.server_list;
                Debug.Log($"[NetworkManager] 登录成功，获取到 {CachedServerList.Count} 个服务器");
                // [核心修改] 登录成功后，立即执行默认选服逻辑
                if (CachedServerList != null && CachedServerList.Count > 0)
                {
                    int targetIndex = DEFAULT_SERVER_INDEX;
                    // 防越界保护
                    if (targetIndex < 0 || targetIndex >= CachedServerList.Count)
                    {
                        targetIndex = 0;
                    }

                    Debug.Log($"[NetworkManager] 执行默认选服策略 (Index: {targetIndex})");
                    // 调用 SwitchServer 会更新 CurrentSelectedServer 并广播 ServerSelectedEvent
                    SwitchServer(CachedServerList[targetIndex]);
                }

                // 发布登录成功事件
                EventManager.Instance.Publish(new LoginSuccessEvent());
            }
            else
            {
                Debug.LogError($"[NetworkManager] 登录失败: {response?.message}");
            }
        }

        /// <summary>
        /// 发送通用业务请求（无需返回值，主要用于触发数据更新）
        /// </summary>
        public async void SendGameRequest(string url, RequestData requestData)
        {
            CheckLoginServerWarning(url);

            Debug.Log($"[NetworkManager] 发送业务请求: {url} | Data: {requestData.ToJson()}");

            var response = await PostAsync<GamesDTO>(url, requestData, true);

            if (response != null && response.status == "success")
            {
                Debug.Log($"[NetworkManager] 请求成功 ({url})，更新游戏数据");
                HandleGameDataUpdate(response.result);
            }
            else
            {
                Debug.LogError($"[NetworkManager] 请求失败 ({url}): {response?.message}");
            }
        }

        /// <summary>
        /// [核心] 泛型异步接口，供业务系统直接 await 使用
        /// 支持传入 RequestData 或 匿名对象
        /// </summary>
        public async UniTask<T> SendAsync<T>(string url, object requestData)
        {
            CheckLoginServerWarning(url);

            // 兼容性处理：如果是 RequestData，调用 ToJson；如果是普通对象/字典，直接序列化
            string jsonBody;
            if (requestData is RequestData rd)
            {
                jsonBody = rd.ToJson();
            }
            else
            {
                jsonBody = JsonConvert.SerializeObject(requestData);
            }

            Debug.Log($"[NetworkManager] SendAsync请求: {url} | Body: {jsonBody}");

            // 复用底层的 PostRawAsync
            var response = await PostRawAsync<T>(url, jsonBody, true);

            if (response != null && response.status == "success")
            {
                // 如果返回类型是 GamesDTO，尝试自动更新 Token 和 全局数据
                if (response.result is GamesDTO gamesDto)
                {
                    HandleGameDataUpdate(gamesDto);
                }
                return response.result;
            }
            else
            {
                Debug.LogError($"[NetworkManager] SendAsync失败 {url}: {response?.message}");
                return default;
            }
        }

        // ===================================================================================
        // 4. 辅助逻辑
        // ===================================================================================

        private void CheckLoginServerWarning(string url)
        {
            if (_currentIp == LOGIN_SERVER_IP && (url.Contains("joinGame") || url.Contains("create")))
            {
                Debug.LogWarning($"[NetworkManager] 警告：正在向 [登录服] 发送 {url} 请求！请检查是否已选服。");
            }
        }

        private void HandleGameDataUpdate(GamesDTO data)
        {
            if (data == null) return;

            // 1. 检查 Token
            if (data.tokenResult != null && !string.IsNullOrEmpty(data.tokenResult.access_token))
            {
                _accessToken = data.tokenResult.access_token;
                Debug.Log($"[NetworkManager] Token 已自动刷新");
            }

            // 2. 更新全局缓存
            CurrentGameData = data;

            // 3. 广播事件
            EventManager.Instance.Publish(new GameDataReceivedEvent(data));
        }

        // ===================================================================================
        // 5. 底层网络实现 (UnityWebRequest)
        // ===================================================================================

        private async UniTask<ServerResponse<T>> PostAsync<T>(string path, RequestData data, bool needAuth)
        {
            return await PostRawAsync<T>(path, data.ToJson(), needAuth);
        }

        private async UniTask<ServerResponse<T>> PostRawAsync<T>(string path, string jsonBody, bool needAuth)
        {
            string url = $"http://{_currentIp}:{_currentPort}{path}";

            // 如果 URL 不包含 http 前缀（防止配置错误）
            if (!url.StartsWith("http")) url = "http://" + url;

            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

            using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
            {
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();

                request.SetRequestHeader("Content-Type", "application/json");
                if (needAuth && !string.IsNullOrEmpty(_accessToken))
                {
                    request.SetRequestHeader("Authorization", $"Bearer {_accessToken}");
                }

                request.timeout = 10; // 10秒超时

                try
                {
                    await request.SendWebRequest();

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        string responseText = request.downloadHandler.text;
                        // Debug.Log($"[NetworkManager] 收到响应: {responseText}"); // 可选：打印详细日志
                        return JsonConvert.DeserializeObject<ServerResponse<T>>(responseText);
                    }
                    else
                    {
                        Debug.LogError($"[NetworkManager] 网络错误: {request.error} | Code: {request.responseCode}");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"[NetworkManager] 请求异常: {e.Message}");
                }
            }
            return null;
        }
    }

    // ===================================================================================
    // 数据封装类 (结合了你的旧项目逻辑和Newtonsoft)
    // ===================================================================================
    public class RequestData
    {
        private Dictionary<string, object> _data = new Dictionary<string, object>();

        public void AddField(string key, object value)
        {
            _data[key] = value;
        }

        public string ToJson()
        {
            // 使用 Newtonsoft.Json 序列化，这是最安全、兼容性最好的方式
            return JsonConvert.SerializeObject(_data);
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

    [Serializable] public class JoinGameDTO { public int server_id; }

    [Serializable]
    public class TokenResultDTO
    {
        public string access_token;
        public int expires_in;
    }

    // [核心] 游戏主数据结构
    [Serializable]
    public class GamesDTO
    {
        // 鉴权结果
        public TokenResultDTO tokenResult;

        // 基础信息
        public PlayerDTO Player;
        public List<TileDTO> Tile;
        public List<BuildingDTO> Building;
        public List<ItemDTO> Item;

        // [新增] 补全 Swagger 中定义的其他模块
        public OfflineDTO Offline;                  // 离线收益
        public List<RewardItemDTO> RewardItem;      // 奖励物品
        public List<QuestDTO> Quest;                // 任务列表
        public BuildingUnlockDTO BuildingUnlock;    // 建筑解锁进度
        public List<GoblinDTO> Goblin;              // 工人/地精
        public BuildingTerminalDTO BuildingTerminal;// 航站楼
        public List<BuildingPlaneDTO> BuildingPlane;// 飞艇
        public List<BoardStartDTO> Board;           // 棋盘/关卡信息
        public GuideDTO Guide;                      // 引导进度
        public BuildingTalentDTO BuildingTalent;    // 人才中心
        public List<BuildingEmployeeDTO> BuildingEmployee; // 雇员列表
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

        // [注意] Swagger文档中未定义此字段，但代码逻辑需要
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

        // [新增] 补全字段
        public long create_time;
        public List<int> Output; // 产出列表
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

    // ===================================================================================
    // [新增] 补充的子模块 DTO
    // ===================================================================================

    [Serializable]
    public class OfflineDTO
    {
        public List<BuildingUpDTO> Building;
        public List<TileUpDTO> Tile;
    }

    [Serializable]
    public class BuildingUpDTO
    {
        public int building_id;
        public int level;
        public int number;
    }

    [Serializable]
    public class TileUpDTO
    {
        public int tile_id;
        public int level;
        public int number;
    }

    [Serializable]
    public class RewardItemDTO
    {
        public int type;
        public int star;
        public int score;
        public string items; // 格式: itemId_count;...
    }

    [Serializable]
    public class QuestDTO
    {
        public string _id;
        public int quest_id;
        public int quest_type;
        public int target;
        public int targetCount;
        public int state; // 0进行中 1已完成 2已领奖
    }

    [Serializable]
    public class BuildingUnlockDTO
    {
        public int berthCount;
        public List<int> airshipType;
        public int cycleSpeed;
        public int airshipMax;
        public int goblins;

        // 注意: Swagger 定义 kinds 为 object，JsonUtility 无法直接反序列化 Dictionary/Object。
        // 如果后端传回的是复杂结构，这里可能需要自定义解析或使用第三方库 (Newtonsoft)。
        // public string kinds; 
    }

    [Serializable]
    public class GoblinDTO
    {
        public string player_id;
        public int goblin_id;
        public string building_id;      // 住所ID
        public string work_building_id; // 工作地ID
        public int work_type;
        public int state;
    }

    [Serializable]
    public class BuildingTerminalDTO
    {
        public string player_id;
        public string building_id;
        public List<string> posList;
    }

    [Serializable]
    public class BuildingPlaneDTO
    {
        public string _id;
        public string player_id;
        public int plane_id;
        public int passenger_level;
        public int cycle_level;
        public int fuel_level;
        public int state;
        public int pos;
        public int totalPassengerCount;
        public int flyingTimes;
        public long start_time;
        public long end_time;
    }

    [Serializable]
    public class BoardStartDTO
    {
        public string player_id;
        public int board_id;
        public int star;
        public int max_score;
        public int state;
        public int is_first;
    }

    [Serializable]
    public class GuideDTO
    {
        public int guide_type;
        public int guide_id;
    }

    [Serializable]
    public class BuildingTalentDTO
    {
        public string player_id;
        public List<int> employees;
        public List<int> recruits;
        public long refresh_time;
        public int m_refresh_count;
        public int f_refresh_count;
        public long create_time;
        public long reset_time;
    }

    [Serializable]
    public class BuildingEmployeeDTO
    {
        public string _id;
        public int employee_id;
        public string work_building_id;
        public int state;
    }

    // ===================================================================================
    // 请求 DTO (Request Bodies)
    // ===================================================================================

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
        public int tile_type; // 对应 Swagger: tile_type
        public int posX;
        public int posY;
        public int posZ;
    }

    [Serializable]
    public class TileUpLevelDTO
    {
        public string _id;
    }

    [Serializable]
    public class BuildingUpLevelDTO
    {
        public string _id;
    }

    [Serializable]
    public class BuildingMoveDTO
    {
        public string _id;
        public int posX;
        public int posY;
        public int posZ;
        public int rotate;
    }
}