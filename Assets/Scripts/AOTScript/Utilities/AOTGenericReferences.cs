using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using GameFramework.ECS.Components;
using Cysharp.Threading.Tasks; // 必须引用 UniTask

public class AOTGenericReferences : MonoBehaviour
{
    // 在 Start 里调用，确保代码被引用
    void Start()
    {
        Ref();
    }

    public void Ref()
    {
        // =========================================================
        // ★★★ 核心修复：UniTask<Entity> ★★★
        // =========================================================
        RefUniTask();

        // =========================================================
        // 其他 ECS 引用
        // =========================================================
        RefComponent<DestroyTag>();
        RefComponent<GlobalInputComponent>();
        RefComponent<GridConfigComponent>();
        RefComponent<VisualGridTag>();
        RefComponent<BridgeHintTag>();
        RefComponent<GridPositionComponent>();
        RefComponent<PlacementStateComponent>();
        RefComponent<NewIslandTag>();
        RefComponent<IslandComponent>();
        RefComponent<PlaceObjectRequest>();
        RefComponent<AssetReferenceComponent>();
        RefComponent<BuildingComponent>();
        RefComponent<VisitorCenterComponent>();
        RefComponent<BridgeComponent>();
        RefComponent<VisitorComponent>();
        RefComponent<PathfindingRequest>();

        RefPlainData<GridCellData>();

        RefBuffer<VisitedBuildingElement>();
        RefBuffer<PathBufferElement>();

        RefHashMap<int3, GridCellData>();
        RefHashMap<int3, Entity>();

        RefList<GamePausedEvent>();
        RefList<ScoreChangedEvent>();
        RefList<ResourceChangedEvent>();
        RefList<TaskUpdatedEvent>();
    }

    // -------------------------------------------------------------
    // 强制生成 UniTask<Entity> 的状态机代码
    // -------------------------------------------------------------
    async void RefUniTask()
    {
        // 这一步至关重要：告诉 IL2CPP 我们需要 UniTask<Entity> 的异步状态机
        await ForceUniTaskEntity();
    }

    async UniTask<Entity> ForceUniTaskEntity()
    {
        await UniTask.Yield();
        return Entity.Null;
    }

    // -------------------------------------------------------------
    // 通用防裁剪模板
    // -------------------------------------------------------------
    void RefComponent<T>() where T : unmanaged, IComponentData
    {
        NativeArray<T> array = new NativeArray<T>(0, Allocator.Temp);
        if (array.Length > 0) { array[0] = default; var x = array[0]; }
        array.Dispose();

        ComponentLookup<T> lookup = default;
        if (lookup.HasComponent(Entity.Null)) { var val = lookup[Entity.Null]; }

        EntityCommandBuffer ecb = default;
        ecb.AddComponent(Entity.Null, new T());
    }

    void RefPlainData<T>() where T : unmanaged
    {
        NativeArray<T> array = new NativeArray<T>(0, Allocator.Temp);
        if (array.Length > 0) { array[0] = default; var x = array[0]; }
        array.Dispose();
    }

    void RefBuffer<T>() where T : unmanaged, IBufferElementData
    {
        DynamicBuffer<T> buffer = default;
        if (buffer.IsCreated) { buffer.Add(default); var val = buffer[0]; }
    }

    void RefHashMap<TK, TV>() where TK : unmanaged, System.IEquatable<TK> where TV : unmanaged
    {
        var map = new NativeParallelHashMap<TK, TV>(0, Allocator.Temp);
        map.TryAdd(default, default);
        map.Dispose();
    }

    void RefList<T>() where T : unmanaged
    {
        var list = new NativeList<T>(0, Allocator.Temp);
        list.Add(default);
        list.Dispose();
    }
}