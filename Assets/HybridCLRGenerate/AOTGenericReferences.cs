using System.Collections.Generic;
public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

	// {{ AOT assemblies
	public static readonly IReadOnlyList<string> PatchedAOTAssemblyList = new List<string>
	{
		"Cinemachine.dll",
		"DOTween.dll",
		"Game.dll",
		"Luban.Runtime.dll",
		"System.Core.dll",
		"System.dll",
		"UniTask.dll",
		"Unity.Burst.dll",
		"Unity.Collections.dll",
		"Unity.Entities.dll",
		"Unity.ResourceManager.dll",
		"UnityEngine.CoreModule.dll",
		"mscorlib.dll",
	};
	// }}

	// {{ constraint implement type
	// }} 

	// {{ AOT generic types
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<object,Unity.Entities.Entity>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<object,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<object,Unity.Entities.Entity>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<object,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<Unity.Entities.Entity>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<object>
	// Cysharp.Threading.Tasks.CompilerServices.IStateMachineRunnerPromise<Unity.Entities.Entity>
	// Cysharp.Threading.Tasks.CompilerServices.IStateMachineRunnerPromise<object>
	// Cysharp.Threading.Tasks.ITaskPoolNode<object>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,Unity.Entities.Entity>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,object>>
	// Cysharp.Threading.Tasks.IUniTaskSource<Unity.Entities.Entity>
	// Cysharp.Threading.Tasks.IUniTaskSource<object>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,Unity.Entities.Entity>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,object>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<Unity.Entities.Entity>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<object>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,Unity.Entities.Entity>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,object>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<Unity.Entities.Entity>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<object>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,Unity.Entities.Entity>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,object>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<Unity.Entities.Entity>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<object>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,Unity.Entities.Entity>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,object>>
	// Cysharp.Threading.Tasks.UniTask<Unity.Entities.Entity>
	// Cysharp.Threading.Tasks.UniTask<object>
	// Cysharp.Threading.Tasks.UniTaskCompletionSourceCore<Cysharp.Threading.Tasks.AsyncUnit>
	// Cysharp.Threading.Tasks.UniTaskCompletionSourceCore<Unity.Entities.Entity>
	// Cysharp.Threading.Tasks.UniTaskCompletionSourceCore<object>
	// DelegateList<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object>>
	// DelegateList<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// DelegateList<float>
	// GameFramework.Managers.ResourceManager.<LoadAssetAsync>d__2<object>
	// GameFramework.Managers.UIManager.<ShowPanelAsync>d__9<object>
	// GameFramework.Singleton<object>
	// System.Action<Cysharp.Threading.Tasks.UniTask>
	// System.Action<GameFramework.ECS.Components.GamePausedEvent>
	// System.Action<GameFramework.ECS.Components.ResourceChangedEvent>
	// System.Action<GameFramework.ECS.Components.TaskUpdatedEvent>
	// System.Action<ObjectBuiltEvent>
	// System.Action<Unity.Entities.Entity>
	// System.Action<Unity.Mathematics.int3>
	// System.Action<UnityEngine.InputSystem.InputAction.CallbackContext>
	// System.Action<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle,object>
	// System.Action<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object>>
	// System.Action<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Action<float>
	// System.Action<int,int>
	// System.Action<int>
	// System.Action<object,object>
	// System.Action<object>
	// System.ArraySegment.Enumerator<GameFramework.ECS.Components.BuildingComponent>
	// System.ArraySegment.Enumerator<GameFramework.ECS.Components.GridCellData>
	// System.ArraySegment.Enumerator<GameFramework.ECS.Components.GridPositionComponent>
	// System.ArraySegment.Enumerator<GameFramework.ECS.Components.PathBufferElement>
	// System.ArraySegment.Enumerator<GameFramework.ECS.Components.PathfindingRequest>
	// System.ArraySegment.Enumerator<GameFramework.ECS.Components.PlaceObjectRequest>
	// System.ArraySegment.Enumerator<GameFramework.ECS.Components.PlacementStateComponent>
	// System.ArraySegment.Enumerator<GameFramework.ECS.Components.VisitedBuildingElement>
	// System.ArraySegment.Enumerator<GameFramework.ECS.Components.VisitorCenterComponent>
	// System.ArraySegment.Enumerator<GameFramework.ECS.Components.VisitorComponent>
	// System.ArraySegment.Enumerator<GameFramework.ECS.Systems.VisitorSpawningSystem.PendingSpawn>
	// System.ArraySegment.Enumerator<System.IntPtr>
	// System.ArraySegment.Enumerator<Unity.Entities.ComponentType>
	// System.ArraySegment.Enumerator<Unity.Entities.Entity>
	// System.ArraySegment.Enumerator<Unity.Entities.EntityQuery>
	// System.ArraySegment.Enumerator<Unity.Entities.EntityQueryBuilder.QueryTypes>
	// System.ArraySegment.Enumerator<Unity.Entities.LinkedEntityGroup>
	// System.ArraySegment.Enumerator<Unity.Entities.PerWorldSystemInfo>
	// System.ArraySegment.Enumerator<Unity.Entities.UpdateIndex>
	// System.ArraySegment.Enumerator<Unity.Mathematics.int3>
	// System.ArraySegment.Enumerator<Unity.Transforms.LocalTransform>
	// System.ArraySegment.Enumerator<int>
	// System.ArraySegment.Enumerator<object>
	// System.ArraySegment<GameFramework.ECS.Components.BuildingComponent>
	// System.ArraySegment<GameFramework.ECS.Components.GridCellData>
	// System.ArraySegment<GameFramework.ECS.Components.GridPositionComponent>
	// System.ArraySegment<GameFramework.ECS.Components.PathBufferElement>
	// System.ArraySegment<GameFramework.ECS.Components.PathfindingRequest>
	// System.ArraySegment<GameFramework.ECS.Components.PlaceObjectRequest>
	// System.ArraySegment<GameFramework.ECS.Components.PlacementStateComponent>
	// System.ArraySegment<GameFramework.ECS.Components.VisitedBuildingElement>
	// System.ArraySegment<GameFramework.ECS.Components.VisitorCenterComponent>
	// System.ArraySegment<GameFramework.ECS.Components.VisitorComponent>
	// System.ArraySegment<GameFramework.ECS.Systems.VisitorSpawningSystem.PendingSpawn>
	// System.ArraySegment<System.IntPtr>
	// System.ArraySegment<Unity.Entities.ComponentType>
	// System.ArraySegment<Unity.Entities.Entity>
	// System.ArraySegment<Unity.Entities.EntityQuery>
	// System.ArraySegment<Unity.Entities.EntityQueryBuilder.QueryTypes>
	// System.ArraySegment<Unity.Entities.LinkedEntityGroup>
	// System.ArraySegment<Unity.Entities.PerWorldSystemInfo>
	// System.ArraySegment<Unity.Entities.UpdateIndex>
	// System.ArraySegment<Unity.Mathematics.int3>
	// System.ArraySegment<Unity.Transforms.LocalTransform>
	// System.ArraySegment<int>
	// System.ArraySegment<object>
	// System.ByReference<GameFramework.ECS.Components.BuildingComponent>
	// System.ByReference<GameFramework.ECS.Components.GridCellData>
	// System.ByReference<GameFramework.ECS.Components.GridPositionComponent>
	// System.ByReference<GameFramework.ECS.Components.PathBufferElement>
	// System.ByReference<GameFramework.ECS.Components.PathfindingRequest>
	// System.ByReference<GameFramework.ECS.Components.PlaceObjectRequest>
	// System.ByReference<GameFramework.ECS.Components.PlacementStateComponent>
	// System.ByReference<GameFramework.ECS.Components.VisitedBuildingElement>
	// System.ByReference<GameFramework.ECS.Components.VisitorCenterComponent>
	// System.ByReference<GameFramework.ECS.Components.VisitorComponent>
	// System.ByReference<GameFramework.ECS.Systems.VisitorSpawningSystem.PendingSpawn>
	// System.ByReference<System.IntPtr>
	// System.ByReference<Unity.Entities.ComponentType>
	// System.ByReference<Unity.Entities.Entity>
	// System.ByReference<Unity.Entities.EntityQuery>
	// System.ByReference<Unity.Entities.EntityQueryBuilder.QueryTypes>
	// System.ByReference<Unity.Entities.LinkedEntityGroup>
	// System.ByReference<Unity.Entities.PerWorldSystemInfo>
	// System.ByReference<Unity.Entities.UpdateIndex>
	// System.ByReference<Unity.Mathematics.int3>
	// System.ByReference<Unity.Transforms.LocalTransform>
	// System.ByReference<int>
	// System.ByReference<object>
	// System.Collections.Generic.ArraySortHelper<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.Generic.ArraySortHelper<Unity.Entities.Entity>
	// System.Collections.Generic.ArraySortHelper<Unity.Mathematics.int3>
	// System.Collections.Generic.ArraySortHelper<int>
	// System.Collections.Generic.ArraySortHelper<object>
	// System.Collections.Generic.Comparer<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,Unity.Entities.Entity>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,object>>
	// System.Collections.Generic.Comparer<Unity.Entities.DynamicBuffer<GameFramework.ECS.Components.PathBufferElement>>
	// System.Collections.Generic.Comparer<Unity.Entities.DynamicBuffer<GameFramework.ECS.Components.VisitedBuildingElement>>
	// System.Collections.Generic.Comparer<Unity.Entities.Entity>
	// System.Collections.Generic.Comparer<Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRW<GameFramework.ECS.Components.GridPositionComponent>>
	// System.Collections.Generic.Comparer<Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRW<GameFramework.ECS.Components.PathfindingRequest>>
	// System.Collections.Generic.Comparer<Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRW<GameFramework.ECS.Components.VisitorComponent>>
	// System.Collections.Generic.Comparer<Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRW<Unity.Transforms.LocalTransform>>
	// System.Collections.Generic.Comparer<Unity.Mathematics.int3>
	// System.Collections.Generic.Comparer<byte>
	// System.Collections.Generic.Comparer<int>
	// System.Collections.Generic.Comparer<object>
	// System.Collections.Generic.ComparisonComparer<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,Unity.Entities.Entity>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,object>>
	// System.Collections.Generic.ComparisonComparer<Unity.Entities.DynamicBuffer<GameFramework.ECS.Components.PathBufferElement>>
	// System.Collections.Generic.ComparisonComparer<Unity.Entities.DynamicBuffer<GameFramework.ECS.Components.VisitedBuildingElement>>
	// System.Collections.Generic.ComparisonComparer<Unity.Entities.Entity>
	// System.Collections.Generic.ComparisonComparer<Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRW<GameFramework.ECS.Components.GridPositionComponent>>
	// System.Collections.Generic.ComparisonComparer<Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRW<GameFramework.ECS.Components.PathfindingRequest>>
	// System.Collections.Generic.ComparisonComparer<Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRW<GameFramework.ECS.Components.VisitorComponent>>
	// System.Collections.Generic.ComparisonComparer<Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRW<Unity.Transforms.LocalTransform>>
	// System.Collections.Generic.ComparisonComparer<Unity.Mathematics.int3>
	// System.Collections.Generic.ComparisonComparer<byte>
	// System.Collections.Generic.ComparisonComparer<int>
	// System.Collections.Generic.ComparisonComparer<object>
	// System.Collections.Generic.Dictionary.Enumerator<Unity.Mathematics.int3,Unity.Mathematics.int3>
	// System.Collections.Generic.Dictionary.Enumerator<Unity.Mathematics.int3,int>
	// System.Collections.Generic.Dictionary.Enumerator<int,Unity.Entities.Entity>
	// System.Collections.Generic.Dictionary.Enumerator<int,int>
	// System.Collections.Generic.Dictionary.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.Enumerator<object,Unity.Entities.SystemTypeIndex>
	// System.Collections.Generic.Dictionary.Enumerator<object,UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Collections.Generic.Dictionary.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<Unity.Mathematics.int3,Unity.Mathematics.int3>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<Unity.Mathematics.int3,int>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,Unity.Entities.Entity>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,int>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,Unity.Entities.SystemTypeIndex>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection<Unity.Mathematics.int3,Unity.Mathematics.int3>
	// System.Collections.Generic.Dictionary.KeyCollection<Unity.Mathematics.int3,int>
	// System.Collections.Generic.Dictionary.KeyCollection<int,Unity.Entities.Entity>
	// System.Collections.Generic.Dictionary.KeyCollection<int,int>
	// System.Collections.Generic.Dictionary.KeyCollection<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection<object,Unity.Entities.SystemTypeIndex>
	// System.Collections.Generic.Dictionary.KeyCollection<object,UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Collections.Generic.Dictionary.KeyCollection<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<Unity.Mathematics.int3,Unity.Mathematics.int3>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<Unity.Mathematics.int3,int>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,Unity.Entities.Entity>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,int>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,Unity.Entities.SystemTypeIndex>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection<Unity.Mathematics.int3,Unity.Mathematics.int3>
	// System.Collections.Generic.Dictionary.ValueCollection<Unity.Mathematics.int3,int>
	// System.Collections.Generic.Dictionary.ValueCollection<int,Unity.Entities.Entity>
	// System.Collections.Generic.Dictionary.ValueCollection<int,int>
	// System.Collections.Generic.Dictionary.ValueCollection<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection<object,Unity.Entities.SystemTypeIndex>
	// System.Collections.Generic.Dictionary.ValueCollection<object,UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Collections.Generic.Dictionary.ValueCollection<object,object>
	// System.Collections.Generic.Dictionary<Unity.Mathematics.int3,Unity.Mathematics.int3>
	// System.Collections.Generic.Dictionary<Unity.Mathematics.int3,int>
	// System.Collections.Generic.Dictionary<int,Unity.Entities.Entity>
	// System.Collections.Generic.Dictionary<int,int>
	// System.Collections.Generic.Dictionary<int,object>
	// System.Collections.Generic.Dictionary<object,Unity.Entities.SystemTypeIndex>
	// System.Collections.Generic.Dictionary<object,UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Collections.Generic.Dictionary<object,object>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,Unity.Entities.Entity>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,object>>
	// System.Collections.Generic.EqualityComparer<Unity.Entities.DynamicBuffer<GameFramework.ECS.Components.PathBufferElement>>
	// System.Collections.Generic.EqualityComparer<Unity.Entities.DynamicBuffer<GameFramework.ECS.Components.VisitedBuildingElement>>
	// System.Collections.Generic.EqualityComparer<Unity.Entities.Entity>
	// System.Collections.Generic.EqualityComparer<Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRW<GameFramework.ECS.Components.GridPositionComponent>>
	// System.Collections.Generic.EqualityComparer<Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRW<GameFramework.ECS.Components.PathfindingRequest>>
	// System.Collections.Generic.EqualityComparer<Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRW<GameFramework.ECS.Components.VisitorComponent>>
	// System.Collections.Generic.EqualityComparer<Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRW<Unity.Transforms.LocalTransform>>
	// System.Collections.Generic.EqualityComparer<Unity.Entities.SystemTypeIndex>
	// System.Collections.Generic.EqualityComparer<Unity.Mathematics.int3>
	// System.Collections.Generic.EqualityComparer<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Collections.Generic.EqualityComparer<byte>
	// System.Collections.Generic.EqualityComparer<int>
	// System.Collections.Generic.EqualityComparer<object>
	// System.Collections.Generic.HashSet.Enumerator<int>
	// System.Collections.Generic.HashSet.Enumerator<object>
	// System.Collections.Generic.HashSet<int>
	// System.Collections.Generic.HashSet<object>
	// System.Collections.Generic.HashSetEqualityComparer<int>
	// System.Collections.Generic.HashSetEqualityComparer<object>
	// System.Collections.Generic.ICollection<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<Unity.Mathematics.int3,Unity.Mathematics.int3>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<Unity.Mathematics.int3,int>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,Unity.Entities.Entity>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,Unity.Entities.SystemTypeIndex>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.ICollection<Unity.Entities.Entity>
	// System.Collections.Generic.ICollection<Unity.Mathematics.int3>
	// System.Collections.Generic.ICollection<int>
	// System.Collections.Generic.ICollection<object>
	// System.Collections.Generic.IComparer<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.Generic.IComparer<Unity.Entities.Entity>
	// System.Collections.Generic.IComparer<Unity.Mathematics.int3>
	// System.Collections.Generic.IComparer<int>
	// System.Collections.Generic.IComparer<object>
	// System.Collections.Generic.IEnumerable<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<Unity.Mathematics.int3,Unity.Mathematics.int3>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<Unity.Mathematics.int3,int>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,Unity.Entities.Entity>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,Unity.Entities.SystemTypeIndex>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerable<Unity.Entities.Entity>
	// System.Collections.Generic.IEnumerable<Unity.Mathematics.int3>
	// System.Collections.Generic.IEnumerable<int>
	// System.Collections.Generic.IEnumerable<object>
	// System.Collections.Generic.IEnumerator<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<Unity.Mathematics.int3,Unity.Mathematics.int3>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<Unity.Mathematics.int3,int>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,Unity.Entities.Entity>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,Unity.Entities.SystemTypeIndex>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerator<System.ValueTuple<Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRW<GameFramework.ECS.Components.PathfindingRequest>,Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRW<GameFramework.ECS.Components.VisitorComponent>,Unity.Entities.DynamicBuffer<GameFramework.ECS.Components.PathBufferElement>>>
	// System.Collections.Generic.IEnumerator<System.ValueTuple<Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRW<Unity.Transforms.LocalTransform>,Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRW<GameFramework.ECS.Components.VisitorComponent>,Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRW<GameFramework.ECS.Components.GridPositionComponent>,Unity.Entities.DynamicBuffer<GameFramework.ECS.Components.PathBufferElement>,Unity.Entities.DynamicBuffer<GameFramework.ECS.Components.VisitedBuildingElement>>>
	// System.Collections.Generic.IEnumerator<Unity.Entities.Entity>
	// System.Collections.Generic.IEnumerator<Unity.Entities.QueryEnumerableWithEntity<Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRW<GameFramework.ECS.Components.VisitorCenterComponent>,Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRO<GameFramework.ECS.Components.GridPositionComponent>>>
	// System.Collections.Generic.IEnumerator<Unity.Entities.QueryEnumerableWithEntity<Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRW<GameFramework.ECS.Components.VisitorComponent>,Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRO<GameFramework.ECS.Components.GridPositionComponent>,Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRW<GameFramework.ECS.Components.PathfindingRequest>,Unity.Entities.DynamicBuffer<GameFramework.ECS.Components.VisitedBuildingElement>>>
	// System.Collections.Generic.IEnumerator<Unity.Mathematics.int3>
	// System.Collections.Generic.IEnumerator<int>
	// System.Collections.Generic.IEnumerator<object>
	// System.Collections.Generic.IEqualityComparer<Unity.Mathematics.int3>
	// System.Collections.Generic.IEqualityComparer<int>
	// System.Collections.Generic.IEqualityComparer<object>
	// System.Collections.Generic.IList<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.Generic.IList<Unity.Entities.Entity>
	// System.Collections.Generic.IList<Unity.Mathematics.int3>
	// System.Collections.Generic.IList<int>
	// System.Collections.Generic.IList<object>
	// System.Collections.Generic.IReadOnlyDictionary<Unity.Mathematics.int3,int>
	// System.Collections.Generic.IReadOnlyDictionary<int,int>
	// System.Collections.Generic.KeyValuePair<Unity.Mathematics.int3,Unity.Mathematics.int3>
	// System.Collections.Generic.KeyValuePair<Unity.Mathematics.int3,int>
	// System.Collections.Generic.KeyValuePair<int,Unity.Entities.Entity>
	// System.Collections.Generic.KeyValuePair<int,int>
	// System.Collections.Generic.KeyValuePair<int,object>
	// System.Collections.Generic.KeyValuePair<object,Unity.Entities.SystemTypeIndex>
	// System.Collections.Generic.KeyValuePair<object,UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Collections.Generic.KeyValuePair<object,object>
	// System.Collections.Generic.LinkedList.Enumerator<object>
	// System.Collections.Generic.LinkedList<object>
	// System.Collections.Generic.LinkedListNode<object>
	// System.Collections.Generic.List.Enumerator<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.Generic.List.Enumerator<Unity.Entities.Entity>
	// System.Collections.Generic.List.Enumerator<Unity.Mathematics.int3>
	// System.Collections.Generic.List.Enumerator<int>
	// System.Collections.Generic.List.Enumerator<object>
	// System.Collections.Generic.List<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.Generic.List<Unity.Entities.Entity>
	// System.Collections.Generic.List<Unity.Mathematics.int3>
	// System.Collections.Generic.List<int>
	// System.Collections.Generic.List<object>
	// System.Collections.Generic.ObjectComparer<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,Unity.Entities.Entity>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,object>>
	// System.Collections.Generic.ObjectComparer<Unity.Entities.DynamicBuffer<GameFramework.ECS.Components.PathBufferElement>>
	// System.Collections.Generic.ObjectComparer<Unity.Entities.DynamicBuffer<GameFramework.ECS.Components.VisitedBuildingElement>>
	// System.Collections.Generic.ObjectComparer<Unity.Entities.Entity>
	// System.Collections.Generic.ObjectComparer<Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRW<GameFramework.ECS.Components.GridPositionComponent>>
	// System.Collections.Generic.ObjectComparer<Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRW<GameFramework.ECS.Components.PathfindingRequest>>
	// System.Collections.Generic.ObjectComparer<Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRW<GameFramework.ECS.Components.VisitorComponent>>
	// System.Collections.Generic.ObjectComparer<Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRW<Unity.Transforms.LocalTransform>>
	// System.Collections.Generic.ObjectComparer<Unity.Mathematics.int3>
	// System.Collections.Generic.ObjectComparer<byte>
	// System.Collections.Generic.ObjectComparer<int>
	// System.Collections.Generic.ObjectComparer<object>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,Unity.Entities.Entity>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,object>>
	// System.Collections.Generic.ObjectEqualityComparer<Unity.Entities.DynamicBuffer<GameFramework.ECS.Components.PathBufferElement>>
	// System.Collections.Generic.ObjectEqualityComparer<Unity.Entities.DynamicBuffer<GameFramework.ECS.Components.VisitedBuildingElement>>
	// System.Collections.Generic.ObjectEqualityComparer<Unity.Entities.Entity>
	// System.Collections.Generic.ObjectEqualityComparer<Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRW<GameFramework.ECS.Components.GridPositionComponent>>
	// System.Collections.Generic.ObjectEqualityComparer<Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRW<GameFramework.ECS.Components.PathfindingRequest>>
	// System.Collections.Generic.ObjectEqualityComparer<Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRW<GameFramework.ECS.Components.VisitorComponent>>
	// System.Collections.Generic.ObjectEqualityComparer<Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRW<Unity.Transforms.LocalTransform>>
	// System.Collections.Generic.ObjectEqualityComparer<Unity.Entities.SystemTypeIndex>
	// System.Collections.Generic.ObjectEqualityComparer<Unity.Mathematics.int3>
	// System.Collections.Generic.ObjectEqualityComparer<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Collections.Generic.ObjectEqualityComparer<byte>
	// System.Collections.Generic.ObjectEqualityComparer<int>
	// System.Collections.Generic.ObjectEqualityComparer<object>
	// System.Collections.ObjectModel.ReadOnlyCollection<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.ObjectModel.ReadOnlyCollection<Unity.Entities.Entity>
	// System.Collections.ObjectModel.ReadOnlyCollection<Unity.Mathematics.int3>
	// System.Collections.ObjectModel.ReadOnlyCollection<int>
	// System.Collections.ObjectModel.ReadOnlyCollection<object>
	// System.Comparison<Cysharp.Threading.Tasks.UniTask>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Comparison<System.ValueTuple<byte,Unity.Entities.Entity>>
	// System.Comparison<System.ValueTuple<byte,object>>
	// System.Comparison<Unity.Entities.DynamicBuffer<GameFramework.ECS.Components.PathBufferElement>>
	// System.Comparison<Unity.Entities.DynamicBuffer<GameFramework.ECS.Components.VisitedBuildingElement>>
	// System.Comparison<Unity.Entities.Entity>
	// System.Comparison<Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRW<GameFramework.ECS.Components.GridPositionComponent>>
	// System.Comparison<Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRW<GameFramework.ECS.Components.PathfindingRequest>>
	// System.Comparison<Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRW<GameFramework.ECS.Components.VisitorComponent>>
	// System.Comparison<Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRW<Unity.Transforms.LocalTransform>>
	// System.Comparison<Unity.Mathematics.int3>
	// System.Comparison<byte>
	// System.Comparison<int>
	// System.Comparison<object>
	// System.Func<Cysharp.Threading.Tasks.UniTask>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Func<System.ValueTuple<byte,Unity.Entities.Entity>>
	// System.Func<System.ValueTuple<byte,object>>
	// System.Func<Unity.Entities.Entity>
	// System.Func<byte>
	// System.Func<int>
	// System.Func<object,Cysharp.Threading.Tasks.UniTask<object>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Func<object,System.ValueTuple<byte,Unity.Entities.Entity>>
	// System.Func<object,System.ValueTuple<byte,object>>
	// System.Func<object,Unity.Entities.Entity>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Func<object,object,System.ValueTuple<byte,Unity.Entities.Entity>>
	// System.Func<object,object,System.ValueTuple<byte,object>>
	// System.Func<object,object,Unity.Entities.Entity>
	// System.Func<object,object,object>
	// System.Func<object,object>
	// System.Func<object>
	// System.IEquatable<Unity.Mathematics.int3>
	// System.Predicate<Cysharp.Threading.Tasks.UniTask>
	// System.Predicate<Unity.Entities.Entity>
	// System.Predicate<Unity.Mathematics.int3>
	// System.Predicate<int>
	// System.Predicate<object>
	// System.ReadOnlySpan.Enumerator<GameFramework.ECS.Components.BuildingComponent>
	// System.ReadOnlySpan.Enumerator<GameFramework.ECS.Components.GridCellData>
	// System.ReadOnlySpan.Enumerator<GameFramework.ECS.Components.GridPositionComponent>
	// System.ReadOnlySpan.Enumerator<GameFramework.ECS.Components.PathBufferElement>
	// System.ReadOnlySpan.Enumerator<GameFramework.ECS.Components.PathfindingRequest>
	// System.ReadOnlySpan.Enumerator<GameFramework.ECS.Components.PlaceObjectRequest>
	// System.ReadOnlySpan.Enumerator<GameFramework.ECS.Components.PlacementStateComponent>
	// System.ReadOnlySpan.Enumerator<GameFramework.ECS.Components.VisitedBuildingElement>
	// System.ReadOnlySpan.Enumerator<GameFramework.ECS.Components.VisitorCenterComponent>
	// System.ReadOnlySpan.Enumerator<GameFramework.ECS.Components.VisitorComponent>
	// System.ReadOnlySpan.Enumerator<GameFramework.ECS.Systems.VisitorSpawningSystem.PendingSpawn>
	// System.ReadOnlySpan.Enumerator<System.IntPtr>
	// System.ReadOnlySpan.Enumerator<Unity.Entities.ComponentType>
	// System.ReadOnlySpan.Enumerator<Unity.Entities.Entity>
	// System.ReadOnlySpan.Enumerator<Unity.Entities.EntityQuery>
	// System.ReadOnlySpan.Enumerator<Unity.Entities.EntityQueryBuilder.QueryTypes>
	// System.ReadOnlySpan.Enumerator<Unity.Entities.LinkedEntityGroup>
	// System.ReadOnlySpan.Enumerator<Unity.Entities.PerWorldSystemInfo>
	// System.ReadOnlySpan.Enumerator<Unity.Entities.UpdateIndex>
	// System.ReadOnlySpan.Enumerator<Unity.Mathematics.int3>
	// System.ReadOnlySpan.Enumerator<Unity.Transforms.LocalTransform>
	// System.ReadOnlySpan.Enumerator<int>
	// System.ReadOnlySpan.Enumerator<object>
	// System.ReadOnlySpan<GameFramework.ECS.Components.BuildingComponent>
	// System.ReadOnlySpan<GameFramework.ECS.Components.GridCellData>
	// System.ReadOnlySpan<GameFramework.ECS.Components.GridPositionComponent>
	// System.ReadOnlySpan<GameFramework.ECS.Components.PathBufferElement>
	// System.ReadOnlySpan<GameFramework.ECS.Components.PathfindingRequest>
	// System.ReadOnlySpan<GameFramework.ECS.Components.PlaceObjectRequest>
	// System.ReadOnlySpan<GameFramework.ECS.Components.PlacementStateComponent>
	// System.ReadOnlySpan<GameFramework.ECS.Components.VisitedBuildingElement>
	// System.ReadOnlySpan<GameFramework.ECS.Components.VisitorCenterComponent>
	// System.ReadOnlySpan<GameFramework.ECS.Components.VisitorComponent>
	// System.ReadOnlySpan<GameFramework.ECS.Systems.VisitorSpawningSystem.PendingSpawn>
	// System.ReadOnlySpan<System.IntPtr>
	// System.ReadOnlySpan<Unity.Entities.ComponentType>
	// System.ReadOnlySpan<Unity.Entities.Entity>
	// System.ReadOnlySpan<Unity.Entities.EntityQuery>
	// System.ReadOnlySpan<Unity.Entities.EntityQueryBuilder.QueryTypes>
	// System.ReadOnlySpan<Unity.Entities.LinkedEntityGroup>
	// System.ReadOnlySpan<Unity.Entities.PerWorldSystemInfo>
	// System.ReadOnlySpan<Unity.Entities.UpdateIndex>
	// System.ReadOnlySpan<Unity.Mathematics.int3>
	// System.ReadOnlySpan<Unity.Transforms.LocalTransform>
	// System.ReadOnlySpan<int>
	// System.ReadOnlySpan<object>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,Unity.Entities.Entity>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,object>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<Unity.Entities.Entity>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,Unity.Entities.Entity>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,object>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<Unity.Entities.Entity>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<object>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,Unity.Entities.Entity>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,object>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<Unity.Entities.Entity>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<object>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,Unity.Entities.Entity>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,object>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<Unity.Entities.Entity>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<object>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,Unity.Entities.Entity>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,object>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<Unity.Entities.Entity>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<object>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,Unity.Entities.Entity>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,object>>
	// System.Runtime.CompilerServices.TaskAwaiter<Unity.Entities.Entity>
	// System.Runtime.CompilerServices.TaskAwaiter<object>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,Unity.Entities.Entity>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,object>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<Unity.Entities.Entity>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<object>
	// System.Span.Enumerator<GameFramework.ECS.Components.BuildingComponent>
	// System.Span.Enumerator<GameFramework.ECS.Components.GridCellData>
	// System.Span.Enumerator<GameFramework.ECS.Components.GridPositionComponent>
	// System.Span.Enumerator<GameFramework.ECS.Components.PathBufferElement>
	// System.Span.Enumerator<GameFramework.ECS.Components.PathfindingRequest>
	// System.Span.Enumerator<GameFramework.ECS.Components.PlaceObjectRequest>
	// System.Span.Enumerator<GameFramework.ECS.Components.PlacementStateComponent>
	// System.Span.Enumerator<GameFramework.ECS.Components.VisitedBuildingElement>
	// System.Span.Enumerator<GameFramework.ECS.Components.VisitorCenterComponent>
	// System.Span.Enumerator<GameFramework.ECS.Components.VisitorComponent>
	// System.Span.Enumerator<GameFramework.ECS.Systems.VisitorSpawningSystem.PendingSpawn>
	// System.Span.Enumerator<System.IntPtr>
	// System.Span.Enumerator<Unity.Entities.ComponentType>
	// System.Span.Enumerator<Unity.Entities.Entity>
	// System.Span.Enumerator<Unity.Entities.EntityQuery>
	// System.Span.Enumerator<Unity.Entities.EntityQueryBuilder.QueryTypes>
	// System.Span.Enumerator<Unity.Entities.LinkedEntityGroup>
	// System.Span.Enumerator<Unity.Entities.PerWorldSystemInfo>
	// System.Span.Enumerator<Unity.Entities.UpdateIndex>
	// System.Span.Enumerator<Unity.Mathematics.int3>
	// System.Span.Enumerator<Unity.Transforms.LocalTransform>
	// System.Span.Enumerator<int>
	// System.Span.Enumerator<object>
	// System.Span<GameFramework.ECS.Components.BuildingComponent>
	// System.Span<GameFramework.ECS.Components.GridCellData>
	// System.Span<GameFramework.ECS.Components.GridPositionComponent>
	// System.Span<GameFramework.ECS.Components.PathBufferElement>
	// System.Span<GameFramework.ECS.Components.PathfindingRequest>
	// System.Span<GameFramework.ECS.Components.PlaceObjectRequest>
	// System.Span<GameFramework.ECS.Components.PlacementStateComponent>
	// System.Span<GameFramework.ECS.Components.VisitedBuildingElement>
	// System.Span<GameFramework.ECS.Components.VisitorCenterComponent>
	// System.Span<GameFramework.ECS.Components.VisitorComponent>
	// System.Span<GameFramework.ECS.Systems.VisitorSpawningSystem.PendingSpawn>
	// System.Span<System.IntPtr>
	// System.Span<Unity.Entities.ComponentType>
	// System.Span<Unity.Entities.Entity>
	// System.Span<Unity.Entities.EntityQuery>
	// System.Span<Unity.Entities.EntityQueryBuilder.QueryTypes>
	// System.Span<Unity.Entities.LinkedEntityGroup>
	// System.Span<Unity.Entities.PerWorldSystemInfo>
	// System.Span<Unity.Entities.UpdateIndex>
	// System.Span<Unity.Mathematics.int3>
	// System.Span<Unity.Transforms.LocalTransform>
	// System.Span<int>
	// System.Span<object>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,Unity.Entities.Entity>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,object>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<Unity.Entities.Entity>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<object>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,Unity.Entities.Entity>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,object>>
	// System.Threading.Tasks.Sources.IValueTaskSource<Unity.Entities.Entity>
	// System.Threading.Tasks.Sources.IValueTaskSource<object>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,Unity.Entities.Entity>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,object>>
	// System.Threading.Tasks.Task<Unity.Entities.Entity>
	// System.Threading.Tasks.Task<object>
	// System.Threading.Tasks.TaskCompletionSource<object>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,Unity.Entities.Entity>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,object>>
	// System.Threading.Tasks.TaskFactory.<>c<Unity.Entities.Entity>
	// System.Threading.Tasks.TaskFactory.<>c<object>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,Unity.Entities.Entity>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,object>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<Unity.Entities.Entity>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<object>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,Unity.Entities.Entity>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,object>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<Unity.Entities.Entity>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<object>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,Unity.Entities.Entity>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,object>>
	// System.Threading.Tasks.TaskFactory<Unity.Entities.Entity>
	// System.Threading.Tasks.TaskFactory<object>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,Unity.Entities.Entity>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,object>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<Unity.Entities.Entity>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<object>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,Unity.Entities.Entity>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,object>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<Unity.Entities.Entity>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<object>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,Unity.Entities.Entity>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,object>>
	// System.Threading.Tasks.ValueTask<Unity.Entities.Entity>
	// System.Threading.Tasks.ValueTask<object>
	// System.ValueTuple<Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRW<GameFramework.ECS.Components.PathfindingRequest>,Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRW<GameFramework.ECS.Components.VisitorComponent>,Unity.Entities.DynamicBuffer<GameFramework.ECS.Components.PathBufferElement>>
	// System.ValueTuple<Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRW<Unity.Transforms.LocalTransform>,Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRW<GameFramework.ECS.Components.VisitorComponent>,Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRW<GameFramework.ECS.Components.GridPositionComponent>,Unity.Entities.DynamicBuffer<GameFramework.ECS.Components.PathBufferElement>,Unity.Entities.DynamicBuffer<GameFramework.ECS.Components.VisitedBuildingElement>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>
	// System.ValueTuple<byte,System.ValueTuple<byte,object>>
	// System.ValueTuple<byte,Unity.Entities.Entity>
	// System.ValueTuple<byte,object>
	// Unity.Burst.SharedStatic<Unity.Collections.Long1024>
	// Unity.Burst.SharedStatic<Unity.Entities.TypeIndex>
	// Unity.Burst.SharedStatic<int>
	// Unity.Collections.IIndexable<byte>
	// Unity.Collections.INativeList<byte>
	// Unity.Collections.LowLevel.Unsafe.KeyValue<Unity.Mathematics.int3,GameFramework.ECS.Components.GridCellData>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList.ParallelReader<GameFramework.ECS.Systems.VisitorSpawningSystem.PendingSpawn>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList.ParallelReader<System.IntPtr>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList.ParallelReader<Unity.Entities.ComponentType>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList.ParallelReader<Unity.Entities.Entity>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList.ParallelReader<Unity.Entities.EntityQuery>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList.ParallelReader<Unity.Entities.EntityQueryBuilder.QueryTypes>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList.ParallelReader<Unity.Entities.PerWorldSystemInfo>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList.ParallelReader<Unity.Entities.UpdateIndex>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList.ParallelReader<int>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList.ParallelWriter<GameFramework.ECS.Systems.VisitorSpawningSystem.PendingSpawn>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList.ParallelWriter<System.IntPtr>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList.ParallelWriter<Unity.Entities.ComponentType>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList.ParallelWriter<Unity.Entities.Entity>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList.ParallelWriter<Unity.Entities.EntityQuery>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList.ParallelWriter<Unity.Entities.EntityQueryBuilder.QueryTypes>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList.ParallelWriter<Unity.Entities.PerWorldSystemInfo>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList.ParallelWriter<Unity.Entities.UpdateIndex>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList.ParallelWriter<int>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList.ReadOnly<GameFramework.ECS.Systems.VisitorSpawningSystem.PendingSpawn>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList.ReadOnly<System.IntPtr>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList.ReadOnly<Unity.Entities.ComponentType>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList.ReadOnly<Unity.Entities.Entity>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList.ReadOnly<Unity.Entities.EntityQuery>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList.ReadOnly<Unity.Entities.EntityQueryBuilder.QueryTypes>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList.ReadOnly<Unity.Entities.PerWorldSystemInfo>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList.ReadOnly<Unity.Entities.UpdateIndex>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList.ReadOnly<int>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList<GameFramework.ECS.Systems.VisitorSpawningSystem.PendingSpawn>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList<System.IntPtr>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList<Unity.Entities.ComponentType>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList<Unity.Entities.Entity>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList<Unity.Entities.EntityQuery>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList<Unity.Entities.EntityQueryBuilder.QueryTypes>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList<Unity.Entities.PerWorldSystemInfo>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList<Unity.Entities.UpdateIndex>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList<int>
	// Unity.Collections.LowLevel.Unsafe.UnsafeParallelHashMap.ReadOnly<Unity.Mathematics.int3,GameFramework.ECS.Components.GridCellData>
	// Unity.Collections.LowLevel.Unsafe.UnsafeParallelHashMap.ReadOnly<Unity.Mathematics.int3,Unity.Entities.Entity>
	// Unity.Collections.LowLevel.Unsafe.UnsafeParallelHashMap<Unity.Mathematics.int3,GameFramework.ECS.Components.GridCellData>
	// Unity.Collections.LowLevel.Unsafe.UnsafeParallelHashMap<Unity.Mathematics.int3,Unity.Entities.Entity>
	// Unity.Collections.LowLevel.Unsafe.UnsafeParallelHashMapBase<Unity.Mathematics.int3,GameFramework.ECS.Components.GridCellData>
	// Unity.Collections.LowLevel.Unsafe.UnsafeParallelHashMapBase<Unity.Mathematics.int3,Unity.Entities.Entity>
	// Unity.Collections.LowLevel.Unsafe.UnsafePtrList.ParallelReader<Unity.Entities.Archetype>
	// Unity.Collections.LowLevel.Unsafe.UnsafePtrList.ParallelWriter<Unity.Entities.Archetype>
	// Unity.Collections.LowLevel.Unsafe.UnsafePtrList.ReadOnly<Unity.Entities.Archetype>
	// Unity.Collections.LowLevel.Unsafe.UnsafePtrList<Unity.Entities.Archetype>
	// Unity.Collections.NativeArray.Enumerator<GameFramework.ECS.Components.BuildingComponent>
	// Unity.Collections.NativeArray.Enumerator<GameFramework.ECS.Components.GridCellData>
	// Unity.Collections.NativeArray.Enumerator<GameFramework.ECS.Components.GridPositionComponent>
	// Unity.Collections.NativeArray.Enumerator<GameFramework.ECS.Components.PathBufferElement>
	// Unity.Collections.NativeArray.Enumerator<GameFramework.ECS.Components.PathfindingRequest>
	// Unity.Collections.NativeArray.Enumerator<GameFramework.ECS.Components.PlaceObjectRequest>
	// Unity.Collections.NativeArray.Enumerator<GameFramework.ECS.Components.PlacementStateComponent>
	// Unity.Collections.NativeArray.Enumerator<GameFramework.ECS.Components.VisitedBuildingElement>
	// Unity.Collections.NativeArray.Enumerator<GameFramework.ECS.Components.VisitorCenterComponent>
	// Unity.Collections.NativeArray.Enumerator<GameFramework.ECS.Components.VisitorComponent>
	// Unity.Collections.NativeArray.Enumerator<GameFramework.ECS.Systems.VisitorSpawningSystem.PendingSpawn>
	// Unity.Collections.NativeArray.Enumerator<System.IntPtr>
	// Unity.Collections.NativeArray.Enumerator<Unity.Entities.ComponentType>
	// Unity.Collections.NativeArray.Enumerator<Unity.Entities.Entity>
	// Unity.Collections.NativeArray.Enumerator<Unity.Entities.EntityQuery>
	// Unity.Collections.NativeArray.Enumerator<Unity.Entities.EntityQueryBuilder.QueryTypes>
	// Unity.Collections.NativeArray.Enumerator<Unity.Entities.LinkedEntityGroup>
	// Unity.Collections.NativeArray.Enumerator<Unity.Entities.PerWorldSystemInfo>
	// Unity.Collections.NativeArray.Enumerator<Unity.Entities.UpdateIndex>
	// Unity.Collections.NativeArray.Enumerator<Unity.Mathematics.int3>
	// Unity.Collections.NativeArray.Enumerator<Unity.Transforms.LocalTransform>
	// Unity.Collections.NativeArray.Enumerator<int>
	// Unity.Collections.NativeArray.Enumerator<object>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<GameFramework.ECS.Components.BuildingComponent>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<GameFramework.ECS.Components.GridCellData>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<GameFramework.ECS.Components.GridPositionComponent>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<GameFramework.ECS.Components.PathBufferElement>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<GameFramework.ECS.Components.PathfindingRequest>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<GameFramework.ECS.Components.PlaceObjectRequest>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<GameFramework.ECS.Components.PlacementStateComponent>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<GameFramework.ECS.Components.VisitedBuildingElement>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<GameFramework.ECS.Components.VisitorCenterComponent>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<GameFramework.ECS.Components.VisitorComponent>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<GameFramework.ECS.Systems.VisitorSpawningSystem.PendingSpawn>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<System.IntPtr>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<Unity.Entities.ComponentType>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<Unity.Entities.Entity>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<Unity.Entities.EntityQuery>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<Unity.Entities.EntityQueryBuilder.QueryTypes>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<Unity.Entities.LinkedEntityGroup>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<Unity.Entities.PerWorldSystemInfo>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<Unity.Entities.UpdateIndex>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<Unity.Mathematics.int3>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<Unity.Transforms.LocalTransform>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<int>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<object>
	// Unity.Collections.NativeArray.ReadOnly<GameFramework.ECS.Components.BuildingComponent>
	// Unity.Collections.NativeArray.ReadOnly<GameFramework.ECS.Components.GridCellData>
	// Unity.Collections.NativeArray.ReadOnly<GameFramework.ECS.Components.GridPositionComponent>
	// Unity.Collections.NativeArray.ReadOnly<GameFramework.ECS.Components.PathBufferElement>
	// Unity.Collections.NativeArray.ReadOnly<GameFramework.ECS.Components.PathfindingRequest>
	// Unity.Collections.NativeArray.ReadOnly<GameFramework.ECS.Components.PlaceObjectRequest>
	// Unity.Collections.NativeArray.ReadOnly<GameFramework.ECS.Components.PlacementStateComponent>
	// Unity.Collections.NativeArray.ReadOnly<GameFramework.ECS.Components.VisitedBuildingElement>
	// Unity.Collections.NativeArray.ReadOnly<GameFramework.ECS.Components.VisitorCenterComponent>
	// Unity.Collections.NativeArray.ReadOnly<GameFramework.ECS.Components.VisitorComponent>
	// Unity.Collections.NativeArray.ReadOnly<GameFramework.ECS.Systems.VisitorSpawningSystem.PendingSpawn>
	// Unity.Collections.NativeArray.ReadOnly<System.IntPtr>
	// Unity.Collections.NativeArray.ReadOnly<Unity.Entities.ComponentType>
	// Unity.Collections.NativeArray.ReadOnly<Unity.Entities.Entity>
	// Unity.Collections.NativeArray.ReadOnly<Unity.Entities.EntityQuery>
	// Unity.Collections.NativeArray.ReadOnly<Unity.Entities.EntityQueryBuilder.QueryTypes>
	// Unity.Collections.NativeArray.ReadOnly<Unity.Entities.LinkedEntityGroup>
	// Unity.Collections.NativeArray.ReadOnly<Unity.Entities.PerWorldSystemInfo>
	// Unity.Collections.NativeArray.ReadOnly<Unity.Entities.UpdateIndex>
	// Unity.Collections.NativeArray.ReadOnly<Unity.Mathematics.int3>
	// Unity.Collections.NativeArray.ReadOnly<Unity.Transforms.LocalTransform>
	// Unity.Collections.NativeArray.ReadOnly<int>
	// Unity.Collections.NativeArray.ReadOnly<object>
	// Unity.Collections.NativeArray<GameFramework.ECS.Components.BuildingComponent>
	// Unity.Collections.NativeArray<GameFramework.ECS.Components.GridCellData>
	// Unity.Collections.NativeArray<GameFramework.ECS.Components.GridPositionComponent>
	// Unity.Collections.NativeArray<GameFramework.ECS.Components.PathBufferElement>
	// Unity.Collections.NativeArray<GameFramework.ECS.Components.PathfindingRequest>
	// Unity.Collections.NativeArray<GameFramework.ECS.Components.PlaceObjectRequest>
	// Unity.Collections.NativeArray<GameFramework.ECS.Components.PlacementStateComponent>
	// Unity.Collections.NativeArray<GameFramework.ECS.Components.VisitedBuildingElement>
	// Unity.Collections.NativeArray<GameFramework.ECS.Components.VisitorCenterComponent>
	// Unity.Collections.NativeArray<GameFramework.ECS.Components.VisitorComponent>
	// Unity.Collections.NativeArray<GameFramework.ECS.Systems.VisitorSpawningSystem.PendingSpawn>
	// Unity.Collections.NativeArray<System.IntPtr>
	// Unity.Collections.NativeArray<Unity.Entities.ComponentType>
	// Unity.Collections.NativeArray<Unity.Entities.Entity>
	// Unity.Collections.NativeArray<Unity.Entities.EntityQuery>
	// Unity.Collections.NativeArray<Unity.Entities.EntityQueryBuilder.QueryTypes>
	// Unity.Collections.NativeArray<Unity.Entities.LinkedEntityGroup>
	// Unity.Collections.NativeArray<Unity.Entities.PerWorldSystemInfo>
	// Unity.Collections.NativeArray<Unity.Entities.UpdateIndex>
	// Unity.Collections.NativeArray<Unity.Mathematics.int3>
	// Unity.Collections.NativeArray<Unity.Transforms.LocalTransform>
	// Unity.Collections.NativeArray<int>
	// Unity.Collections.NativeArray<object>
	// Unity.Collections.NativeKeyValueArrays<Unity.Mathematics.int3,GameFramework.ECS.Components.GridCellData>
	// Unity.Collections.NativeKeyValueArrays<Unity.Mathematics.int3,Unity.Entities.Entity>
	// Unity.Collections.NativeList.ParallelWriter<GameFramework.ECS.Systems.VisitorSpawningSystem.PendingSpawn>
	// Unity.Collections.NativeList.ParallelWriter<Unity.Entities.Entity>
	// Unity.Collections.NativeList.ParallelWriter<int>
	// Unity.Collections.NativeList<GameFramework.ECS.Systems.VisitorSpawningSystem.PendingSpawn>
	// Unity.Collections.NativeList<Unity.Entities.Entity>
	// Unity.Collections.NativeList<int>
	// Unity.Collections.NativeParallelHashMap.Enumerator<Unity.Mathematics.int3,GameFramework.ECS.Components.GridCellData>
	// Unity.Collections.NativeParallelHashMap.ReadOnly<Unity.Mathematics.int3,GameFramework.ECS.Components.GridCellData>
	// Unity.Collections.NativeParallelHashMap.ReadOnly<Unity.Mathematics.int3,Unity.Entities.Entity>
	// Unity.Collections.NativeParallelHashMap<Unity.Mathematics.int3,GameFramework.ECS.Components.GridCellData>
	// Unity.Collections.NativeParallelHashMap<Unity.Mathematics.int3,Unity.Entities.Entity>
	// Unity.Collections.NativeSlice.Enumerator<GameFramework.ECS.Components.PathBufferElement>
	// Unity.Collections.NativeSlice.Enumerator<GameFramework.ECS.Components.VisitedBuildingElement>
	// Unity.Collections.NativeSlice.Enumerator<Unity.Entities.LinkedEntityGroup>
	// Unity.Collections.NativeSlice.Enumerator<object>
	// Unity.Collections.NativeSlice<GameFramework.ECS.Components.PathBufferElement>
	// Unity.Collections.NativeSlice<GameFramework.ECS.Components.VisitedBuildingElement>
	// Unity.Collections.NativeSlice<Unity.Entities.LinkedEntityGroup>
	// Unity.Collections.NativeSlice<object>
	// Unity.Entities.BufferAccessor<GameFramework.ECS.Components.PathBufferElement>
	// Unity.Entities.BufferAccessor<GameFramework.ECS.Components.VisitedBuildingElement>
	// Unity.Entities.BufferTypeHandle<GameFramework.ECS.Components.PathBufferElement>
	// Unity.Entities.BufferTypeHandle<GameFramework.ECS.Components.VisitedBuildingElement>
	// Unity.Entities.ComponentTypeHandle<GameFramework.ECS.Components.BuildingComponent>
	// Unity.Entities.ComponentTypeHandle<GameFramework.ECS.Components.GridPositionComponent>
	// Unity.Entities.ComponentTypeHandle<GameFramework.ECS.Components.PathfindingRequest>
	// Unity.Entities.ComponentTypeHandle<GameFramework.ECS.Components.PlaceObjectRequest>
	// Unity.Entities.ComponentTypeHandle<GameFramework.ECS.Components.VisitorCenterComponent>
	// Unity.Entities.ComponentTypeHandle<GameFramework.ECS.Components.VisitorComponent>
	// Unity.Entities.ComponentTypeHandle<Unity.Transforms.LocalTransform>
	// Unity.Entities.DynamicBuffer<GameFramework.ECS.Components.PathBufferElement>
	// Unity.Entities.DynamicBuffer<GameFramework.ECS.Components.VisitedBuildingElement>
	// Unity.Entities.DynamicBuffer<Unity.Entities.LinkedEntityGroup>
	// Unity.Entities.DynamicBuffer<object>
	// Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRO<GameFramework.ECS.Components.GridPositionComponent>
	// Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRW<GameFramework.ECS.Components.GridPositionComponent>
	// Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRW<GameFramework.ECS.Components.PathfindingRequest>
	// Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRW<GameFramework.ECS.Components.VisitorCenterComponent>
	// Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRW<GameFramework.ECS.Components.VisitorComponent>
	// Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRW<Unity.Transforms.LocalTransform>
	// Unity.Entities.QueryEnumerableWithEntity<Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRW<GameFramework.ECS.Components.VisitorCenterComponent>,Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRO<GameFramework.ECS.Components.GridPositionComponent>>
	// Unity.Entities.QueryEnumerableWithEntity<Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRW<GameFramework.ECS.Components.VisitorComponent>,Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRO<GameFramework.ECS.Components.GridPositionComponent>,Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRW<GameFramework.ECS.Components.PathfindingRequest>,Unity.Entities.DynamicBuffer<GameFramework.ECS.Components.VisitedBuildingElement>>
	// Unity.Entities.RefRO<GameFramework.ECS.Components.GridPositionComponent>
	// Unity.Entities.RefRO<GameFramework.ECS.Components.PathfindingRequest>
	// Unity.Entities.RefRO<GameFramework.ECS.Components.PlacementStateComponent>
	// Unity.Entities.RefRO<GameFramework.ECS.Components.VisitorCenterComponent>
	// Unity.Entities.RefRO<GameFramework.ECS.Components.VisitorComponent>
	// Unity.Entities.RefRO<Unity.Transforms.LocalTransform>
	// Unity.Entities.RefRW<GameFramework.ECS.Components.GridPositionComponent>
	// Unity.Entities.RefRW<GameFramework.ECS.Components.PathfindingRequest>
	// Unity.Entities.RefRW<GameFramework.ECS.Components.PlacementStateComponent>
	// Unity.Entities.RefRW<GameFramework.ECS.Components.VisitorCenterComponent>
	// Unity.Entities.RefRW<GameFramework.ECS.Components.VisitorComponent>
	// Unity.Entities.RefRW<Unity.Transforms.LocalTransform>
	// Unity.Entities.TypeManager.SharedTypeIndex<object>
	// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationBase.<>c__DisplayClass60_0<object>
	// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationBase.<>c__DisplayClass61_0<object>
	// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationBase<object>
	// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle.<>c<object>
	// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object>
	// UnityEngine.ResourceManagement.Util.GlobalLinkedListNodeCache<object>
	// UnityEngine.ResourceManagement.Util.LinkedListNodeCache<object>
	// }}

	public void RefMethods()
	{
		// object Cinemachine.CinemachineVirtualCamera.AddCinemachineComponent<object>()
		// object Cinemachine.CinemachineVirtualCamera.GetCinemachineComponent<object>()
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,object>(Cysharp.Threading.Tasks.UniTask.Awaiter&,object&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,object>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,object&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<Unity.Entities.Entity>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<Unity.Entities.Entity>,object>(Cysharp.Threading.Tasks.UniTask.Awaiter<Unity.Entities.Entity>&,object&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<Unity.Entities.Entity>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,object>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,object&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,object>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,object&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<object>(object&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<Unity.Entities.Entity>.Start<object>(object&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<object>(object&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,object>(Cysharp.Threading.Tasks.UniTask.Awaiter&,object&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<Unity.Entities.Entity>,object>(Cysharp.Threading.Tasks.UniTask.Awaiter<Unity.Entities.Entity>&,object&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,object>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,object&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<object>(object&)
		// object DG.Tweening.TweenSettingsExtensions.SetLoops<object>(object,int,DG.Tweening.LoopType)
		// Cysharp.Threading.Tasks.UniTask<object> GameFramework.Managers.ResourceManager.LoadAssetAsync<object>(string)
		// Cysharp.Threading.Tasks.UniTask<object> GameFramework.Managers.UIManager.ShowPanelAsync<object>(string,GameFramework.Managers.UILayer)
		// string Luban.StringUtil.CollectionToString<object>(System.Collections.Generic.IEnumerable<object>)
		// int System.Collections.Generic.CollectionExtensions.GetValueOrDefault<Unity.Mathematics.int3,int>(System.Collections.Generic.IReadOnlyDictionary<Unity.Mathematics.int3,int>,Unity.Mathematics.int3,int)
		// int System.Collections.Generic.CollectionExtensions.GetValueOrDefault<int,int>(System.Collections.Generic.IReadOnlyDictionary<int,int>,int,int)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,object>(Cysharp.Threading.Tasks.UniTask.Awaiter&,object&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,object>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,object&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<object>(object&)
		// object& System.Runtime.CompilerServices.Unsafe.As<object,object>(object&)
		// System.Void* System.Runtime.CompilerServices.Unsafe.AsPointer<object>(object&)
		// long Unity.Burst.BurstRuntime.GetHashCode64<object>()
		// System.Void* Unity.Collections.AllocatorManager.Allocate<Unity.Collections.AllocatorManager.AllocatorHandle>(Unity.Collections.AllocatorManager.AllocatorHandle&,int,int,int)
		// Unity.Collections.AllocatorManager.Block Unity.Collections.AllocatorManager.AllocateBlock<Unity.Collections.AllocatorManager.AllocatorHandle>(Unity.Collections.AllocatorManager.AllocatorHandle&,int,int,int)
		// System.Void* Unity.Collections.AllocatorManager.AllocateStruct<Unity.Collections.AllocatorManager.AllocatorHandle,GameFramework.ECS.Components.BuildingComponent>(Unity.Collections.AllocatorManager.AllocatorHandle&,GameFramework.ECS.Components.BuildingComponent,int)
		// System.Void* Unity.Collections.AllocatorManager.AllocateStruct<Unity.Collections.AllocatorManager.AllocatorHandle,GameFramework.ECS.Components.GridPositionComponent>(Unity.Collections.AllocatorManager.AllocatorHandle&,GameFramework.ECS.Components.GridPositionComponent,int)
		// System.Void* Unity.Collections.AllocatorManager.AllocateStruct<Unity.Collections.AllocatorManager.AllocatorHandle,GameFramework.ECS.Components.PlaceObjectRequest>(Unity.Collections.AllocatorManager.AllocatorHandle&,GameFramework.ECS.Components.PlaceObjectRequest,int)
		// Unity.Collections.NativeArray<GameFramework.ECS.Components.BuildingComponent> Unity.Collections.CollectionHelper.CreateNativeArray<GameFramework.ECS.Components.BuildingComponent>(int,Unity.Collections.AllocatorManager.AllocatorHandle,Unity.Collections.NativeArrayOptions)
		// Unity.Collections.NativeArray<GameFramework.ECS.Components.GridPositionComponent> Unity.Collections.CollectionHelper.CreateNativeArray<GameFramework.ECS.Components.GridPositionComponent>(int,Unity.Collections.AllocatorManager.AllocatorHandle,Unity.Collections.NativeArrayOptions)
		// Unity.Collections.NativeArray<GameFramework.ECS.Components.PlaceObjectRequest> Unity.Collections.CollectionHelper.CreateNativeArray<GameFramework.ECS.Components.PlaceObjectRequest>(int,Unity.Collections.AllocatorManager.AllocatorHandle,Unity.Collections.NativeArrayOptions)
		// Unity.Collections.CopyError Unity.Collections.FixedStringMethods.CopyFromTruncated<Unity.Collections.FixedString64Bytes,Unity.Collections.FixedString128Bytes>(Unity.Collections.FixedString64Bytes&,Unity.Collections.FixedString128Bytes&)
		// System.Void* Unity.Collections.LowLevel.Unsafe.NativeArrayUnsafeUtility.GetUnsafePtr<GameFramework.ECS.Components.BuildingComponent>(Unity.Collections.NativeArray<GameFramework.ECS.Components.BuildingComponent>)
		// System.Void* Unity.Collections.LowLevel.Unsafe.NativeArrayUnsafeUtility.GetUnsafePtr<GameFramework.ECS.Components.GridPositionComponent>(Unity.Collections.NativeArray<GameFramework.ECS.Components.GridPositionComponent>)
		// System.Void* Unity.Collections.LowLevel.Unsafe.NativeArrayUnsafeUtility.GetUnsafePtr<GameFramework.ECS.Components.PlaceObjectRequest>(Unity.Collections.NativeArray<GameFramework.ECS.Components.PlaceObjectRequest>)
		// System.Void* Unity.Collections.LowLevel.Unsafe.UnsafeUtility.AddressOf<Unity.Physics.PhysicsWorldIndex>(Unity.Physics.PhysicsWorldIndex&)
		// int Unity.Collections.LowLevel.Unsafe.UnsafeUtility.AlignOf<GameFramework.ECS.Components.BuildingComponent>()
		// int Unity.Collections.LowLevel.Unsafe.UnsafeUtility.AlignOf<GameFramework.ECS.Components.GridPositionComponent>()
		// int Unity.Collections.LowLevel.Unsafe.UnsafeUtility.AlignOf<GameFramework.ECS.Components.PlaceObjectRequest>()
		// GameFramework.ECS.Components.GridConfigComponent& Unity.Collections.LowLevel.Unsafe.UnsafeUtility.AsRef<GameFramework.ECS.Components.GridConfigComponent>(System.Void*)
		// GameFramework.ECS.Components.PlacementStateComponent& Unity.Collections.LowLevel.Unsafe.UnsafeUtility.AsRef<GameFramework.ECS.Components.PlacementStateComponent>(System.Void*)
		// Unity.Physics.PhysicsWorldSingleton& Unity.Collections.LowLevel.Unsafe.UnsafeUtility.AsRef<Unity.Physics.PhysicsWorldSingleton>(System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyPtrToStructure<GameFramework.ECS.Components.BuildingComponent>(System.Void*,GameFramework.ECS.Components.BuildingComponent&)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyPtrToStructure<GameFramework.ECS.Components.GridPositionComponent>(System.Void*,GameFramework.ECS.Components.GridPositionComponent&)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyPtrToStructure<GameFramework.ECS.Components.IslandComponent>(System.Void*,GameFramework.ECS.Components.IslandComponent&)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyPtrToStructure<GameFramework.ECS.Components.PlacementStateComponent>(System.Void*,GameFramework.ECS.Components.PlacementStateComponent&)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyStructureToPtr<GameFramework.ECS.Components.BridgeComponent>(GameFramework.ECS.Components.BridgeComponent&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyStructureToPtr<GameFramework.ECS.Components.BuildingComponent>(GameFramework.ECS.Components.BuildingComponent&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyStructureToPtr<GameFramework.ECS.Components.GridConfigComponent>(GameFramework.ECS.Components.GridConfigComponent&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyStructureToPtr<GameFramework.ECS.Components.GridPositionComponent>(GameFramework.ECS.Components.GridPositionComponent&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyStructureToPtr<GameFramework.ECS.Components.IslandComponent>(GameFramework.ECS.Components.IslandComponent&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyStructureToPtr<GameFramework.ECS.Components.PlaceObjectRequest>(GameFramework.ECS.Components.PlaceObjectRequest&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyStructureToPtr<GameFramework.ECS.Components.PlacementStateComponent>(GameFramework.ECS.Components.PlacementStateComponent&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyStructureToPtr<GameFramework.ECS.Components.VisitorCenterComponent>(GameFramework.ECS.Components.VisitorCenterComponent&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyStructureToPtr<GameFramework.ECS.Components.VisitorComponent>(GameFramework.ECS.Components.VisitorComponent&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyStructureToPtr<Unity.Physics.PhysicsCollider>(Unity.Physics.PhysicsCollider&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyStructureToPtr<Unity.Transforms.LocalTransform>(Unity.Transforms.LocalTransform&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyStructureToPtr<Unity.Transforms.Parent>(Unity.Transforms.Parent&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyPtrToStructure<GameFramework.ECS.Components.BuildingComponent>(System.Void*,GameFramework.ECS.Components.BuildingComponent&)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyPtrToStructure<GameFramework.ECS.Components.GridPositionComponent>(System.Void*,GameFramework.ECS.Components.GridPositionComponent&)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyPtrToStructure<GameFramework.ECS.Components.IslandComponent>(System.Void*,GameFramework.ECS.Components.IslandComponent&)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyPtrToStructure<GameFramework.ECS.Components.PlacementStateComponent>(System.Void*,GameFramework.ECS.Components.PlacementStateComponent&)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyStructureToPtr<GameFramework.ECS.Components.BridgeComponent>(GameFramework.ECS.Components.BridgeComponent&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyStructureToPtr<GameFramework.ECS.Components.BuildingComponent>(GameFramework.ECS.Components.BuildingComponent&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyStructureToPtr<GameFramework.ECS.Components.GridConfigComponent>(GameFramework.ECS.Components.GridConfigComponent&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyStructureToPtr<GameFramework.ECS.Components.GridPositionComponent>(GameFramework.ECS.Components.GridPositionComponent&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyStructureToPtr<GameFramework.ECS.Components.IslandComponent>(GameFramework.ECS.Components.IslandComponent&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyStructureToPtr<GameFramework.ECS.Components.PlaceObjectRequest>(GameFramework.ECS.Components.PlaceObjectRequest&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyStructureToPtr<GameFramework.ECS.Components.PlacementStateComponent>(GameFramework.ECS.Components.PlacementStateComponent&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyStructureToPtr<GameFramework.ECS.Components.VisitorCenterComponent>(GameFramework.ECS.Components.VisitorCenterComponent&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyStructureToPtr<GameFramework.ECS.Components.VisitorComponent>(GameFramework.ECS.Components.VisitorComponent&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyStructureToPtr<Unity.Physics.PhysicsCollider>(Unity.Physics.PhysicsCollider&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyStructureToPtr<Unity.Transforms.LocalTransform>(Unity.Transforms.LocalTransform&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyStructureToPtr<Unity.Transforms.Parent>(Unity.Transforms.Parent&,System.Void*)
		// int Unity.Collections.LowLevel.Unsafe.UnsafeUtility.SizeOf<GameFramework.ECS.Components.BuildingComponent>()
		// int Unity.Collections.LowLevel.Unsafe.UnsafeUtility.SizeOf<GameFramework.ECS.Components.GridPositionComponent>()
		// int Unity.Collections.LowLevel.Unsafe.UnsafeUtility.SizeOf<GameFramework.ECS.Components.PathfindingRequest>()
		// int Unity.Collections.LowLevel.Unsafe.UnsafeUtility.SizeOf<GameFramework.ECS.Components.PlaceObjectRequest>()
		// int Unity.Collections.LowLevel.Unsafe.UnsafeUtility.SizeOf<GameFramework.ECS.Components.PlacementStateComponent>()
		// int Unity.Collections.LowLevel.Unsafe.UnsafeUtility.SizeOf<GameFramework.ECS.Components.VisitorCenterComponent>()
		// int Unity.Collections.LowLevel.Unsafe.UnsafeUtility.SizeOf<GameFramework.ECS.Components.VisitorComponent>()
		// int Unity.Collections.LowLevel.Unsafe.UnsafeUtility.SizeOf<Unity.Collections.LowLevel.Unsafe.UnsafeUtility.AlignOfHelper<GameFramework.ECS.Components.BuildingComponent>>()
		// int Unity.Collections.LowLevel.Unsafe.UnsafeUtility.SizeOf<Unity.Collections.LowLevel.Unsafe.UnsafeUtility.AlignOfHelper<GameFramework.ECS.Components.GridPositionComponent>>()
		// int Unity.Collections.LowLevel.Unsafe.UnsafeUtility.SizeOf<Unity.Collections.LowLevel.Unsafe.UnsafeUtility.AlignOfHelper<GameFramework.ECS.Components.PlaceObjectRequest>>()
		// int Unity.Collections.LowLevel.Unsafe.UnsafeUtility.SizeOf<Unity.Transforms.LocalTransform>()
		// int Unity.Collections.LowLevel.Unsafe.UnsafeUtility.SizeOf<object>()
		// System.Void Unity.Collections.NativeArrayExtensions.Initialize<GameFramework.ECS.Components.BuildingComponent>(Unity.Collections.NativeArray<GameFramework.ECS.Components.BuildingComponent>&,int,Unity.Collections.AllocatorManager.AllocatorHandle,Unity.Collections.NativeArrayOptions)
		// System.Void Unity.Collections.NativeArrayExtensions.Initialize<GameFramework.ECS.Components.GridPositionComponent>(Unity.Collections.NativeArray<GameFramework.ECS.Components.GridPositionComponent>&,int,Unity.Collections.AllocatorManager.AllocatorHandle,Unity.Collections.NativeArrayOptions)
		// System.Void Unity.Collections.NativeArrayExtensions.Initialize<GameFramework.ECS.Components.PlaceObjectRequest>(Unity.Collections.NativeArray<GameFramework.ECS.Components.PlaceObjectRequest>&,int,Unity.Collections.AllocatorManager.AllocatorHandle,Unity.Collections.NativeArrayOptions)
		// Unity.Entities.BufferAccessor<GameFramework.ECS.Components.PathBufferElement> Unity.Entities.ArchetypeChunk.GetBufferAccessor<GameFramework.ECS.Components.PathBufferElement>(Unity.Entities.BufferTypeHandle<GameFramework.ECS.Components.PathBufferElement>&)
		// Unity.Entities.BufferAccessor<GameFramework.ECS.Components.VisitedBuildingElement> Unity.Entities.ArchetypeChunk.GetBufferAccessor<GameFramework.ECS.Components.VisitedBuildingElement>(Unity.Entities.BufferTypeHandle<GameFramework.ECS.Components.VisitedBuildingElement>&)
		// Unity.Entities.BufferAccessor<GameFramework.ECS.Components.PathBufferElement> Unity.Entities.ArchetypeChunk.GetBufferAccessorInternal<GameFramework.ECS.Components.PathBufferElement>(Unity.Entities.BufferTypeHandle<GameFramework.ECS.Components.PathBufferElement>&,bool)
		// Unity.Entities.BufferAccessor<GameFramework.ECS.Components.VisitedBuildingElement> Unity.Entities.ArchetypeChunk.GetBufferAccessorInternal<GameFramework.ECS.Components.VisitedBuildingElement>(Unity.Entities.BufferTypeHandle<GameFramework.ECS.Components.VisitedBuildingElement>&,bool)
		// System.Void Unity.Entities.ArchetypeChunk.JournalAddRecordGetBufferRW<GameFramework.ECS.Components.PathBufferElement>(Unity.Entities.BufferTypeHandle<GameFramework.ECS.Components.PathBufferElement>&)
		// System.Void Unity.Entities.ArchetypeChunk.JournalAddRecordGetBufferRW<GameFramework.ECS.Components.VisitedBuildingElement>(Unity.Entities.BufferTypeHandle<GameFramework.ECS.Components.VisitedBuildingElement>&)
		// System.Void Unity.Entities.ArchetypeChunk.JournalAddRecordGetComponentDataRW<GameFramework.ECS.Components.GridPositionComponent>(Unity.Entities.ComponentTypeHandle<GameFramework.ECS.Components.GridPositionComponent>&,System.Void*,int)
		// System.Void Unity.Entities.ArchetypeChunk.JournalAddRecordGetComponentDataRW<GameFramework.ECS.Components.PathfindingRequest>(Unity.Entities.ComponentTypeHandle<GameFramework.ECS.Components.PathfindingRequest>&,System.Void*,int)
		// System.Void Unity.Entities.ArchetypeChunk.JournalAddRecordGetComponentDataRW<GameFramework.ECS.Components.VisitorCenterComponent>(Unity.Entities.ComponentTypeHandle<GameFramework.ECS.Components.VisitorCenterComponent>&,System.Void*,int)
		// System.Void Unity.Entities.ArchetypeChunk.JournalAddRecordGetComponentDataRW<GameFramework.ECS.Components.VisitorComponent>(Unity.Entities.ComponentTypeHandle<GameFramework.ECS.Components.VisitorComponent>&,System.Void*,int)
		// System.Void Unity.Entities.ArchetypeChunk.JournalAddRecordGetComponentDataRW<Unity.Transforms.LocalTransform>(Unity.Entities.ComponentTypeHandle<Unity.Transforms.LocalTransform>&,System.Void*,int)
		// Unity.Collections.NativeArray<GameFramework.ECS.Components.BuildingComponent> Unity.Entities.ChunkIterationUtility.CreateComponentDataArray<GameFramework.ECS.Components.BuildingComponent>(Unity.Collections.AllocatorManager.AllocatorHandle,Unity.Entities.ComponentTypeHandle<GameFramework.ECS.Components.BuildingComponent>&,int,Unity.Entities.EntityQuery)
		// Unity.Collections.NativeArray<GameFramework.ECS.Components.GridPositionComponent> Unity.Entities.ChunkIterationUtility.CreateComponentDataArray<GameFramework.ECS.Components.GridPositionComponent>(Unity.Collections.AllocatorManager.AllocatorHandle,Unity.Entities.ComponentTypeHandle<GameFramework.ECS.Components.GridPositionComponent>&,int,Unity.Entities.EntityQuery)
		// Unity.Collections.NativeArray<GameFramework.ECS.Components.PlaceObjectRequest> Unity.Entities.ChunkIterationUtility.CreateComponentDataArray<GameFramework.ECS.Components.PlaceObjectRequest>(Unity.Collections.AllocatorManager.AllocatorHandle,Unity.Entities.ComponentTypeHandle<GameFramework.ECS.Components.PlaceObjectRequest>&,int,Unity.Entities.EntityQuery)
		// System.Void Unity.Entities.ComponentSystemBase.RequireForUpdate<GameFramework.ECS.Components.GridConfigComponent>()
		// System.Void Unity.Entities.ComponentSystemBase.RequireForUpdate<GameFramework.ECS.Components.PlacementStateComponent>()
		// System.Void Unity.Entities.ComponentSystemBase.RequireForUpdate<Unity.Physics.PhysicsWorldSingleton>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadOnly<GameFramework.ECS.Components.GridConfigComponent>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadOnly<GameFramework.ECS.Components.GridPositionComponent>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadOnly<GameFramework.ECS.Components.PathBufferElement>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadOnly<GameFramework.ECS.Components.PathfindingRequest>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadOnly<GameFramework.ECS.Components.PlacementStateComponent>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadOnly<GameFramework.ECS.Components.VisitedBuildingElement>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadOnly<GameFramework.ECS.Components.VisitorCenterComponent>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadOnly<GameFramework.ECS.Components.VisitorComponent>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadOnly<Unity.Physics.PhysicsWorldSingleton>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadOnly<Unity.Transforms.LocalTransform>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadWrite<GameFramework.ECS.Components.BridgeComponent>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadWrite<GameFramework.ECS.Components.BuildingComponent>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadWrite<GameFramework.ECS.Components.GridConfigComponent>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadWrite<GameFramework.ECS.Components.GridPositionComponent>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadWrite<GameFramework.ECS.Components.IslandComponent>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadWrite<GameFramework.ECS.Components.PathBufferElement>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadWrite<GameFramework.ECS.Components.PathfindingRequest>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadWrite<GameFramework.ECS.Components.PlaceObjectRequest>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadWrite<GameFramework.ECS.Components.PlacementStateComponent>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadWrite<GameFramework.ECS.Components.VisitedBuildingElement>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadWrite<GameFramework.ECS.Components.VisitorCenterComponent>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadWrite<GameFramework.ECS.Components.VisitorComponent>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadWrite<GameFramework.ECS.Components.VisualGridTag>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadWrite<Unity.Entities.LinkedEntityGroup>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadWrite<Unity.Entities.Prefab>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadWrite<Unity.Physics.PhysicsCollider>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadWrite<Unity.Physics.PhysicsWorldIndex>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadWrite<Unity.Physics.PhysicsWorldSingleton>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadWrite<Unity.Transforms.LocalToWorld>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadWrite<Unity.Transforms.LocalTransform>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadWrite<Unity.Transforms.Parent>()
		// Unity.Entities.DynamicBuffer<GameFramework.ECS.Components.PathBufferElement> Unity.Entities.EntityDataAccess.GetBuffer<GameFramework.ECS.Components.PathBufferElement>(Unity.Entities.Entity,bool)
		// Unity.Entities.DynamicBuffer<GameFramework.ECS.Components.VisitedBuildingElement> Unity.Entities.EntityDataAccess.GetBuffer<GameFramework.ECS.Components.VisitedBuildingElement>(Unity.Entities.Entity,bool)
		// Unity.Entities.DynamicBuffer<Unity.Entities.LinkedEntityGroup> Unity.Entities.EntityDataAccess.GetBuffer<Unity.Entities.LinkedEntityGroup>(Unity.Entities.Entity,bool)
		// Unity.Entities.BufferTypeHandle<GameFramework.ECS.Components.PathBufferElement> Unity.Entities.EntityDataAccess.GetBufferTypeHandle<GameFramework.ECS.Components.PathBufferElement>(bool)
		// Unity.Entities.BufferTypeHandle<GameFramework.ECS.Components.VisitedBuildingElement> Unity.Entities.EntityDataAccess.GetBufferTypeHandle<GameFramework.ECS.Components.VisitedBuildingElement>(bool)
		// GameFramework.ECS.Components.BuildingComponent Unity.Entities.EntityDataAccess.GetComponentData<GameFramework.ECS.Components.BuildingComponent>(Unity.Entities.Entity)
		// GameFramework.ECS.Components.GridPositionComponent Unity.Entities.EntityDataAccess.GetComponentData<GameFramework.ECS.Components.GridPositionComponent>(Unity.Entities.Entity)
		// GameFramework.ECS.Components.IslandComponent Unity.Entities.EntityDataAccess.GetComponentData<GameFramework.ECS.Components.IslandComponent>(Unity.Entities.Entity)
		// GameFramework.ECS.Components.PlacementStateComponent Unity.Entities.EntityDataAccess.GetComponentData<GameFramework.ECS.Components.PlacementStateComponent>(Unity.Entities.Entity)
		// System.Void Unity.Entities.EntityDataAccess.SetComponentData<GameFramework.ECS.Components.BridgeComponent>(Unity.Entities.Entity,GameFramework.ECS.Components.BridgeComponent,Unity.Entities.SystemHandle&)
		// System.Void Unity.Entities.EntityDataAccess.SetComponentData<GameFramework.ECS.Components.BuildingComponent>(Unity.Entities.Entity,GameFramework.ECS.Components.BuildingComponent,Unity.Entities.SystemHandle&)
		// System.Void Unity.Entities.EntityDataAccess.SetComponentData<GameFramework.ECS.Components.GridConfigComponent>(Unity.Entities.Entity,GameFramework.ECS.Components.GridConfigComponent,Unity.Entities.SystemHandle&)
		// System.Void Unity.Entities.EntityDataAccess.SetComponentData<GameFramework.ECS.Components.GridPositionComponent>(Unity.Entities.Entity,GameFramework.ECS.Components.GridPositionComponent,Unity.Entities.SystemHandle&)
		// System.Void Unity.Entities.EntityDataAccess.SetComponentData<GameFramework.ECS.Components.IslandComponent>(Unity.Entities.Entity,GameFramework.ECS.Components.IslandComponent,Unity.Entities.SystemHandle&)
		// System.Void Unity.Entities.EntityDataAccess.SetComponentData<GameFramework.ECS.Components.PlaceObjectRequest>(Unity.Entities.Entity,GameFramework.ECS.Components.PlaceObjectRequest,Unity.Entities.SystemHandle&)
		// System.Void Unity.Entities.EntityDataAccess.SetComponentData<GameFramework.ECS.Components.PlacementStateComponent>(Unity.Entities.Entity,GameFramework.ECS.Components.PlacementStateComponent,Unity.Entities.SystemHandle&)
		// System.Void Unity.Entities.EntityDataAccess.SetComponentData<GameFramework.ECS.Components.VisitorCenterComponent>(Unity.Entities.Entity,GameFramework.ECS.Components.VisitorCenterComponent,Unity.Entities.SystemHandle&)
		// System.Void Unity.Entities.EntityDataAccess.SetComponentData<GameFramework.ECS.Components.VisitorComponent>(Unity.Entities.Entity,GameFramework.ECS.Components.VisitorComponent,Unity.Entities.SystemHandle&)
		// System.Void Unity.Entities.EntityDataAccess.SetComponentData<Unity.Physics.PhysicsCollider>(Unity.Entities.Entity,Unity.Physics.PhysicsCollider,Unity.Entities.SystemHandle&)
		// System.Void Unity.Entities.EntityDataAccess.SetComponentData<Unity.Transforms.LocalTransform>(Unity.Entities.Entity,Unity.Transforms.LocalTransform,Unity.Entities.SystemHandle&)
		// System.Void Unity.Entities.EntityDataAccess.SetComponentData<Unity.Transforms.Parent>(Unity.Entities.Entity,Unity.Transforms.Parent,Unity.Entities.SystemHandle&)
		// Unity.Entities.DynamicBuffer<GameFramework.ECS.Components.PathBufferElement> Unity.Entities.EntityManager.AddBuffer<GameFramework.ECS.Components.PathBufferElement>(Unity.Entities.Entity)
		// Unity.Entities.DynamicBuffer<GameFramework.ECS.Components.VisitedBuildingElement> Unity.Entities.EntityManager.AddBuffer<GameFramework.ECS.Components.VisitedBuildingElement>(Unity.Entities.Entity)
		// Unity.Entities.DynamicBuffer<Unity.Entities.LinkedEntityGroup> Unity.Entities.EntityManager.AddBuffer<Unity.Entities.LinkedEntityGroup>(Unity.Entities.Entity)
		// bool Unity.Entities.EntityManager.AddComponent<GameFramework.ECS.Components.PathBufferElement>(Unity.Entities.Entity)
		// bool Unity.Entities.EntityManager.AddComponent<GameFramework.ECS.Components.PathfindingRequest>(Unity.Entities.Entity)
		// bool Unity.Entities.EntityManager.AddComponent<GameFramework.ECS.Components.VisitedBuildingElement>(Unity.Entities.Entity)
		// bool Unity.Entities.EntityManager.AddComponent<GameFramework.ECS.Components.VisualGridTag>(Unity.Entities.Entity)
		// bool Unity.Entities.EntityManager.AddComponent<Unity.Entities.LinkedEntityGroup>(Unity.Entities.Entity)
		// bool Unity.Entities.EntityManager.AddComponent<Unity.Entities.Prefab>(Unity.Entities.Entity)
		// bool Unity.Entities.EntityManager.AddComponent<Unity.Transforms.LocalToWorld>(Unity.Entities.Entity)
		// bool Unity.Entities.EntityManager.AddComponentData<GameFramework.ECS.Components.BridgeComponent>(Unity.Entities.Entity,GameFramework.ECS.Components.BridgeComponent)
		// bool Unity.Entities.EntityManager.AddComponentData<GameFramework.ECS.Components.BuildingComponent>(Unity.Entities.Entity,GameFramework.ECS.Components.BuildingComponent)
		// bool Unity.Entities.EntityManager.AddComponentData<GameFramework.ECS.Components.GridPositionComponent>(Unity.Entities.Entity,GameFramework.ECS.Components.GridPositionComponent)
		// bool Unity.Entities.EntityManager.AddComponentData<GameFramework.ECS.Components.IslandComponent>(Unity.Entities.Entity,GameFramework.ECS.Components.IslandComponent)
		// bool Unity.Entities.EntityManager.AddComponentData<GameFramework.ECS.Components.PlaceObjectRequest>(Unity.Entities.Entity,GameFramework.ECS.Components.PlaceObjectRequest)
		// bool Unity.Entities.EntityManager.AddComponentData<GameFramework.ECS.Components.VisitorCenterComponent>(Unity.Entities.Entity,GameFramework.ECS.Components.VisitorCenterComponent)
		// bool Unity.Entities.EntityManager.AddComponentData<GameFramework.ECS.Components.VisitorComponent>(Unity.Entities.Entity,GameFramework.ECS.Components.VisitorComponent)
		// bool Unity.Entities.EntityManager.AddComponentData<Unity.Physics.PhysicsCollider>(Unity.Entities.Entity,Unity.Physics.PhysicsCollider)
		// bool Unity.Entities.EntityManager.AddComponentData<Unity.Transforms.LocalTransform>(Unity.Entities.Entity,Unity.Transforms.LocalTransform)
		// bool Unity.Entities.EntityManager.AddComponentData<Unity.Transforms.Parent>(Unity.Entities.Entity,Unity.Transforms.Parent)
		// bool Unity.Entities.EntityManager.AddSharedComponent<Unity.Physics.PhysicsWorldIndex>(Unity.Entities.Entity,Unity.Physics.PhysicsWorldIndex)
		// System.Void Unity.Entities.EntityManager.CompleteDependencyBeforeRO<GameFramework.ECS.Components.GridPositionComponent>()
		// System.Void Unity.Entities.EntityManager.CompleteDependencyBeforeRW<GameFramework.ECS.Components.GridPositionComponent>()
		// System.Void Unity.Entities.EntityManager.CompleteDependencyBeforeRW<GameFramework.ECS.Components.PathBufferElement>()
		// System.Void Unity.Entities.EntityManager.CompleteDependencyBeforeRW<GameFramework.ECS.Components.PathfindingRequest>()
		// System.Void Unity.Entities.EntityManager.CompleteDependencyBeforeRW<GameFramework.ECS.Components.VisitedBuildingElement>()
		// System.Void Unity.Entities.EntityManager.CompleteDependencyBeforeRW<GameFramework.ECS.Components.VisitorCenterComponent>()
		// System.Void Unity.Entities.EntityManager.CompleteDependencyBeforeRW<GameFramework.ECS.Components.VisitorComponent>()
		// System.Void Unity.Entities.EntityManager.CompleteDependencyBeforeRW<Unity.Transforms.LocalTransform>()
		// Unity.Entities.Entity Unity.Entities.EntityManager.CreateSingleton<GameFramework.ECS.Components.GridConfigComponent>(GameFramework.ECS.Components.GridConfigComponent,Unity.Collections.FixedString64Bytes)
		// Unity.Entities.Entity Unity.Entities.EntityManager.CreateSingleton<GameFramework.ECS.Components.PlacementStateComponent>(Unity.Collections.FixedString64Bytes)
		// Unity.Entities.Entity Unity.Entities.EntityManager.CreateSingletonEntityInternal<GameFramework.ECS.Components.GridConfigComponent>(Unity.Collections.FixedString64Bytes)
		// Unity.Entities.Entity Unity.Entities.EntityManager.CreateSingletonEntityInternal<GameFramework.ECS.Components.PlacementStateComponent>(Unity.Collections.FixedString64Bytes)
		// Unity.Entities.DynamicBuffer<GameFramework.ECS.Components.PathBufferElement> Unity.Entities.EntityManager.GetBuffer<GameFramework.ECS.Components.PathBufferElement>(Unity.Entities.Entity,bool)
		// Unity.Entities.DynamicBuffer<GameFramework.ECS.Components.VisitedBuildingElement> Unity.Entities.EntityManager.GetBuffer<GameFramework.ECS.Components.VisitedBuildingElement>(Unity.Entities.Entity,bool)
		// Unity.Entities.DynamicBuffer<Unity.Entities.LinkedEntityGroup> Unity.Entities.EntityManager.GetBuffer<Unity.Entities.LinkedEntityGroup>(Unity.Entities.Entity,bool)
		// Unity.Entities.DynamicBuffer<GameFramework.ECS.Components.PathBufferElement> Unity.Entities.EntityManager.GetBufferInternal<GameFramework.ECS.Components.PathBufferElement>(Unity.Entities.EntityDataAccess*,Unity.Entities.Entity,bool)
		// Unity.Entities.DynamicBuffer<GameFramework.ECS.Components.VisitedBuildingElement> Unity.Entities.EntityManager.GetBufferInternal<GameFramework.ECS.Components.VisitedBuildingElement>(Unity.Entities.EntityDataAccess*,Unity.Entities.Entity,bool)
		// Unity.Entities.DynamicBuffer<Unity.Entities.LinkedEntityGroup> Unity.Entities.EntityManager.GetBufferInternal<Unity.Entities.LinkedEntityGroup>(Unity.Entities.EntityDataAccess*,Unity.Entities.Entity,bool)
		// Unity.Entities.BufferTypeHandle<GameFramework.ECS.Components.PathBufferElement> Unity.Entities.EntityManager.GetBufferTypeHandle<GameFramework.ECS.Components.PathBufferElement>(bool)
		// Unity.Entities.BufferTypeHandle<GameFramework.ECS.Components.VisitedBuildingElement> Unity.Entities.EntityManager.GetBufferTypeHandle<GameFramework.ECS.Components.VisitedBuildingElement>(bool)
		// GameFramework.ECS.Components.BuildingComponent Unity.Entities.EntityManager.GetComponentData<GameFramework.ECS.Components.BuildingComponent>(Unity.Entities.Entity)
		// GameFramework.ECS.Components.GridPositionComponent Unity.Entities.EntityManager.GetComponentData<GameFramework.ECS.Components.GridPositionComponent>(Unity.Entities.Entity)
		// GameFramework.ECS.Components.IslandComponent Unity.Entities.EntityManager.GetComponentData<GameFramework.ECS.Components.IslandComponent>(Unity.Entities.Entity)
		// GameFramework.ECS.Components.PlacementStateComponent Unity.Entities.EntityManager.GetComponentData<GameFramework.ECS.Components.PlacementStateComponent>(Unity.Entities.Entity)
		// Unity.Entities.ComponentTypeHandle<GameFramework.ECS.Components.GridPositionComponent> Unity.Entities.EntityManager.GetComponentTypeHandle<GameFramework.ECS.Components.GridPositionComponent>(bool)
		// Unity.Entities.ComponentTypeHandle<GameFramework.ECS.Components.PathfindingRequest> Unity.Entities.EntityManager.GetComponentTypeHandle<GameFramework.ECS.Components.PathfindingRequest>(bool)
		// Unity.Entities.ComponentTypeHandle<GameFramework.ECS.Components.VisitorCenterComponent> Unity.Entities.EntityManager.GetComponentTypeHandle<GameFramework.ECS.Components.VisitorCenterComponent>(bool)
		// Unity.Entities.ComponentTypeHandle<GameFramework.ECS.Components.VisitorComponent> Unity.Entities.EntityManager.GetComponentTypeHandle<GameFramework.ECS.Components.VisitorComponent>(bool)
		// Unity.Entities.ComponentTypeHandle<Unity.Transforms.LocalTransform> Unity.Entities.EntityManager.GetComponentTypeHandle<Unity.Transforms.LocalTransform>(bool)
		// bool Unity.Entities.EntityManager.HasComponent<GameFramework.ECS.Components.BuildingComponent>(Unity.Entities.Entity)
		// bool Unity.Entities.EntityManager.HasComponent<GameFramework.ECS.Components.GridPositionComponent>(Unity.Entities.Entity)
		// bool Unity.Entities.EntityManager.HasComponent<GameFramework.ECS.Components.IslandComponent>(Unity.Entities.Entity)
		// bool Unity.Entities.EntityManager.HasComponent<Unity.Transforms.LocalToWorld>(Unity.Entities.Entity)
		// bool Unity.Entities.EntityManager.RemoveComponent<Unity.Entities.Prefab>(Unity.Entities.Entity)
		// System.Void Unity.Entities.EntityManager.SetComponentData<GameFramework.ECS.Components.BridgeComponent>(Unity.Entities.Entity,GameFramework.ECS.Components.BridgeComponent)
		// System.Void Unity.Entities.EntityManager.SetComponentData<GameFramework.ECS.Components.BuildingComponent>(Unity.Entities.Entity,GameFramework.ECS.Components.BuildingComponent)
		// System.Void Unity.Entities.EntityManager.SetComponentData<GameFramework.ECS.Components.GridConfigComponent>(Unity.Entities.Entity,GameFramework.ECS.Components.GridConfigComponent)
		// System.Void Unity.Entities.EntityManager.SetComponentData<GameFramework.ECS.Components.GridPositionComponent>(Unity.Entities.Entity,GameFramework.ECS.Components.GridPositionComponent)
		// System.Void Unity.Entities.EntityManager.SetComponentData<GameFramework.ECS.Components.IslandComponent>(Unity.Entities.Entity,GameFramework.ECS.Components.IslandComponent)
		// System.Void Unity.Entities.EntityManager.SetComponentData<GameFramework.ECS.Components.PlaceObjectRequest>(Unity.Entities.Entity,GameFramework.ECS.Components.PlaceObjectRequest)
		// System.Void Unity.Entities.EntityManager.SetComponentData<GameFramework.ECS.Components.PlacementStateComponent>(Unity.Entities.Entity,GameFramework.ECS.Components.PlacementStateComponent)
		// System.Void Unity.Entities.EntityManager.SetComponentData<GameFramework.ECS.Components.VisitorCenterComponent>(Unity.Entities.Entity,GameFramework.ECS.Components.VisitorCenterComponent)
		// System.Void Unity.Entities.EntityManager.SetComponentData<GameFramework.ECS.Components.VisitorComponent>(Unity.Entities.Entity,GameFramework.ECS.Components.VisitorComponent)
		// System.Void Unity.Entities.EntityManager.SetComponentData<Unity.Physics.PhysicsCollider>(Unity.Entities.Entity,Unity.Physics.PhysicsCollider)
		// System.Void Unity.Entities.EntityManager.SetComponentData<Unity.Transforms.LocalTransform>(Unity.Entities.Entity,Unity.Transforms.LocalTransform)
		// System.Void Unity.Entities.EntityManager.SetComponentData<Unity.Transforms.Parent>(Unity.Entities.Entity,Unity.Transforms.Parent)
		// GameFramework.ECS.Components.GridConfigComponent Unity.Entities.EntityQuery.GetSingleton<GameFramework.ECS.Components.GridConfigComponent>()
		// GameFramework.ECS.Components.PlacementStateComponent Unity.Entities.EntityQuery.GetSingleton<GameFramework.ECS.Components.PlacementStateComponent>()
		// Unity.Physics.PhysicsWorldSingleton Unity.Entities.EntityQuery.GetSingleton<Unity.Physics.PhysicsWorldSingleton>()
		// Unity.Entities.RefRW<GameFramework.ECS.Components.PlacementStateComponent> Unity.Entities.EntityQuery.GetSingletonRW<GameFramework.ECS.Components.PlacementStateComponent>()
		// bool Unity.Entities.EntityQuery.HasSingleton<GameFramework.ECS.Components.GridConfigComponent>()
		// bool Unity.Entities.EntityQuery.HasSingleton<GameFramework.ECS.Components.PlacementStateComponent>()
		// Unity.Collections.NativeArray<GameFramework.ECS.Components.BuildingComponent> Unity.Entities.EntityQuery.ToComponentDataArray<GameFramework.ECS.Components.BuildingComponent>(Unity.Collections.AllocatorManager.AllocatorHandle)
		// Unity.Collections.NativeArray<GameFramework.ECS.Components.GridPositionComponent> Unity.Entities.EntityQuery.ToComponentDataArray<GameFramework.ECS.Components.GridPositionComponent>(Unity.Collections.AllocatorManager.AllocatorHandle)
		// Unity.Collections.NativeArray<GameFramework.ECS.Components.PlaceObjectRequest> Unity.Entities.EntityQuery.ToComponentDataArray<GameFramework.ECS.Components.PlaceObjectRequest>(Unity.Collections.AllocatorManager.AllocatorHandle)
		// Unity.Entities.EntityQueryBuilder Unity.Entities.EntityQueryBuilder.WithAll<GameFramework.ECS.Components.BuildingComponent,GameFramework.ECS.Components.GridPositionComponent>()
		// Unity.Entities.EntityQueryBuilder Unity.Entities.EntityQueryBuilder.WithAll<GameFramework.ECS.Components.GridConfigComponent>()
		// Unity.Entities.EntityQueryBuilder Unity.Entities.EntityQueryBuilder.WithAll<GameFramework.ECS.Components.GridPositionComponent>()
		// Unity.Entities.EntityQueryBuilder Unity.Entities.EntityQueryBuilder.WithAll<GameFramework.ECS.Components.PlaceObjectRequest>()
		// Unity.Entities.EntityQueryBuilder Unity.Entities.EntityQueryBuilder.WithAll<GameFramework.ECS.Components.PlacementStateComponent>()
		// Unity.Entities.EntityQueryBuilder Unity.Entities.EntityQueryBuilder.WithAll<Unity.Physics.PhysicsWorldSingleton>()
		// Unity.Entities.EntityQueryBuilder Unity.Entities.EntityQueryBuilder.WithAllRW<GameFramework.ECS.Components.GridPositionComponent>()
		// Unity.Entities.EntityQueryBuilder Unity.Entities.EntityQueryBuilder.WithAllRW<GameFramework.ECS.Components.PathBufferElement>()
		// Unity.Entities.EntityQueryBuilder Unity.Entities.EntityQueryBuilder.WithAllRW<GameFramework.ECS.Components.PathfindingRequest>()
		// Unity.Entities.EntityQueryBuilder Unity.Entities.EntityQueryBuilder.WithAllRW<GameFramework.ECS.Components.PlacementStateComponent>()
		// Unity.Entities.EntityQueryBuilder Unity.Entities.EntityQueryBuilder.WithAllRW<GameFramework.ECS.Components.VisitedBuildingElement>()
		// Unity.Entities.EntityQueryBuilder Unity.Entities.EntityQueryBuilder.WithAllRW<GameFramework.ECS.Components.VisitorCenterComponent>()
		// Unity.Entities.EntityQueryBuilder Unity.Entities.EntityQueryBuilder.WithAllRW<GameFramework.ECS.Components.VisitorComponent>()
		// Unity.Entities.EntityQueryBuilder Unity.Entities.EntityQueryBuilder.WithAllRW<Unity.Transforms.LocalTransform>()
		// GameFramework.ECS.Components.GridConfigComponent Unity.Entities.EntityQueryImpl.GetSingleton<GameFramework.ECS.Components.GridConfigComponent>()
		// GameFramework.ECS.Components.PlacementStateComponent Unity.Entities.EntityQueryImpl.GetSingleton<GameFramework.ECS.Components.PlacementStateComponent>()
		// Unity.Physics.PhysicsWorldSingleton Unity.Entities.EntityQueryImpl.GetSingleton<Unity.Physics.PhysicsWorldSingleton>()
		// Unity.Entities.RefRW<GameFramework.ECS.Components.PlacementStateComponent> Unity.Entities.EntityQueryImpl.GetSingletonRW<GameFramework.ECS.Components.PlacementStateComponent>()
		// bool Unity.Entities.EntityQueryImpl.HasSingleton<GameFramework.ECS.Components.GridConfigComponent>()
		// bool Unity.Entities.EntityQueryImpl.HasSingleton<GameFramework.ECS.Components.PlacementStateComponent>()
		// Unity.Collections.NativeArray<GameFramework.ECS.Components.BuildingComponent> Unity.Entities.EntityQueryImpl.ToComponentDataArray<GameFramework.ECS.Components.BuildingComponent>(Unity.Collections.AllocatorManager.AllocatorHandle,Unity.Entities.EntityQuery)
		// Unity.Collections.NativeArray<GameFramework.ECS.Components.GridPositionComponent> Unity.Entities.EntityQueryImpl.ToComponentDataArray<GameFramework.ECS.Components.GridPositionComponent>(Unity.Collections.AllocatorManager.AllocatorHandle,Unity.Entities.EntityQuery)
		// Unity.Collections.NativeArray<GameFramework.ECS.Components.PlaceObjectRequest> Unity.Entities.EntityQueryImpl.ToComponentDataArray<GameFramework.ECS.Components.PlaceObjectRequest>(Unity.Collections.AllocatorManager.AllocatorHandle,Unity.Entities.EntityQuery)
		// System.IntPtr Unity.Entities.Internal.InternalCompilerInterface.UnsafeGetChunkNativeArrayIntPtrWithoutChecks<GameFramework.ECS.Components.GridPositionComponent>(Unity.Entities.ArchetypeChunk&,Unity.Entities.ComponentTypeHandle<GameFramework.ECS.Components.GridPositionComponent>&)
		// System.IntPtr Unity.Entities.Internal.InternalCompilerInterface.UnsafeGetChunkNativeArrayIntPtrWithoutChecks<GameFramework.ECS.Components.PathfindingRequest>(Unity.Entities.ArchetypeChunk&,Unity.Entities.ComponentTypeHandle<GameFramework.ECS.Components.PathfindingRequest>&)
		// System.IntPtr Unity.Entities.Internal.InternalCompilerInterface.UnsafeGetChunkNativeArrayIntPtrWithoutChecks<GameFramework.ECS.Components.VisitorCenterComponent>(Unity.Entities.ArchetypeChunk&,Unity.Entities.ComponentTypeHandle<GameFramework.ECS.Components.VisitorCenterComponent>&)
		// System.IntPtr Unity.Entities.Internal.InternalCompilerInterface.UnsafeGetChunkNativeArrayIntPtrWithoutChecks<GameFramework.ECS.Components.VisitorComponent>(Unity.Entities.ArchetypeChunk&,Unity.Entities.ComponentTypeHandle<GameFramework.ECS.Components.VisitorComponent>&)
		// System.IntPtr Unity.Entities.Internal.InternalCompilerInterface.UnsafeGetChunkNativeArrayIntPtrWithoutChecks<Unity.Transforms.LocalTransform>(Unity.Entities.ArchetypeChunk&,Unity.Entities.ComponentTypeHandle<Unity.Transforms.LocalTransform>&)
		// System.IntPtr Unity.Entities.Internal.InternalCompilerInterface.UnsafeGetChunkNativeArrayReadOnlyIntPtrWithoutChecks<GameFramework.ECS.Components.GridPositionComponent>(Unity.Entities.ArchetypeChunk&,Unity.Entities.ComponentTypeHandle<GameFramework.ECS.Components.GridPositionComponent>&)
		// Unity.Entities.Entity Unity.Entities.Internal.InternalCompilerInterface.UnsafeGetCopyOfNativeArrayPtrElement<Unity.Entities.Entity>(System.IntPtr,int)
		// Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRO<GameFramework.ECS.Components.GridPositionComponent> Unity.Entities.Internal.InternalCompilerInterface.UnsafeGetUncheckedRefRO<GameFramework.ECS.Components.GridPositionComponent>(System.IntPtr,int)
		// Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRW<GameFramework.ECS.Components.GridPositionComponent> Unity.Entities.Internal.InternalCompilerInterface.UnsafeGetUncheckedRefRW<GameFramework.ECS.Components.GridPositionComponent>(System.IntPtr,int)
		// Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRW<GameFramework.ECS.Components.PathfindingRequest> Unity.Entities.Internal.InternalCompilerInterface.UnsafeGetUncheckedRefRW<GameFramework.ECS.Components.PathfindingRequest>(System.IntPtr,int)
		// Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRW<GameFramework.ECS.Components.VisitorCenterComponent> Unity.Entities.Internal.InternalCompilerInterface.UnsafeGetUncheckedRefRW<GameFramework.ECS.Components.VisitorCenterComponent>(System.IntPtr,int)
		// Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRW<GameFramework.ECS.Components.VisitorComponent> Unity.Entities.Internal.InternalCompilerInterface.UnsafeGetUncheckedRefRW<GameFramework.ECS.Components.VisitorComponent>(System.IntPtr,int)
		// Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRW<Unity.Transforms.LocalTransform> Unity.Entities.Internal.InternalCompilerInterface.UnsafeGetUncheckedRefRW<Unity.Transforms.LocalTransform>(System.IntPtr,int)
		// Unity.Entities.BufferTypeHandle<GameFramework.ECS.Components.PathBufferElement> Unity.Entities.SystemState.GetBufferTypeHandle<GameFramework.ECS.Components.PathBufferElement>(bool)
		// Unity.Entities.BufferTypeHandle<GameFramework.ECS.Components.VisitedBuildingElement> Unity.Entities.SystemState.GetBufferTypeHandle<GameFramework.ECS.Components.VisitedBuildingElement>(bool)
		// Unity.Entities.ComponentTypeHandle<GameFramework.ECS.Components.GridPositionComponent> Unity.Entities.SystemState.GetComponentTypeHandle<GameFramework.ECS.Components.GridPositionComponent>(bool)
		// Unity.Entities.ComponentTypeHandle<GameFramework.ECS.Components.PathfindingRequest> Unity.Entities.SystemState.GetComponentTypeHandle<GameFramework.ECS.Components.PathfindingRequest>(bool)
		// Unity.Entities.ComponentTypeHandle<GameFramework.ECS.Components.VisitorCenterComponent> Unity.Entities.SystemState.GetComponentTypeHandle<GameFramework.ECS.Components.VisitorCenterComponent>(bool)
		// Unity.Entities.ComponentTypeHandle<GameFramework.ECS.Components.VisitorComponent> Unity.Entities.SystemState.GetComponentTypeHandle<GameFramework.ECS.Components.VisitorComponent>(bool)
		// Unity.Entities.ComponentTypeHandle<Unity.Transforms.LocalTransform> Unity.Entities.SystemState.GetComponentTypeHandle<Unity.Transforms.LocalTransform>(bool)
		// System.Void Unity.Entities.SystemState.RequireForUpdate<GameFramework.ECS.Components.GridConfigComponent>()
		// System.Void Unity.Entities.SystemState.RequireForUpdate<GameFramework.ECS.Components.PlacementStateComponent>()
		// System.Void Unity.Entities.SystemState.RequireForUpdate<Unity.Physics.PhysicsWorldSingleton>()
		// Unity.Entities.SystemTypeIndex Unity.Entities.TypeManager.GetSystemTypeIndex<object>()
		// Unity.Entities.SystemTypeIndex Unity.Entities.TypeManager.GetSystemTypeIndexNoThrow<object>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<GameFramework.ECS.Components.BridgeComponent>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<GameFramework.ECS.Components.BuildingComponent>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<GameFramework.ECS.Components.GridConfigComponent>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<GameFramework.ECS.Components.GridPositionComponent>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<GameFramework.ECS.Components.IslandComponent>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<GameFramework.ECS.Components.PathBufferElement>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<GameFramework.ECS.Components.PathfindingRequest>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<GameFramework.ECS.Components.PlaceObjectRequest>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<GameFramework.ECS.Components.PlacementStateComponent>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<GameFramework.ECS.Components.VisitedBuildingElement>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<GameFramework.ECS.Components.VisitorCenterComponent>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<GameFramework.ECS.Components.VisitorComponent>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<GameFramework.ECS.Components.VisualGridTag>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<Unity.Entities.LinkedEntityGroup>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<Unity.Entities.Prefab>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<Unity.Physics.PhysicsCollider>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<Unity.Physics.PhysicsWorldIndex>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<Unity.Physics.PhysicsWorldSingleton>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<Unity.Transforms.LocalToWorld>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<Unity.Transforms.LocalTransform>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<Unity.Transforms.Parent>()
		// System.Void Unity.Entities.TypeManager.ManagedException<GameFramework.ECS.Components.BridgeComponent>()
		// System.Void Unity.Entities.TypeManager.ManagedException<GameFramework.ECS.Components.BuildingComponent>()
		// System.Void Unity.Entities.TypeManager.ManagedException<GameFramework.ECS.Components.GridConfigComponent>()
		// System.Void Unity.Entities.TypeManager.ManagedException<GameFramework.ECS.Components.GridPositionComponent>()
		// System.Void Unity.Entities.TypeManager.ManagedException<GameFramework.ECS.Components.IslandComponent>()
		// System.Void Unity.Entities.TypeManager.ManagedException<GameFramework.ECS.Components.PathBufferElement>()
		// System.Void Unity.Entities.TypeManager.ManagedException<GameFramework.ECS.Components.PathfindingRequest>()
		// System.Void Unity.Entities.TypeManager.ManagedException<GameFramework.ECS.Components.PlaceObjectRequest>()
		// System.Void Unity.Entities.TypeManager.ManagedException<GameFramework.ECS.Components.PlacementStateComponent>()
		// System.Void Unity.Entities.TypeManager.ManagedException<GameFramework.ECS.Components.VisitedBuildingElement>()
		// System.Void Unity.Entities.TypeManager.ManagedException<GameFramework.ECS.Components.VisitorCenterComponent>()
		// System.Void Unity.Entities.TypeManager.ManagedException<GameFramework.ECS.Components.VisitorComponent>()
		// System.Void Unity.Entities.TypeManager.ManagedException<GameFramework.ECS.Components.VisualGridTag>()
		// System.Void Unity.Entities.TypeManager.ManagedException<Unity.Entities.LinkedEntityGroup>()
		// System.Void Unity.Entities.TypeManager.ManagedException<Unity.Entities.Prefab>()
		// System.Void Unity.Entities.TypeManager.ManagedException<Unity.Physics.PhysicsCollider>()
		// System.Void Unity.Entities.TypeManager.ManagedException<Unity.Physics.PhysicsWorldIndex>()
		// System.Void Unity.Entities.TypeManager.ManagedException<Unity.Physics.PhysicsWorldSingleton>()
		// System.Void Unity.Entities.TypeManager.ManagedException<Unity.Transforms.LocalToWorld>()
		// System.Void Unity.Entities.TypeManager.ManagedException<Unity.Transforms.LocalTransform>()
		// System.Void Unity.Entities.TypeManager.ManagedException<Unity.Transforms.Parent>()
		// System.Void Unity.Entities.TypeManager.ManagedException<object>()
		// object Unity.Entities.World.AddSystemManaged<object>(object)
		// object Unity.Entities.World.GetExistingSystemManaged<object>()
		// object Unity.Entities.World.GetOrCreateSystemManaged<object>()
		// object UnityEngine.Component.GetComponent<object>()
		// object UnityEngine.Component.GetComponentInChildren<object>()
		// object UnityEngine.Component.GetComponentInParent<object>()
		// object[] UnityEngine.Component.GetComponents<object>()
		// object UnityEngine.GameObject.AddComponent<object>()
		// object UnityEngine.GameObject.GetComponent<object>()
		// object[] UnityEngine.GameObject.GetComponents<object>()
		// object[] UnityEngine.GameObject.GetComponentsInChildren<object>()
		// object[] UnityEngine.GameObject.GetComponentsInChildren<object>(bool)
		// bool UnityEngine.GameObject.TryGetComponent<object>(object&)
		// object UnityEngine.Object.FindObjectOfType<object>()
		// object UnityEngine.Object.Instantiate<object>(object)
		// object UnityEngine.Resources.Load<object>(string)
		// string string.Join<object>(string,System.Collections.Generic.IEnumerable<object>)
		// string string.JoinCore<object>(System.Char*,int,System.Collections.Generic.IEnumerable<object>)
	}
}