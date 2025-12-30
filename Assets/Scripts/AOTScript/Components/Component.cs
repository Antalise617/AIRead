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
    public interface IPlacementConfirmPanel
    {
        void Show();
        void Hide();
        void UpdatePosition(Vector3 worldPosition);

        // 用于判断 UI 是否显示
        bool IsVisible { get; }
    }

    // 可建建筑列表
    [InternalBufferCapacity(8)]
    public struct BuildableStructureElement : IBufferElementData
    {
        public int StructureType;
    }

    /// <summary>
    /// 岛屿状态机组件
    /// 记录岛屿当前的生命周期状态及相关时间戳
    /// </summary>
    public struct IslandStatusComponent : IComponentData
    {
        /// <summary>
        /// 岛屿状态
        /// 1: 正常 (Normal)
        /// 2: 建造/升级中 (Building)
        /// 3: 销毁中 (Destroying)
        /// (具体枚举值需与后端对应)
        /// </summary>
        public int State;

        /// <summary>
        /// 操作开始时间 (时间戳: 秒或毫秒)
        /// 用于计算进度条：Progress = (Now - Start) / (End - Start)
        /// </summary>
        public long StartTime;

        /// <summary>
        /// 操作预计结束时间 (时间戳)
        /// </summary>
        public long EndTime;

        /// <summary>
        /// 岛屿最初创建的时间 (时间戳)
        /// </summary>
        public long CreateTime;
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

    #region 建筑相关组件
    public struct BuildingComponent : IComponentData
    {
        public int ConfigId;
        public FixedString64Bytes Name;
        public int3 Size;
        public int BuildingType;
        public int BuildingSubtype;
    }

    // 【新增】繁荣度组件
    public struct ProsperityComponent : IComponentData
    {
        public int Value; // 提供的繁荣度数值
    }

    // 【新增】电力组件
    public struct ElectricityComponent : IComponentData
    {
        public int PowerConsumption; // 耗电量 (负数代表发电，正数代表耗电，视配表而定，通常配表填正数代表消耗)
        public bool IsPowered;       // 是否有电 (决定建筑是否工作)
    }

    public struct VisitorCenterComponent : IComponentData
    {
        public int UnspawnedVisitorCount;
        public float SpawnTimer;
        public float SpawnInterval;
    }
    // 【新增】原料清单 Buffer (ItemId, Count)
    [InternalBufferCapacity(4)] // 预估大多数工厂原料不超过4种，优化内存
    public struct ProductionInputElement : IBufferElementData
    {
        public int ItemId;
        public int Count;
    }

    // 【新增】产出清单 Buffer (ItemId, Count)
    [InternalBufferCapacity(4)]
    public struct ProductionOutputElement : IBufferElementData
    {
        public int ItemId;
        public int CountPerCycle;    // [配置] 单次生产增加的数量
        public int CurrentStorage;   // [运行时] 当前已生产并存储的数量
    }
    [InternalBufferCapacity(4)]
    public struct IslandAffinityElement : IBufferElementData
    {
        public int IslandType; // 对应 zsEnum.IslandType
    }

    // 【修改】生产组件：增加岗位和职业信息，移除旧的 CurrentReserves
    public struct ProductionComponent : IComponentData
    {
        public float ProductionInterval;
        public int MaxReserves;          // 总库存上限 (所有产出物数量之和不能超过此值)

        // 新增配置数据
        public int JobSlots;             // 岗位数量
        public int DemandOccupation;     // 需求职业 (zsEnum.Profession)

        // 运行时状态
        public float Timer;
        public bool IsActive;
        // public int CurrentReserves;   <-- [移除] 改为通过遍历 OutputElement 动态计算
    }
    public struct HasProgressBarTag : IComponentData { }
    // 【修改】服务组件：单人服务模式
    public struct ServiceComponent : IComponentData
    {
        public int ServiceConfigId;

        // --- 配置数据 ---
        public float ServiceTime;        // 单次服务所需时间
        public int MaxVisitorCapacity;   // 建筑内最大容纳人数 (包含正在服务的 + 排队的)

        public int OutputItemId;         // 服务奖励ID (金币)
        public int OutputItemCount;      // 服务奖励数量

        // --- 运行时状态 ---
        public bool IsActive;            // 开关
        public bool IsServing;           // 是否正在进行服务 (True = 柜台有人)
        public float ServiceTimer;       // 当前服务倒计时
        public int CurrentServingItemId; // 记录当前服务消耗了什么物品 (用于结算或统计)
    }

    // 【修改】游客队列 (存储所有在建筑内的游客)
    // 逻辑约定：队列第0个元素即为“正在接受服务”的游客，后面的是“排队中”的游客
    [InternalBufferCapacity(10)]
    public struct ServiceQueueElement : IBufferElementData
    {
        public Entity VisitorEntity;
    }
    #endregion

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

    public struct GridCellData
    {
        public int3 Position;
        public float3 WorldPosition;
        public GridType Type;
        public FixedString64Bytes IslandID;
        public FixedString64Bytes BuildingID;
        public bool IsMovable;
        public bool IsBuildable;
        public bool IsBridgeable;
    }

    public enum GridType : byte { Space, Island, IslandAirspace, Building, PublicBridge, PrivateBridge }

    // 事件接口
    public interface IGameEvent { }

    // 事件示例
    public struct GamePausedEvent : IGameEvent
    {
        public bool IsPaused;
    }

    public struct ScoreChangedEvent : IGameEvent
    {
        public int OldScore;
        public int NewScore;
    }

    // 资源类型枚举保持不变
    public enum ResourceType
    {
        Wood = 0,   // 木头
        Stone = 1,  // 石头
        Gold = 2,   // 金币
        Food = 3    // 食物
    }

    // ★ 关键修改：实现 IGameEvent 接口
    public partial struct ResourceChangedEvent : IGameEvent
    {
        public ResourceType Type;
        public int NewValue;
        public int Delta; // 变化量

        public ResourceChangedEvent(ResourceType type, int newValue, int delta)
        {
            Type = type;
            NewValue = newValue;
            Delta = delta;
        }
    }

    // 定义任务更新事件
    public struct TaskUpdatedEvent : IGameEvent
    {
        public Unity.Collections.FixedString128Bytes Description; // 任务描述
        public bool IsFinished;    // 是否全部完成（可选，用于控制UI显隐）
    }

    public partial struct DestroySystem : ISystem
    {
        //[BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            new DestroyEntityJob
            {
                Ecb = ecb.AsParallelWriter()
            }.ScheduleParallel();
        }

        //[BurstCompile]
        partial struct DestroyEntityJob : IJobEntity
        {
            public EntityCommandBuffer.ParallelWriter Ecb;

            void Execute(Entity entity, [EntityIndexInQuery] int sortKey, in DestroyTag tag)
            {
                Ecb.DestroyEntity(sortKey, entity);
            }
        }
    }
}