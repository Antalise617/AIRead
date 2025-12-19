using GameFramework.ECS.Components; // 引用 PlacementType

namespace GameFramework.Events
{
    // 假设 IGameEvent 定义在 GameFramework.Events 命名空间中
    // 如果 IGameEvent 定义在其他地方，请添加对应的 using

    public struct ObjectBuiltEvent : IGameEvent // ★★★ 关键：加上这个接口实现 ★★★
    {
        public PlacementType Type;
    }
}