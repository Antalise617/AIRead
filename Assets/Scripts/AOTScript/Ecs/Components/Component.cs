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

    public struct IslandComponent : IComponentData
    {
        public int ConfigId;
        public int3 Size;
        public int AirSpace;
    }

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
    public struct BuildingComponent : IComponentData
    {
        public int ConfigId;
        public int3 Size;
        public int FuncType;
    }

    public struct VisitorCenterComponent : IComponentData
    {
        public int UnspawnedVisitorCount;
        public float SpawnTimer;
        public float SpawnInterval;
    }

    public struct BridgeComponent : IComponentData
    {
        public int ConfigId;
    }

    // 游客相关
    public enum VisitorState { Idle, Pathfinding, Moving, Arrived }

    public struct VisitorComponent : IComponentData
    {
        public FixedString64Bytes Name;
        public int Age;
        public float MoveSpeed;
        public VisitorState CurrentState;
        public float StateTimer;
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


    #region 建筑功能相关组件
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
    #endregion
}