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
}