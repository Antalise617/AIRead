using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using cfg.building;

namespace GameFramework.ECS.Components
{
    #region 通用组件定义
    public struct DestroyTag : IComponentData { }

    // 这是一个特殊的组件，整个世界只存一份，用于充当 InputManager 和 ECS 之间的桥梁
    public struct GlobalInputComponent : IComponentData
    {
        // --- 新增放置系统输入 ---
        public bool IsConfirmPlace;     // 是否按下了放置键 (Space/Click)
        public bool IsCancelPlace;      // 是否取消 (Esc/RightClick)
        public int3 HoverGridPosition;  // 鼠标悬停的网格坐标
        public bool HasHoverTarget;     // 鼠标是否指在有效的放置平面上
    }
    #endregion

    #region 网格相关组件定义
    

    // [单例组件] 全局网格配置
    public struct GridConfigComponent : IComponentData
    {
        public int Width;      // X轴数量
        public int Length;     // Z轴数量
        public int Height;     // Y轴层数
        public float CellSize; // 格子大小
    }

    // [标签] 用于标记可视化的格子实体
    public struct VisualGridTag : IComponentData { }

    // [标签] 用于标记桥梁提示的格子实体
    public struct BridgeHintTag : IComponentData { }

    public struct GridPositionComponent : IComponentData
    {
        public int3 Value; // 格子的逻辑坐标 (x, y, z)
    }

    // [保留] 放置状态
    public struct PlacementStateComponent : IComponentData
    {
        public bool IsActive;
        public int CurrentObjectId;
        public PlacementType Type;

        // 这里的坐标将直接从射线命中的 Entity 上读取
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
    #endregion

    #region 岛屿相关组件定义
    public struct NewIslandTag : IComponentData { }

    // 1. 岛屿基础数据组件
    public struct IslandComponent : IComponentData
    {
        public int ConfigId;       // 配置表ID
        public int3 Size;          // 尺寸 (长, 高, 宽)
        public int AirSpace;       // 空域高度
        // 可扩展：人口上限、当前等级等
    }

    // 2. 放置请求组件 (用于系统间传递指令)
    public struct PlaceObjectRequest : IComponentData
    {
        public int ObjectId;
        public int3 Position;
        public PlacementType Type;
        public int3 Size;
        public quaternion Rotation;
        public int AirspaceHeight;

        public int RotationIndex; // [新增] 旋转索引 (0,1,2,3)，用于计算网格逻辑坐标
    }

    // 3. 资源引用组件 (告诉可视化系统加载什么)
    public struct AssetReferenceComponent : IComponentData
    {
        public Unity.Collections.FixedString64Bytes ResourcePath;
    }

    // 4. 视图实例组件 (Managed组件，用于持有GameObject)
    public class ViewInstanceComponent : IComponentData
    {
        public GameObject GameObject;
        public Transform Transform;
    }
    #endregion

    #region 寻路相关组件定义

    #endregion

    #region 建筑相关组件定义
    /// <summary>
    /// 建筑组件：标记该实体为建筑，并存储运行时数据
    /// </summary>
    public struct BuildingComponent : IComponentData
    {
        public int ConfigId;    // 对应配置表中的 ID
        public int3 Size;       // 建筑尺寸 (x, y, z)
        public FunctionType FuncType; // [新增] 建筑功能类型枚举
    }

    /// <summary>
    /// 游客中心功能组件
    /// 挂载此组件的实体具备生成游客的能力
    /// </summary>
    public struct VisitorCenterComponent : IComponentData
    {
        public int UnspawnedVisitorCount; // 待生成的游客数量 (库存)
        public float SpawnTimer;          // 生成计时器
        public float SpawnInterval;       // 生成间隔 (秒)
    }
    #endregion

    #region 桥梁相关组件定义
    /// <summary>
    /// 桥梁组件：标记该实体为桥梁，并记录基础配置ID
    /// </summary>
    public struct BridgeComponent : IComponentData
    {
        public int ConfigId;
    }
    #endregion

    #region 游客相关组件定义
    public enum VisitorState
    {
        Idle,           // 0: 空闲/思考中
        Pathfinding,    // 1: 正在等待寻路
        Moving,         // 2: 正在移动
        Arrived         // 3: 到达目的地（短暂状态）
    }

    public struct VisitorComponent : IComponentData
    {
        public FixedString64Bytes Name;
        public int Age;
        public float MoveSpeed;

        // 状态机数据
        public VisitorState CurrentState;
        public float StateTimer; // 通用计时器（用于停留、发呆等）
    }

    // [Buffer] 已访问过的建筑 ID 列表
    [InternalBufferCapacity(8)]
    public struct VisitedBuildingElement : IBufferElementData
    {
        public int BuildingConfigId;
    }

    // [Buffer] 移动路径点队列
    [InternalBufferCapacity(30)]
    public struct PathBufferElement : IBufferElementData
    {
        public int3 GridPos;
    }

    // [Component] 寻路请求
    public struct PathfindingRequest : IComponentData, IEnableableComponent
    {
        public int3 StartPos;
        public int3 EndPos;
        public bool IsPending; // true = 需要计算, false = 计算完成
    }

    #endregion
}
