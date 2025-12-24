using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace GameFramework.ECS.Components
{
    // 通用组件
    public struct DestroyTag : IComponentData { }

    public struct GlobalInputComponent : IComponentData
    {
        public bool IsConfirmPlace;
        public bool IsCancelPlace;
        public int3 HoverGridPosition;
        public bool HasHoverTarget;
    }

    public struct GridConfigComponent : IComponentData
    {
        public int Width;
        public int Length;
        public int Height;
        public float CellSize;
    }

    public struct VisualGridTag : IComponentData { }
    public struct BridgeHintTag : IComponentData { }

    public struct GridPositionComponent : IComponentData
    {
        public int3 Value;
    }

    public struct PlacementStateComponent : IComponentData
    {
        public bool IsActive;
        public int CurrentObjectId;
        public PlacementType Type;
        public int3 CurrentGridPos;
        public bool IsPositionValid;
        public int RotationIndex;
    }

    public enum PlacementType
    {
        None,
        Island,
        Building,
        Bridge
    }

    public struct NewIslandTag : IComponentData { }

    #region 岛屿相关组件
    public struct IslandComponent : IComponentData
    {
        public int ConfigId;
        public int3 Size;
        public int AirSpace;
    }
    public struct IslandDataComponent : IComponentData
    {
        public int Level;               // 当前等级
        public int IslandType;          // 存储枚举的 int 值
        public int BonusType;           // 存储加成枚举的 int 值
        public int BonusValue;          // 加成值
    }

    // 可建建筑列表
    [InternalBufferCapacity(8)]
    public struct BuildableStructureElement : IBufferElementData
    {
        public int StructureType;
    }
    #endregion

    public struct PlaceObjectRequest : IComponentData
    {
        public int ObjectId;
        public int3 Position;
        public PlacementType Type;
        public int3 Size;
        public quaternion Rotation;
        public int AirspaceHeight;
        public int RotationIndex;
    }

    public struct AssetReferenceComponent : IComponentData
    {
        public Unity.Collections.FixedString64Bytes ResourcePath;
    }

    public class ViewInstanceComponent : IComponentData
    {
        public GameObject GameObject;
        public Transform Transform;
    }

    // 建筑相关 (FuncType 改为 int)
    

    public struct BridgeComponent : IComponentData
    {
        public int ConfigId;
    }

    // 游客相关
    public enum VisitorState
    {
        Idle,
        Pathfinding,
        Moving,
        Arrived,
        // 新增状态
        Waiting,    // 在建筑排队中
        BeingServed // 正在接受服务
    }

    public struct VisitorComponent : IComponentData
    {
        public FixedString64Bytes Name;
        public int Age;
        public float MoveSpeed;
        public VisitorState CurrentState;
        public float StateTimer;

        // 【新增】记录当前前往的目标建筑实体
        public Entity TargetBuildingEntity;
    }

    [InternalBufferCapacity(8)]
    public struct VisitedBuildingElement : IBufferElementData
    {
        public int BuildingConfigId;
    }

    [InternalBufferCapacity(30)]
    public struct PathBufferElement : IBufferElementData
    {
        public int3 GridPos;
    }

    public struct PathfindingRequest : IComponentData, IEnableableComponent
    {
        public int3 StartPos;
        public int3 EndPos;
        public bool IsPending;
    }


    #region 建筑相关组件
    public struct BuildingComponent : IComponentData
    {
        public int ConfigId;
        public int Name;
        public int3 Size;
        public int BuildingType;
        public int BuildingSubtype;
    }

    public struct VisitorCenterComponent : IComponentData
    {
        public int UnspawnedVisitorCount;
        public float SpawnTimer;
        public float SpawnInterval;
    }
    /// <summary>
    /// 建筑生产组件
    /// </summary>
    public struct ProductionComponent : IComponentData
    {
        // 生产配置数据
        public int InputItemId; // 输入物品ID
        public int InputCount; // 输入数量
        public int OutputItemId; // 输出物品ID
        public int OutputCount; // 输出数量
        public float ProductionInterval; // 生产周期
        public int MaxReserves; // 储量上限

        // 生产时数据
        public float Timer;           // 当前计时
        public bool IsActive;         // 是否生产中
        public int CurrentReserves; // 当前储量
    }
    /// <summary>
    /// 服务设施组件
    /// </summary>
    public struct ServiceComponent : IComponentData
    {
        public int ServiceConfigId;      // 对应的服务配置ID (220001等)

        // --- 配置数据 ---
        public float ServiceTime;        // 单次服务所需时间
        public int QueueCapacity;        // 排队队列上限
        public int MaxConcurrentNum;     // 最大同时服务人数

        // --- 产出配置 (根据csv中的 outputItem: 104;10) ---
        public int OutputItemId;         // 服务完成后获得的奖励ID
        public int OutputItemCount;      // 奖励数量

        public bool IsActive;            // 建筑是否激活
    }

    // === 新增：排队队列 Buffer ===
    [InternalBufferCapacity(10)] // 预设容量，超过会自动分配堆内存
    public struct ServiceQueueElement : IBufferElementData
    {
        public Entity VisitorEntity;
    }

    /// <summary>
    /// 正在服务中的槽位 (支持配置表中的 concurrentlyServiceNum > 1)
    /// </summary>
    [InternalBufferCapacity(4)]
    public struct ServiceSlotElement : IBufferElementData
    {
        public Entity VisitorEntity; // 正在服务的游客
        public float Timer;          // 当前服务进度
        public bool IsOccupied;      // 槽位是否被占用
    }
    #endregion
}