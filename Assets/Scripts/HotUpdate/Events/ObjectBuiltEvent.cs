using GameFramework.Events;
using GameFramework.ECS.Components; // 引用 PlacementType

public struct ObjectBuiltEvent : IGameEvent // ★★★ 关键：加上这个接口实现 ★★★
{
    public PlacementType Type;
    public int ConfigID;
    private int currentObjectId;

    public ObjectBuiltEvent(PlacementType type, int currentObjectId) : this()
    {
        Type = type;
        this.currentObjectId = currentObjectId;
    }
}