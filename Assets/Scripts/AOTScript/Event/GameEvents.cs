using System.Collections.Generic;
using GameFramework.ECS.Components; // 引用 PlacementType 等枚举
using GameFramework.Managers;       // 引用 DTO (GamesDTO等)

namespace GameFramework.Events
{
    // ===================================================================================
    // 核心玩法事件 (Gameplay)
    // ===================================================================================

    /// <summary>
    /// 物体建造完成事件
    /// </summary>
    public struct ObjectBuiltEvent : IGameEvent
    {
        public PlacementType Type;
        public int ConfigID;
        public int EntityId; // 建议由 private 改为 public，以便系统访问

        public ObjectBuiltEvent(PlacementType type, int configId, int entityId)
        {
            Type = type;
            ConfigID = configId;
            EntityId = entityId;
        }
    }

    /// <summary>
    /// 任务状态/描述更新事件 (用于刷新 UI)
    /// </summary>
    public struct TaskUpdatedEvent : IGameEvent
    {
        public string Description;
        public bool IsFinished;
    }

    // ===================================================================================
    // 网络通信事件 (Network)
    // ===================================================================================

    /// <summary>
    /// 登录成功事件 (当从登录服获取到 Token 和服务器列表后触发)
    /// </summary>
    public struct LoginSuccessEvent : IGameEvent
    {
    }

    /// <summary>
    /// 收到游戏全量数据事件 (通常是选服进入游戏，或重连时触发)
    /// </summary>
    public struct GameDataReceivedEvent : IGameEvent
    {
        public GamesDTO Data;

        public GameDataReceivedEvent(GamesDTO data)
        {
            Data = data;
        }
    }

    // ===================================================================================
    // UI 交互事件 (UI Interaction)
    // ===================================================================================

    /// <summary>
    /// 玩家点击了具体的服务器项 (用于 UI 选中态刷新)
    /// </summary>
    public struct ServerSelectedEvent : IGameEvent
    {
        public ServerDTO ServerInfo;
        public ServerSelectedEvent(ServerDTO info) { ServerInfo = info; }
    }
}