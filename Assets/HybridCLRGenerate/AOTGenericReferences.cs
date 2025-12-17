using System.Collections.Generic;
public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

	// {{ AOT assemblies
	public static readonly IReadOnlyList<string> PatchedAOTAssemblyList = new List<string>
	{
		"Cinemachine.dll",
		"Game.dll",
		"System.Core.dll",
		"System.dll",
		"UniTask.dll",
		"Unity.Addressables.dll",
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
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<BootPanel.<WaitForStartClickAsync>d__7>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<GameFramework.Core.ConfigManager.<>c.<<LoadLubanTablesAsync>b__8_0>d,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<GameFramework.Core.ConfigManager.<LoadLubanTablesAsync>d__8>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<GameFramework.Core.GameBootstrap.<InitializeGame>d__5>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<GameFramework.ECS.EntityFactory.<GetOrCreateEntityPrefabAsync>d__6,Unity.Entities.Entity>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<GameFramework.ECS.EntityFactory.<LoadEntityArchetypeAsync>d__4,Unity.Entities.Entity>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<GameFramework.Scenes.LoadingScene.<LoadGame>d__3>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<cfg.Tables.<>c__DisplayClass5_0.<<LoadAsync>b__0>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<cfg.Tables.<LoadAsync>d__5>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<BootPanel.<WaitForStartClickAsync>d__7>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<GameFramework.Core.ConfigManager.<>c.<<LoadLubanTablesAsync>b__8_0>d,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<GameFramework.Core.ConfigManager.<LoadLubanTablesAsync>d__8>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<GameFramework.Core.GameBootstrap.<InitializeGame>d__5>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<GameFramework.ECS.EntityFactory.<GetOrCreateEntityPrefabAsync>d__6,Unity.Entities.Entity>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<GameFramework.ECS.EntityFactory.<LoadEntityArchetypeAsync>d__4,Unity.Entities.Entity>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<GameFramework.Scenes.LoadingScene.<LoadGame>d__3>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<cfg.Tables.<>c__DisplayClass5_0.<<LoadAsync>b__0>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<cfg.Tables.<LoadAsync>d__5>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<Unity.Entities.Entity>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<GameFramework.ECS.Systems.GridEntityVisualizationSystem.<LoadGridResource>d__7>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<GameFramework.ECS.Systems.GridEntityVisualizationSystem.<LoadGridResource>d__7>
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
	// GameFramework.Singleton<object>
	// System.Action<Cysharp.Threading.Tasks.UniTask>
	// System.Action<GameFramework.Events.GamePausedEvent>
	// System.Action<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle,object>
	// System.Action<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object>>
	// System.Action<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Action<float>
	// System.Action<int,int>
	// System.Action<object,object>
	// System.Action<object>
	// System.ByReference<GameFramework.ECS.Components.GlobalInputComponent>
	// System.ByReference<GameFramework.ECS.Components.PlaceObjectRequest>
	// System.ByReference<GameFramework.ECS.Components.PlacementStateComponent>
	// System.ByReference<Unity.Entities.ChunkIndex>
	// System.ByReference<Unity.Entities.ComponentType>
	// System.ByReference<Unity.Entities.Entity>
	// System.ByReference<Unity.Entities.EntityQuery>
	// System.ByReference<Unity.Entities.EntityQueryBuilder.QueryTypes>
	// System.ByReference<Unity.Mathematics.int3>
	// System.ByReference<Unity.Transforms.LocalTransform>
	// System.ByReference<int>
	// System.Collections.Generic.ArraySortHelper<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.Generic.ArraySortHelper<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
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
	// System.Collections.Generic.Comparer<Unity.Entities.Entity>
	// System.Collections.Generic.Comparer<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Collections.Generic.Comparer<byte>
	// System.Collections.Generic.Comparer<object>
	// System.Collections.Generic.Dictionary.Enumerator<int,Unity.Entities.Entity>
	// System.Collections.Generic.Dictionary.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.Enumerator<object,Unity.Entities.SystemTypeIndex>
	// System.Collections.Generic.Dictionary.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,Unity.Entities.Entity>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,Unity.Entities.SystemTypeIndex>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection<int,Unity.Entities.Entity>
	// System.Collections.Generic.Dictionary.KeyCollection<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection<object,Unity.Entities.SystemTypeIndex>
	// System.Collections.Generic.Dictionary.KeyCollection<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,Unity.Entities.Entity>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,Unity.Entities.SystemTypeIndex>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection<int,Unity.Entities.Entity>
	// System.Collections.Generic.Dictionary.ValueCollection<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection<object,Unity.Entities.SystemTypeIndex>
	// System.Collections.Generic.Dictionary.ValueCollection<object,object>
	// System.Collections.Generic.Dictionary<int,Unity.Entities.Entity>
	// System.Collections.Generic.Dictionary<int,object>
	// System.Collections.Generic.Dictionary<object,Unity.Entities.SystemTypeIndex>
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
	// System.Collections.Generic.EqualityComparer<Unity.Entities.Entity>
	// System.Collections.Generic.EqualityComparer<Unity.Entities.SystemTypeIndex>
	// System.Collections.Generic.EqualityComparer<byte>
	// System.Collections.Generic.EqualityComparer<int>
	// System.Collections.Generic.EqualityComparer<object>
	// System.Collections.Generic.HashSet.Enumerator<object>
	// System.Collections.Generic.HashSet<object>
	// System.Collections.Generic.ICollection<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,Unity.Entities.Entity>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,Unity.Entities.SystemTypeIndex>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.ICollection<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Collections.Generic.ICollection<object>
	// System.Collections.Generic.IComparer<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.Generic.IComparer<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Collections.Generic.IComparer<object>
	// System.Collections.Generic.IEnumerable<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,Unity.Entities.Entity>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,Unity.Entities.SystemTypeIndex>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerable<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Collections.Generic.IEnumerable<object>
	// System.Collections.Generic.IEnumerator<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,Unity.Entities.Entity>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,Unity.Entities.SystemTypeIndex>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerator<Unity.Entities.QueryEnumerableWithEntity<Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRO<GameFramework.ECS.Components.PlaceObjectRequest>>>
	// System.Collections.Generic.IEnumerator<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Collections.Generic.IEnumerator<object>
	// System.Collections.Generic.IEqualityComparer<int>
	// System.Collections.Generic.IEqualityComparer<object>
	// System.Collections.Generic.IList<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.Generic.IList<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Collections.Generic.IList<object>
	// System.Collections.Generic.KeyValuePair<int,Unity.Entities.Entity>
	// System.Collections.Generic.KeyValuePair<int,object>
	// System.Collections.Generic.KeyValuePair<object,Unity.Entities.SystemTypeIndex>
	// System.Collections.Generic.KeyValuePair<object,object>
	// System.Collections.Generic.LinkedList.Enumerator<object>
	// System.Collections.Generic.LinkedList<object>
	// System.Collections.Generic.LinkedListNode<object>
	// System.Collections.Generic.List.Enumerator<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.Generic.List.Enumerator<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Collections.Generic.List.Enumerator<object>
	// System.Collections.Generic.List<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.Generic.List<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
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
	// System.Collections.Generic.ObjectComparer<Unity.Entities.Entity>
	// System.Collections.Generic.ObjectComparer<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Collections.Generic.ObjectComparer<byte>
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
	// System.Collections.Generic.ObjectEqualityComparer<Unity.Entities.Entity>
	// System.Collections.Generic.ObjectEqualityComparer<Unity.Entities.SystemTypeIndex>
	// System.Collections.Generic.ObjectEqualityComparer<byte>
	// System.Collections.Generic.ObjectEqualityComparer<int>
	// System.Collections.Generic.ObjectEqualityComparer<object>
	// System.Collections.ObjectModel.ReadOnlyCollection<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Collections.ObjectModel.ReadOnlyCollection<object>
	// System.Comparison<Cysharp.Threading.Tasks.UniTask>
	// System.Comparison<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
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
	// System.Func<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle,UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object>>
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
	// System.Func<object,object,object>
	// System.Func<object,object>
	// System.Func<object>
	// System.IEquatable<Unity.Mathematics.int3>
	// System.Predicate<Cysharp.Threading.Tasks.UniTask>
	// System.Predicate<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Predicate<object>
	// System.ReadOnlySpan<GameFramework.ECS.Components.GlobalInputComponent>
	// System.ReadOnlySpan<GameFramework.ECS.Components.PlaceObjectRequest>
	// System.ReadOnlySpan<GameFramework.ECS.Components.PlacementStateComponent>
	// System.ReadOnlySpan<Unity.Entities.ChunkIndex>
	// System.ReadOnlySpan<Unity.Entities.ComponentType>
	// System.ReadOnlySpan<Unity.Entities.Entity>
	// System.ReadOnlySpan<Unity.Entities.EntityQuery>
	// System.ReadOnlySpan<Unity.Entities.EntityQueryBuilder.QueryTypes>
	// System.ReadOnlySpan<Unity.Mathematics.int3>
	// System.ReadOnlySpan<Unity.Transforms.LocalTransform>
	// System.ReadOnlySpan<int>
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
	// System.Span<GameFramework.ECS.Components.GlobalInputComponent>
	// System.Span<GameFramework.ECS.Components.PlaceObjectRequest>
	// System.Span<GameFramework.ECS.Components.PlacementStateComponent>
	// System.Span<Unity.Entities.ChunkIndex>
	// System.Span<Unity.Entities.ComponentType>
	// System.Span<Unity.Entities.Entity>
	// System.Span<Unity.Entities.EntityQuery>
	// System.Span<Unity.Entities.EntityQueryBuilder.QueryTypes>
	// System.Span<Unity.Mathematics.int3>
	// System.Span<Unity.Transforms.LocalTransform>
	// System.Span<int>
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
	// Unity.Burst.SharedStatic<System.IntPtr>
	// Unity.Burst.SharedStatic<Unity.Entities.TypeIndex>
	// Unity.Burst.SharedStatic<int>
	// Unity.Collections.IIndexable<byte>
	// Unity.Collections.INativeList<byte>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList.ParallelReader<Unity.Entities.ChunkIndex>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList.ParallelReader<Unity.Entities.ComponentType>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList.ParallelReader<Unity.Entities.Entity>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList.ParallelReader<Unity.Entities.EntityQuery>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList.ParallelReader<Unity.Entities.EntityQueryBuilder.QueryTypes>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList.ParallelWriter<Unity.Entities.ChunkIndex>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList.ParallelWriter<Unity.Entities.ComponentType>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList.ParallelWriter<Unity.Entities.Entity>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList.ParallelWriter<Unity.Entities.EntityQuery>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList.ParallelWriter<Unity.Entities.EntityQueryBuilder.QueryTypes>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList.ReadOnly<Unity.Entities.ChunkIndex>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList.ReadOnly<Unity.Entities.ComponentType>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList.ReadOnly<Unity.Entities.Entity>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList.ReadOnly<Unity.Entities.EntityQuery>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList.ReadOnly<Unity.Entities.EntityQueryBuilder.QueryTypes>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList<Unity.Entities.ChunkIndex>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList<Unity.Entities.ComponentType>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList<Unity.Entities.Entity>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList<Unity.Entities.EntityQuery>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList<Unity.Entities.EntityQueryBuilder.QueryTypes>
	// Unity.Collections.LowLevel.Unsafe.UnsafeParallelHashMap.ParallelWriter<Unity.Mathematics.int3,Unity.Entities.Entity>
	// Unity.Collections.LowLevel.Unsafe.UnsafeParallelHashMap.ReadOnly<Unity.Mathematics.int3,Unity.Entities.Entity>
	// Unity.Collections.LowLevel.Unsafe.UnsafeParallelHashMap<Unity.Mathematics.int3,Unity.Entities.Entity>
	// Unity.Collections.LowLevel.Unsafe.UnsafeParallelHashMapBase<Unity.Mathematics.int3,Unity.Entities.Entity>
	// Unity.Collections.NativeArray.Enumerator<GameFramework.ECS.Components.GlobalInputComponent>
	// Unity.Collections.NativeArray.Enumerator<GameFramework.ECS.Components.PlaceObjectRequest>
	// Unity.Collections.NativeArray.Enumerator<GameFramework.ECS.Components.PlacementStateComponent>
	// Unity.Collections.NativeArray.Enumerator<Unity.Entities.ChunkIndex>
	// Unity.Collections.NativeArray.Enumerator<Unity.Entities.ComponentType>
	// Unity.Collections.NativeArray.Enumerator<Unity.Entities.Entity>
	// Unity.Collections.NativeArray.Enumerator<Unity.Entities.EntityQuery>
	// Unity.Collections.NativeArray.Enumerator<Unity.Entities.EntityQueryBuilder.QueryTypes>
	// Unity.Collections.NativeArray.Enumerator<Unity.Mathematics.int3>
	// Unity.Collections.NativeArray.Enumerator<Unity.Transforms.LocalTransform>
	// Unity.Collections.NativeArray.Enumerator<int>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<GameFramework.ECS.Components.GlobalInputComponent>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<GameFramework.ECS.Components.PlaceObjectRequest>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<GameFramework.ECS.Components.PlacementStateComponent>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<Unity.Entities.ChunkIndex>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<Unity.Entities.ComponentType>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<Unity.Entities.Entity>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<Unity.Entities.EntityQuery>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<Unity.Entities.EntityQueryBuilder.QueryTypes>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<Unity.Mathematics.int3>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<Unity.Transforms.LocalTransform>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<int>
	// Unity.Collections.NativeArray.ReadOnly<GameFramework.ECS.Components.GlobalInputComponent>
	// Unity.Collections.NativeArray.ReadOnly<GameFramework.ECS.Components.PlaceObjectRequest>
	// Unity.Collections.NativeArray.ReadOnly<GameFramework.ECS.Components.PlacementStateComponent>
	// Unity.Collections.NativeArray.ReadOnly<Unity.Entities.ChunkIndex>
	// Unity.Collections.NativeArray.ReadOnly<Unity.Entities.ComponentType>
	// Unity.Collections.NativeArray.ReadOnly<Unity.Entities.Entity>
	// Unity.Collections.NativeArray.ReadOnly<Unity.Entities.EntityQuery>
	// Unity.Collections.NativeArray.ReadOnly<Unity.Entities.EntityQueryBuilder.QueryTypes>
	// Unity.Collections.NativeArray.ReadOnly<Unity.Mathematics.int3>
	// Unity.Collections.NativeArray.ReadOnly<Unity.Transforms.LocalTransform>
	// Unity.Collections.NativeArray.ReadOnly<int>
	// Unity.Collections.NativeArray<GameFramework.ECS.Components.GlobalInputComponent>
	// Unity.Collections.NativeArray<GameFramework.ECS.Components.PlaceObjectRequest>
	// Unity.Collections.NativeArray<GameFramework.ECS.Components.PlacementStateComponent>
	// Unity.Collections.NativeArray<Unity.Entities.ChunkIndex>
	// Unity.Collections.NativeArray<Unity.Entities.ComponentType>
	// Unity.Collections.NativeArray<Unity.Entities.Entity>
	// Unity.Collections.NativeArray<Unity.Entities.EntityQuery>
	// Unity.Collections.NativeArray<Unity.Entities.EntityQueryBuilder.QueryTypes>
	// Unity.Collections.NativeArray<Unity.Mathematics.int3>
	// Unity.Collections.NativeArray<Unity.Transforms.LocalTransform>
	// Unity.Collections.NativeArray<int>
	// Unity.Collections.NativeKeyValueArrays<Unity.Mathematics.int3,Unity.Entities.Entity>
	// Unity.Collections.NativeList.ParallelWriter<Unity.Entities.Entity>
	// Unity.Collections.NativeList<Unity.Entities.Entity>
	// Unity.Collections.NativeParallelHashMap.ParallelWriter<Unity.Mathematics.int3,Unity.Entities.Entity>
	// Unity.Collections.NativeParallelHashMap.ReadOnly<Unity.Mathematics.int3,Unity.Entities.Entity>
	// Unity.Collections.NativeParallelHashMap<Unity.Mathematics.int3,Unity.Entities.Entity>
	// Unity.Entities.ComponentLookup<GameFramework.ECS.Components.PlacementStateComponent>
	// Unity.Entities.ComponentLookup<Unity.Transforms.LocalTransform>
	// Unity.Entities.ComponentTypeHandle<GameFramework.ECS.Components.DestroyTag>
	// Unity.Entities.ComponentTypeHandle<GameFramework.ECS.Components.GridPositionComponent>
	// Unity.Entities.ComponentTypeHandle<GameFramework.ECS.Components.ObjectSizeComponent>
	// Unity.Entities.ComponentTypeHandle<GameFramework.ECS.Components.PlaceObjectRequest>
	// Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRO<GameFramework.ECS.Components.PlaceObjectRequest>
	// Unity.Entities.JobChunkExtensions.JobChunkProducer.ExecuteJobFunction<GameFramework.ECS.Systems.DestroySystem.DestroyEntityJob>
	// Unity.Entities.JobChunkExtensions.JobChunkProducer.ExecuteJobFunction<GameFramework.ECS.Systems.GridOccupancySystem.GridOccupancySystem_38F63872_LambdaJob_0_Job>
	// Unity.Entities.JobChunkExtensions.JobChunkProducer<GameFramework.ECS.Systems.DestroySystem.DestroyEntityJob>
	// Unity.Entities.JobChunkExtensions.JobChunkProducer<GameFramework.ECS.Systems.GridOccupancySystem.GridOccupancySystem_38F63872_LambdaJob_0_Job>
	// Unity.Entities.QueryEnumerableWithEntity<Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRO<GameFramework.ECS.Components.PlaceObjectRequest>>
	// Unity.Entities.RefRO<GameFramework.ECS.Components.GlobalInputComponent>
	// Unity.Entities.RefRO<GameFramework.ECS.Components.PlaceObjectRequest>
	// Unity.Entities.RefRO<GameFramework.ECS.Components.PlacementStateComponent>
	// Unity.Entities.RefRO<Unity.Transforms.LocalTransform>
	// Unity.Entities.RefRW<GameFramework.ECS.Components.GlobalInputComponent>
	// Unity.Entities.RefRW<GameFramework.ECS.Components.PlacementStateComponent>
	// Unity.Entities.RefRW<Unity.Transforms.LocalTransform>
	// Unity.Entities.TypeManager.SharedTypeIndex<GameFramework.ECS.Components.BuildingTag>
	// Unity.Entities.TypeManager.SharedTypeIndex<GameFramework.ECS.Components.DestroyTag>
	// Unity.Entities.TypeManager.SharedTypeIndex<GameFramework.ECS.Components.GlobalInputComponent>
	// Unity.Entities.TypeManager.SharedTypeIndex<GameFramework.ECS.Components.GridConfigComponent>
	// Unity.Entities.TypeManager.SharedTypeIndex<GameFramework.ECS.Components.GridPositionComponent>
	// Unity.Entities.TypeManager.SharedTypeIndex<GameFramework.ECS.Components.InputComponent>
	// Unity.Entities.TypeManager.SharedTypeIndex<GameFramework.ECS.Components.IslandTag>
	// Unity.Entities.TypeManager.SharedTypeIndex<GameFramework.ECS.Components.ObjectSizeComponent>
	// Unity.Entities.TypeManager.SharedTypeIndex<GameFramework.ECS.Components.PlaceObjectRequest>
	// Unity.Entities.TypeManager.SharedTypeIndex<GameFramework.ECS.Components.PlacementStateComponent>
	// Unity.Entities.TypeManager.SharedTypeIndex<GameFramework.ECS.Components.PositionComponent>
	// Unity.Entities.TypeManager.SharedTypeIndex<GameFramework.ECS.Components.PreviewTag>
	// Unity.Entities.TypeManager.SharedTypeIndex<GameFramework.ECS.Components.RotationComponent>
	// Unity.Entities.TypeManager.SharedTypeIndex<object>
	// UnityEngine.AddressableAssets.AddressablesImpl.<>c__DisplayClass79_0<object>
	// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationBase.<>c__DisplayClass60_0<object>
	// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationBase.<>c__DisplayClass61_0<object>
	// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationBase<object>
	// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle.<>c<object>
	// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object>
	// UnityEngine.ResourceManagement.ChainOperationTypelessDepedency<object>
	// UnityEngine.ResourceManagement.ResourceManager.CompletedOperation<object>
	// UnityEngine.ResourceManagement.Util.GlobalLinkedListNodeCache<object>
	// UnityEngine.ResourceManagement.Util.LinkedListNodeCache<object>
	// }}

	public void RefMethods()
	{
		// object Cinemachine.CinemachineVirtualCamera.AddCinemachineComponent<object>()
		// object Cinemachine.CinemachineVirtualCamera.GetCinemachineComponent<object>()
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,BootPanel.<WaitForStartClickAsync>d__7>(Cysharp.Threading.Tasks.UniTask.Awaiter&,BootPanel.<WaitForStartClickAsync>d__7&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,GameFramework.Core.ConfigManager.<LoadLubanTablesAsync>d__8>(Cysharp.Threading.Tasks.UniTask.Awaiter&,GameFramework.Core.ConfigManager.<LoadLubanTablesAsync>d__8&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,GameFramework.Core.GameBootstrap.<InitializeGame>d__5>(Cysharp.Threading.Tasks.UniTask.Awaiter&,GameFramework.Core.GameBootstrap.<InitializeGame>d__5&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,GameFramework.Scenes.LoadingScene.<LoadGame>d__3>(Cysharp.Threading.Tasks.UniTask.Awaiter&,GameFramework.Scenes.LoadingScene.<LoadGame>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,cfg.Tables.<LoadAsync>d__5>(Cysharp.Threading.Tasks.UniTask.Awaiter&,cfg.Tables.<LoadAsync>d__5&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,cfg.Tables.<>c__DisplayClass5_0.<<LoadAsync>b__0>d>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,cfg.Tables.<>c__DisplayClass5_0.<<LoadAsync>b__0>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<Unity.Entities.Entity>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<Unity.Entities.Entity>,GameFramework.ECS.EntityFactory.<LoadEntityArchetypeAsync>d__4>(Cysharp.Threading.Tasks.UniTask.Awaiter<Unity.Entities.Entity>&,GameFramework.ECS.EntityFactory.<LoadEntityArchetypeAsync>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<Unity.Entities.Entity>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,GameFramework.ECS.EntityFactory.<GetOrCreateEntityPrefabAsync>d__6>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,GameFramework.ECS.EntityFactory.<GetOrCreateEntityPrefabAsync>d__6&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,GameFramework.Core.ConfigManager.<>c.<<LoadLubanTablesAsync>b__8_0>d>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,GameFramework.Core.ConfigManager.<>c.<<LoadLubanTablesAsync>b__8_0>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<BootPanel.<WaitForStartClickAsync>d__7>(BootPanel.<WaitForStartClickAsync>d__7&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<GameFramework.Core.ConfigManager.<LoadLubanTablesAsync>d__8>(GameFramework.Core.ConfigManager.<LoadLubanTablesAsync>d__8&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<GameFramework.Core.GameBootstrap.<InitializeGame>d__5>(GameFramework.Core.GameBootstrap.<InitializeGame>d__5&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<GameFramework.Scenes.LoadingScene.<LoadGame>d__3>(GameFramework.Scenes.LoadingScene.<LoadGame>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<cfg.Tables.<>c__DisplayClass5_0.<<LoadAsync>b__0>d>(cfg.Tables.<>c__DisplayClass5_0.<<LoadAsync>b__0>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<cfg.Tables.<LoadAsync>d__5>(cfg.Tables.<LoadAsync>d__5&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<Unity.Entities.Entity>.Start<GameFramework.ECS.EntityFactory.<GetOrCreateEntityPrefabAsync>d__6>(GameFramework.ECS.EntityFactory.<GetOrCreateEntityPrefabAsync>d__6&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<Unity.Entities.Entity>.Start<GameFramework.ECS.EntityFactory.<LoadEntityArchetypeAsync>d__4>(GameFramework.ECS.EntityFactory.<LoadEntityArchetypeAsync>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<GameFramework.Core.ConfigManager.<>c.<<LoadLubanTablesAsync>b__8_0>d>(GameFramework.Core.ConfigManager.<>c.<<LoadLubanTablesAsync>b__8_0>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<GameFramework.Managers.ResourceManager.<LoadAssetAsync>d__2<object>>(GameFramework.Managers.ResourceManager.<LoadAssetAsync>d__2<object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<Unity.Entities.Entity>,GameFramework.ECS.Systems.GridEntityVisualizationSystem.<LoadGridResource>d__7>(Cysharp.Threading.Tasks.UniTask.Awaiter<Unity.Entities.Entity>&,GameFramework.ECS.Systems.GridEntityVisualizationSystem.<LoadGridResource>d__7&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<GameFramework.ECS.Systems.GridEntityVisualizationSystem.<LoadGridResource>d__7>(GameFramework.ECS.Systems.GridEntityVisualizationSystem.<LoadGridResource>d__7&)
		// System.Void GameFramework.Events.EventManager.Publish<GameFramework.Events.GamePausedEvent>(GameFramework.Events.GamePausedEvent)
		// Cysharp.Threading.Tasks.UniTask<object> GameFramework.Managers.ResourceManager.LoadAssetAsync<object>(string)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,GameFramework.Core.GameBootstrap.<Awake>d__4>(Cysharp.Threading.Tasks.UniTask.Awaiter&,GameFramework.Core.GameBootstrap.<Awake>d__4&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,GameFramework.Scenes.LoadingScene.<Start>d__2>(Cysharp.Threading.Tasks.UniTask.Awaiter&,GameFramework.Scenes.LoadingScene.<Start>d__2&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<GameFramework.Core.GameBootstrap.<Awake>d__4>(GameFramework.Core.GameBootstrap.<Awake>d__4&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<GameFramework.Scenes.LoadingScene.<Start>d__2>(GameFramework.Scenes.LoadingScene.<Start>d__2&)
		// object& System.Runtime.CompilerServices.Unsafe.As<object,object>(object&)
		// System.Void* System.Runtime.CompilerServices.Unsafe.AsPointer<object>(object&)
		// long Unity.Burst.BurstRuntime.GetHashCode64<GameFramework.ECS.Systems.DestroySystem>()
		// long Unity.Burst.BurstRuntime.GetHashCode64<GameFramework.ECS.Systems.MoveSystem>()
		// long Unity.Burst.BurstRuntime.GetHashCode64<GameFramework.ECS.Systems.ObjectSpawnSystem>()
		// long Unity.Burst.BurstRuntime.GetHashCode64<object>()
		// Unity.Collections.CopyError Unity.Collections.FixedStringMethods.CopyFromTruncated<Unity.Collections.FixedString64Bytes,Unity.Collections.FixedString128Bytes>(Unity.Collections.FixedString64Bytes&,Unity.Collections.FixedString128Bytes&)
		// System.Void* Unity.Collections.LowLevel.Unsafe.UnsafeUtility.AddressOf<Unity.Entities.JobChunkExtensions.JobChunkWrapper<GameFramework.ECS.Systems.DestroySystem.DestroyEntityJob>>(Unity.Entities.JobChunkExtensions.JobChunkWrapper<GameFramework.ECS.Systems.DestroySystem.DestroyEntityJob>&)
		// System.Void* Unity.Collections.LowLevel.Unsafe.UnsafeUtility.AddressOf<Unity.Entities.JobChunkExtensions.JobChunkWrapper<GameFramework.ECS.Systems.GridOccupancySystem.GridOccupancySystem_38F63872_LambdaJob_0_Job>>(Unity.Entities.JobChunkExtensions.JobChunkWrapper<GameFramework.ECS.Systems.GridOccupancySystem.GridOccupancySystem_38F63872_LambdaJob_0_Job>&)
		// GameFramework.ECS.Components.GlobalInputComponent& Unity.Collections.LowLevel.Unsafe.UnsafeUtility.AsRef<GameFramework.ECS.Components.GlobalInputComponent>(System.Void*)
		// GameFramework.ECS.Components.GridConfigComponent& Unity.Collections.LowLevel.Unsafe.UnsafeUtility.AsRef<GameFramework.ECS.Components.GridConfigComponent>(System.Void*)
		// GameFramework.ECS.Components.GridPositionComponent& Unity.Collections.LowLevel.Unsafe.UnsafeUtility.AsRef<GameFramework.ECS.Components.GridPositionComponent>(System.Void*)
		// GameFramework.ECS.Components.ObjectSizeComponent& Unity.Collections.LowLevel.Unsafe.UnsafeUtility.AsRef<GameFramework.ECS.Components.ObjectSizeComponent>(System.Void*)
		// Unity.Entities.EndSimulationEntityCommandBufferSystem.Singleton& Unity.Collections.LowLevel.Unsafe.UnsafeUtility.AsRef<Unity.Entities.EndSimulationEntityCommandBufferSystem.Singleton>(System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyStructureToPtr<GameFramework.ECS.Components.BuildingTag>(GameFramework.ECS.Components.BuildingTag&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyStructureToPtr<GameFramework.ECS.Components.GridConfigComponent>(GameFramework.ECS.Components.GridConfigComponent&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyStructureToPtr<GameFramework.ECS.Components.GridPositionComponent>(GameFramework.ECS.Components.GridPositionComponent&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyStructureToPtr<GameFramework.ECS.Components.IslandTag>(GameFramework.ECS.Components.IslandTag&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyStructureToPtr<GameFramework.ECS.Components.ObjectSizeComponent>(GameFramework.ECS.Components.ObjectSizeComponent&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyStructureToPtr<GameFramework.ECS.Components.PlaceObjectRequest>(GameFramework.ECS.Components.PlaceObjectRequest&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyStructureToPtr<Unity.Transforms.LocalTransform>(Unity.Transforms.LocalTransform&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyStructureToPtr<GameFramework.ECS.Components.BuildingTag>(GameFramework.ECS.Components.BuildingTag&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyStructureToPtr<GameFramework.ECS.Components.GridConfigComponent>(GameFramework.ECS.Components.GridConfigComponent&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyStructureToPtr<GameFramework.ECS.Components.GridPositionComponent>(GameFramework.ECS.Components.GridPositionComponent&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyStructureToPtr<GameFramework.ECS.Components.IslandTag>(GameFramework.ECS.Components.IslandTag&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyStructureToPtr<GameFramework.ECS.Components.ObjectSizeComponent>(GameFramework.ECS.Components.ObjectSizeComponent&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyStructureToPtr<GameFramework.ECS.Components.PlaceObjectRequest>(GameFramework.ECS.Components.PlaceObjectRequest&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyStructureToPtr<Unity.Transforms.LocalTransform>(Unity.Transforms.LocalTransform&,System.Void*)
		// int Unity.Collections.LowLevel.Unsafe.UnsafeUtility.SizeOf<GameFramework.ECS.Components.BuildingTag>()
		// int Unity.Collections.LowLevel.Unsafe.UnsafeUtility.SizeOf<GameFramework.ECS.Components.GridPositionComponent>()
		// int Unity.Collections.LowLevel.Unsafe.UnsafeUtility.SizeOf<GameFramework.ECS.Components.IslandTag>()
		// int Unity.Collections.LowLevel.Unsafe.UnsafeUtility.SizeOf<GameFramework.ECS.Components.ObjectSizeComponent>()
		// int Unity.Collections.LowLevel.Unsafe.UnsafeUtility.SizeOf<GameFramework.ECS.Components.PlaceObjectRequest>()
		// int Unity.Collections.LowLevel.Unsafe.UnsafeUtility.SizeOf<Unity.Transforms.LocalTransform>()
		// System.Void* Unity.Entities.ArchetypeChunk.GetRequiredComponentDataPtrRO<GameFramework.ECS.Components.GridPositionComponent>(Unity.Entities.ComponentTypeHandle<GameFramework.ECS.Components.GridPositionComponent>&)
		// System.Void* Unity.Entities.ArchetypeChunk.GetRequiredComponentDataPtrRO<GameFramework.ECS.Components.ObjectSizeComponent>(Unity.Entities.ComponentTypeHandle<GameFramework.ECS.Components.ObjectSizeComponent>&)
		// System.Void Unity.Entities.ComponentSystemBase.RequireForUpdate<GameFramework.ECS.Components.GlobalInputComponent>()
		// System.Void Unity.Entities.ComponentSystemBase.RequireForUpdate<GameFramework.ECS.Components.GridConfigComponent>()
		// System.Void Unity.Entities.ComponentSystemBase.RequireForUpdate<GameFramework.ECS.Components.PlacementStateComponent>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadOnly<GameFramework.ECS.Components.DestroyTag>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadOnly<GameFramework.ECS.Components.GlobalInputComponent>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadOnly<GameFramework.ECS.Components.GridConfigComponent>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadOnly<GameFramework.ECS.Components.GridPositionComponent>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadOnly<GameFramework.ECS.Components.ObjectSizeComponent>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadOnly<GameFramework.ECS.Components.PlaceObjectRequest>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadOnly<GameFramework.ECS.Components.PlacementStateComponent>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadOnly<Unity.Transforms.LocalTransform>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadWrite<GameFramework.ECS.Components.BuildingTag>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadWrite<GameFramework.ECS.Components.DestroyTag>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadWrite<GameFramework.ECS.Components.GlobalInputComponent>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadWrite<GameFramework.ECS.Components.GridConfigComponent>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadWrite<GameFramework.ECS.Components.GridPositionComponent>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadWrite<GameFramework.ECS.Components.IslandTag>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadWrite<GameFramework.ECS.Components.ObjectSizeComponent>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadWrite<GameFramework.ECS.Components.PlaceObjectRequest>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadWrite<GameFramework.ECS.Components.PlacementStateComponent>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadWrite<GameFramework.ECS.Components.PreviewTag>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadWrite<Unity.Entities.Prefab>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadWrite<Unity.Transforms.LocalTransform>()
		// System.Void Unity.Entities.EntityCommandBuffer.AddComponent<GameFramework.ECS.Components.BuildingTag>(Unity.Entities.Entity,GameFramework.ECS.Components.BuildingTag)
		// System.Void Unity.Entities.EntityCommandBuffer.AddComponent<GameFramework.ECS.Components.GridPositionComponent>(Unity.Entities.Entity,GameFramework.ECS.Components.GridPositionComponent)
		// System.Void Unity.Entities.EntityCommandBuffer.AddComponent<GameFramework.ECS.Components.IslandTag>(Unity.Entities.Entity,GameFramework.ECS.Components.IslandTag)
		// System.Void Unity.Entities.EntityCommandBuffer.AddComponent<GameFramework.ECS.Components.ObjectSizeComponent>(Unity.Entities.Entity,GameFramework.ECS.Components.ObjectSizeComponent)
		// System.Void Unity.Entities.EntityCommandBuffer.AddComponent<Unity.Transforms.LocalTransform>(Unity.Entities.Entity,Unity.Transforms.LocalTransform)
		// System.Void Unity.Entities.EntityCommandBufferData.AddEntityComponentTypeWithValueCommand<GameFramework.ECS.Components.BuildingTag>(Unity.Entities.EntityCommandBufferChain*,int,Unity.Entities.ECBCommand,Unity.Entities.Entity,GameFramework.ECS.Components.BuildingTag)
		// System.Void Unity.Entities.EntityCommandBufferData.AddEntityComponentTypeWithValueCommand<GameFramework.ECS.Components.GridPositionComponent>(Unity.Entities.EntityCommandBufferChain*,int,Unity.Entities.ECBCommand,Unity.Entities.Entity,GameFramework.ECS.Components.GridPositionComponent)
		// System.Void Unity.Entities.EntityCommandBufferData.AddEntityComponentTypeWithValueCommand<GameFramework.ECS.Components.IslandTag>(Unity.Entities.EntityCommandBufferChain*,int,Unity.Entities.ECBCommand,Unity.Entities.Entity,GameFramework.ECS.Components.IslandTag)
		// System.Void Unity.Entities.EntityCommandBufferData.AddEntityComponentTypeWithValueCommand<GameFramework.ECS.Components.ObjectSizeComponent>(Unity.Entities.EntityCommandBufferChain*,int,Unity.Entities.ECBCommand,Unity.Entities.Entity,GameFramework.ECS.Components.ObjectSizeComponent)
		// System.Void Unity.Entities.EntityCommandBufferData.AddEntityComponentTypeWithValueCommand<Unity.Transforms.LocalTransform>(Unity.Entities.EntityCommandBufferChain*,int,Unity.Entities.ECBCommand,Unity.Entities.Entity,Unity.Transforms.LocalTransform)
		// System.Void Unity.Entities.EntityDataAccess.SetComponentData<GameFramework.ECS.Components.GridConfigComponent>(Unity.Entities.Entity,GameFramework.ECS.Components.GridConfigComponent,Unity.Entities.SystemHandle&)
		// System.Void Unity.Entities.EntityDataAccess.SetComponentData<GameFramework.ECS.Components.ObjectSizeComponent>(Unity.Entities.Entity,GameFramework.ECS.Components.ObjectSizeComponent,Unity.Entities.SystemHandle&)
		// System.Void Unity.Entities.EntityDataAccess.SetComponentData<GameFramework.ECS.Components.PlaceObjectRequest>(Unity.Entities.Entity,GameFramework.ECS.Components.PlaceObjectRequest,Unity.Entities.SystemHandle&)
		// System.Void Unity.Entities.EntityDataAccess.SetComponentData<Unity.Transforms.LocalTransform>(Unity.Entities.Entity,Unity.Transforms.LocalTransform,Unity.Entities.SystemHandle&)
		// bool Unity.Entities.EntityManager.AddComponent<GameFramework.ECS.Components.PreviewTag>(Unity.Entities.Entity)
		// bool Unity.Entities.EntityManager.AddComponent<Unity.Entities.Prefab>(Unity.Entities.Entity)
		// bool Unity.Entities.EntityManager.AddComponentData<GameFramework.ECS.Components.ObjectSizeComponent>(Unity.Entities.Entity,GameFramework.ECS.Components.ObjectSizeComponent)
		// bool Unity.Entities.EntityManager.AddComponentData<GameFramework.ECS.Components.PlaceObjectRequest>(Unity.Entities.Entity,GameFramework.ECS.Components.PlaceObjectRequest)
		// bool Unity.Entities.EntityManager.AddComponentData<Unity.Transforms.LocalTransform>(Unity.Entities.Entity,Unity.Transforms.LocalTransform)
		// System.Void Unity.Entities.EntityManager.CompleteDependencyBeforeRO<GameFramework.ECS.Components.PlaceObjectRequest>()
		// System.Void Unity.Entities.EntityManager.CompleteDependencyBeforeRO<GameFramework.ECS.Components.PlacementStateComponent>()
		// System.Void Unity.Entities.EntityManager.CompleteDependencyBeforeRW<GameFramework.ECS.Components.PlacementStateComponent>()
		// System.Void Unity.Entities.EntityManager.CompleteDependencyBeforeRW<Unity.Transforms.LocalTransform>()
		// Unity.Entities.Entity Unity.Entities.EntityManager.CreateSingleton<GameFramework.ECS.Components.GlobalInputComponent>(Unity.Collections.FixedString64Bytes)
		// Unity.Entities.Entity Unity.Entities.EntityManager.CreateSingleton<GameFramework.ECS.Components.GridConfigComponent>(GameFramework.ECS.Components.GridConfigComponent,Unity.Collections.FixedString64Bytes)
		// Unity.Entities.Entity Unity.Entities.EntityManager.CreateSingleton<GameFramework.ECS.Components.PlacementStateComponent>(Unity.Collections.FixedString64Bytes)
		// Unity.Entities.Entity Unity.Entities.EntityManager.CreateSingletonEntityInternal<GameFramework.ECS.Components.GlobalInputComponent>(Unity.Collections.FixedString64Bytes)
		// Unity.Entities.Entity Unity.Entities.EntityManager.CreateSingletonEntityInternal<GameFramework.ECS.Components.GridConfigComponent>(Unity.Collections.FixedString64Bytes)
		// Unity.Entities.Entity Unity.Entities.EntityManager.CreateSingletonEntityInternal<GameFramework.ECS.Components.PlacementStateComponent>(Unity.Collections.FixedString64Bytes)
		// Unity.Entities.ComponentLookup<GameFramework.ECS.Components.PlacementStateComponent> Unity.Entities.EntityManager.GetComponentLookup<GameFramework.ECS.Components.PlacementStateComponent>(Unity.Entities.TypeIndex,bool)
		// Unity.Entities.ComponentLookup<GameFramework.ECS.Components.PlacementStateComponent> Unity.Entities.EntityManager.GetComponentLookup<GameFramework.ECS.Components.PlacementStateComponent>(bool)
		// Unity.Entities.ComponentLookup<Unity.Transforms.LocalTransform> Unity.Entities.EntityManager.GetComponentLookup<Unity.Transforms.LocalTransform>(Unity.Entities.TypeIndex,bool)
		// Unity.Entities.ComponentLookup<Unity.Transforms.LocalTransform> Unity.Entities.EntityManager.GetComponentLookup<Unity.Transforms.LocalTransform>(bool)
		// Unity.Entities.ComponentTypeHandle<GameFramework.ECS.Components.DestroyTag> Unity.Entities.EntityManager.GetComponentTypeHandle<GameFramework.ECS.Components.DestroyTag>(bool)
		// Unity.Entities.ComponentTypeHandle<GameFramework.ECS.Components.GridPositionComponent> Unity.Entities.EntityManager.GetComponentTypeHandle<GameFramework.ECS.Components.GridPositionComponent>(bool)
		// Unity.Entities.ComponentTypeHandle<GameFramework.ECS.Components.ObjectSizeComponent> Unity.Entities.EntityManager.GetComponentTypeHandle<GameFramework.ECS.Components.ObjectSizeComponent>(bool)
		// Unity.Entities.ComponentTypeHandle<GameFramework.ECS.Components.PlaceObjectRequest> Unity.Entities.EntityManager.GetComponentTypeHandle<GameFramework.ECS.Components.PlaceObjectRequest>(bool)
		// bool Unity.Entities.EntityManager.RemoveComponent<Unity.Entities.Prefab>(Unity.Entities.Entity)
		// System.Void Unity.Entities.EntityManager.SetComponentData<GameFramework.ECS.Components.GridConfigComponent>(Unity.Entities.Entity,GameFramework.ECS.Components.GridConfigComponent)
		// System.Void Unity.Entities.EntityManager.SetComponentData<GameFramework.ECS.Components.ObjectSizeComponent>(Unity.Entities.Entity,GameFramework.ECS.Components.ObjectSizeComponent)
		// System.Void Unity.Entities.EntityManager.SetComponentData<GameFramework.ECS.Components.PlaceObjectRequest>(Unity.Entities.Entity,GameFramework.ECS.Components.PlaceObjectRequest)
		// System.Void Unity.Entities.EntityManager.SetComponentData<Unity.Transforms.LocalTransform>(Unity.Entities.Entity,Unity.Transforms.LocalTransform)
		// GameFramework.ECS.Components.GlobalInputComponent Unity.Entities.EntityQuery.GetSingleton<GameFramework.ECS.Components.GlobalInputComponent>()
		// GameFramework.ECS.Components.GridConfigComponent Unity.Entities.EntityQuery.GetSingleton<GameFramework.ECS.Components.GridConfigComponent>()
		// Unity.Entities.EndSimulationEntityCommandBufferSystem.Singleton Unity.Entities.EntityQuery.GetSingleton<Unity.Entities.EndSimulationEntityCommandBufferSystem.Singleton>()
		// bool Unity.Entities.EntityQuery.HasSingleton<GameFramework.ECS.Components.GlobalInputComponent>()
		// System.Void Unity.Entities.EntityQuery.SetSingleton<GameFramework.ECS.Components.GlobalInputComponent>(GameFramework.ECS.Components.GlobalInputComponent)
		// bool Unity.Entities.EntityQuery.TryGetSingleton<GameFramework.ECS.Components.GridConfigComponent>(GameFramework.ECS.Components.GridConfigComponent&)
		// Unity.Entities.EntityQueryBuilder Unity.Entities.EntityQueryBuilder.WithAll<GameFramework.ECS.Components.DestroyTag>()
		// Unity.Entities.EntityQueryBuilder Unity.Entities.EntityQueryBuilder.WithAll<GameFramework.ECS.Components.GlobalInputComponent>()
		// Unity.Entities.EntityQueryBuilder Unity.Entities.EntityQueryBuilder.WithAll<GameFramework.ECS.Components.GridConfigComponent>()
		// Unity.Entities.EntityQueryBuilder Unity.Entities.EntityQueryBuilder.WithAll<GameFramework.ECS.Components.GridPositionComponent>()
		// Unity.Entities.EntityQueryBuilder Unity.Entities.EntityQueryBuilder.WithAll<GameFramework.ECS.Components.ObjectSizeComponent>()
		// Unity.Entities.EntityQueryBuilder Unity.Entities.EntityQueryBuilder.WithAll<GameFramework.ECS.Components.PlaceObjectRequest>()
		// Unity.Entities.EntityQueryBuilder Unity.Entities.EntityQueryBuilder.WithAll<GameFramework.ECS.Components.PlacementStateComponent>()
		// Unity.Entities.EntityQueryBuilder Unity.Entities.EntityQueryBuilder.WithAll<Unity.Entities.EndSimulationEntityCommandBufferSystem.Singleton>()
		// Unity.Entities.EntityQueryBuilder Unity.Entities.EntityQueryBuilder.WithAllRW<GameFramework.ECS.Components.GlobalInputComponent>()
		// Unity.Entities.EntityQueryBuilder Unity.Entities.EntityQueryBuilder.WithNone<GameFramework.ECS.Components.PreviewTag>()
		// GameFramework.ECS.Components.GlobalInputComponent Unity.Entities.EntityQueryImpl.GetSingleton<GameFramework.ECS.Components.GlobalInputComponent>()
		// GameFramework.ECS.Components.GridConfigComponent Unity.Entities.EntityQueryImpl.GetSingleton<GameFramework.ECS.Components.GridConfigComponent>()
		// Unity.Entities.EndSimulationEntityCommandBufferSystem.Singleton Unity.Entities.EntityQueryImpl.GetSingleton<Unity.Entities.EndSimulationEntityCommandBufferSystem.Singleton>()
		// Unity.Entities.RefRW<GameFramework.ECS.Components.GlobalInputComponent> Unity.Entities.EntityQueryImpl.GetSingletonRW<GameFramework.ECS.Components.GlobalInputComponent>()
		// bool Unity.Entities.EntityQueryImpl.HasSingleton<GameFramework.ECS.Components.GlobalInputComponent>()
		// bool Unity.Entities.EntityQueryImpl.HasSingleton<GameFramework.ECS.Components.GridConfigComponent>()
		// System.Void Unity.Entities.EntityQueryImpl.SetSingleton<GameFramework.ECS.Components.GlobalInputComponent>(GameFramework.ECS.Components.GlobalInputComponent)
		// bool Unity.Entities.EntityQueryImpl.TryGetSingleton<GameFramework.ECS.Components.GridConfigComponent>(GameFramework.ECS.Components.GridConfigComponent&)
		// GameFramework.ECS.Components.PlacementStateComponent Unity.Entities.Internal.InternalCompilerInterface.GetComponentAfterCompletingDependency<GameFramework.ECS.Components.PlacementStateComponent>(Unity.Entities.ComponentLookup<GameFramework.ECS.Components.PlacementStateComponent>&,Unity.Entities.SystemState&,Unity.Entities.Entity)
		// System.Void Unity.Entities.Internal.InternalCompilerInterface.SetComponentAfterCompletingDependency<GameFramework.ECS.Components.PlacementStateComponent>(Unity.Entities.ComponentLookup<GameFramework.ECS.Components.PlacementStateComponent>&,Unity.Entities.SystemState&,GameFramework.ECS.Components.PlacementStateComponent,Unity.Entities.Entity)
		// System.Void Unity.Entities.Internal.InternalCompilerInterface.SetComponentAfterCompletingDependency<Unity.Transforms.LocalTransform>(Unity.Entities.ComponentLookup<Unity.Transforms.LocalTransform>&,Unity.Entities.SystemState&,Unity.Transforms.LocalTransform,Unity.Entities.Entity)
		// System.IntPtr Unity.Entities.Internal.InternalCompilerInterface.UnsafeGetChunkNativeArrayReadOnlyIntPtr<GameFramework.ECS.Components.GridPositionComponent>(Unity.Entities.ArchetypeChunk,Unity.Entities.ComponentTypeHandle<GameFramework.ECS.Components.GridPositionComponent>&)
		// System.IntPtr Unity.Entities.Internal.InternalCompilerInterface.UnsafeGetChunkNativeArrayReadOnlyIntPtr<GameFramework.ECS.Components.ObjectSizeComponent>(Unity.Entities.ArchetypeChunk,Unity.Entities.ComponentTypeHandle<GameFramework.ECS.Components.ObjectSizeComponent>&)
		// System.IntPtr Unity.Entities.Internal.InternalCompilerInterface.UnsafeGetChunkNativeArrayReadOnlyIntPtrWithoutChecks<GameFramework.ECS.Components.PlaceObjectRequest>(Unity.Entities.ArchetypeChunk&,Unity.Entities.ComponentTypeHandle<GameFramework.ECS.Components.PlaceObjectRequest>&)
		// Unity.Entities.Entity Unity.Entities.Internal.InternalCompilerInterface.UnsafeGetCopyOfNativeArrayPtrElement<Unity.Entities.Entity>(System.IntPtr,int)
		// GameFramework.ECS.Components.GridPositionComponent& Unity.Entities.Internal.InternalCompilerInterface.UnsafeGetRefToNativeArrayPtrElement<GameFramework.ECS.Components.GridPositionComponent>(System.IntPtr,int)
		// GameFramework.ECS.Components.ObjectSizeComponent& Unity.Entities.Internal.InternalCompilerInterface.UnsafeGetRefToNativeArrayPtrElement<GameFramework.ECS.Components.ObjectSizeComponent>(System.IntPtr,int)
		// Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRO<GameFramework.ECS.Components.PlaceObjectRequest> Unity.Entities.Internal.InternalCompilerInterface.UnsafeGetUncheckedRefRO<GameFramework.ECS.Components.PlaceObjectRequest>(System.IntPtr,int)
		// Unity.Jobs.JobHandle Unity.Entities.Internal.InternalCompilerInterface.JobChunkInterface.ScheduleParallel<GameFramework.ECS.Systems.GridOccupancySystem.GridOccupancySystem_38F63872_LambdaJob_0_Job>(GameFramework.ECS.Systems.GridOccupancySystem.GridOccupancySystem_38F63872_LambdaJob_0_Job,Unity.Entities.EntityQuery,Unity.Jobs.JobHandle)
		// Unity.Jobs.JobHandle Unity.Entities.Internal.InternalCompilerInterface.JobChunkInterface.ScheduleParallelByRef<GameFramework.ECS.Systems.DestroySystem.DestroyEntityJob>(GameFramework.ECS.Systems.DestroySystem.DestroyEntityJob&,Unity.Entities.EntityQuery,Unity.Jobs.JobHandle,Unity.Collections.NativeArray<int>)
		// System.Void Unity.Entities.JobChunkExtensions.EarlyJobInit<GameFramework.ECS.Systems.DestroySystem.DestroyEntityJob>()
		// System.Void Unity.Entities.JobChunkExtensions.EarlyJobInit<GameFramework.ECS.Systems.GridOccupancySystem.GridOccupancySystem_38F63872_LambdaJob_0_Job>()
		// System.Void Unity.Entities.JobChunkExtensions.RunByRef<GameFramework.ECS.Systems.DestroySystem.DestroyEntityJob>(GameFramework.ECS.Systems.DestroySystem.DestroyEntityJob&,Unity.Entities.EntityQuery)
		// Unity.Jobs.JobHandle Unity.Entities.JobChunkExtensions.ScheduleByRef<GameFramework.ECS.Systems.DestroySystem.DestroyEntityJob>(GameFramework.ECS.Systems.DestroySystem.DestroyEntityJob&,Unity.Entities.EntityQuery,Unity.Jobs.JobHandle)
		// Unity.Jobs.JobHandle Unity.Entities.JobChunkExtensions.ScheduleInternal<GameFramework.ECS.Systems.DestroySystem.DestroyEntityJob>(GameFramework.ECS.Systems.DestroySystem.DestroyEntityJob&,Unity.Entities.EntityQuery,Unity.Jobs.JobHandle,Unity.Jobs.LowLevel.Unsafe.ScheduleMode,Unity.Collections.NativeArray<int>)
		// Unity.Jobs.JobHandle Unity.Entities.JobChunkExtensions.ScheduleInternal<GameFramework.ECS.Systems.GridOccupancySystem.GridOccupancySystem_38F63872_LambdaJob_0_Job>(GameFramework.ECS.Systems.GridOccupancySystem.GridOccupancySystem_38F63872_LambdaJob_0_Job&,Unity.Entities.EntityQuery,Unity.Jobs.JobHandle,Unity.Jobs.LowLevel.Unsafe.ScheduleMode,Unity.Collections.NativeArray<int>)
		// Unity.Entities.ComponentLookup<GameFramework.ECS.Components.PlacementStateComponent> Unity.Entities.SystemState.GetComponentLookup<GameFramework.ECS.Components.PlacementStateComponent>(bool)
		// Unity.Entities.ComponentLookup<Unity.Transforms.LocalTransform> Unity.Entities.SystemState.GetComponentLookup<Unity.Transforms.LocalTransform>(bool)
		// Unity.Entities.ComponentTypeHandle<GameFramework.ECS.Components.DestroyTag> Unity.Entities.SystemState.GetComponentTypeHandle<GameFramework.ECS.Components.DestroyTag>(bool)
		// Unity.Entities.ComponentTypeHandle<GameFramework.ECS.Components.GridPositionComponent> Unity.Entities.SystemState.GetComponentTypeHandle<GameFramework.ECS.Components.GridPositionComponent>(bool)
		// Unity.Entities.ComponentTypeHandle<GameFramework.ECS.Components.ObjectSizeComponent> Unity.Entities.SystemState.GetComponentTypeHandle<GameFramework.ECS.Components.ObjectSizeComponent>(bool)
		// Unity.Entities.ComponentTypeHandle<GameFramework.ECS.Components.PlaceObjectRequest> Unity.Entities.SystemState.GetComponentTypeHandle<GameFramework.ECS.Components.PlaceObjectRequest>(bool)
		// System.Void Unity.Entities.SystemState.RequireForUpdate<GameFramework.ECS.Components.GlobalInputComponent>()
		// System.Void Unity.Entities.SystemState.RequireForUpdate<GameFramework.ECS.Components.GridConfigComponent>()
		// System.Void Unity.Entities.SystemState.RequireForUpdate<GameFramework.ECS.Components.PlacementStateComponent>()
		// Unity.Entities.SystemTypeIndex Unity.Entities.TypeManager.GetSystemTypeIndex<object>()
		// Unity.Entities.SystemTypeIndex Unity.Entities.TypeManager.GetSystemTypeIndexNoThrow<object>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<GameFramework.ECS.Components.BuildingTag>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<GameFramework.ECS.Components.DestroyTag>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<GameFramework.ECS.Components.GlobalInputComponent>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<GameFramework.ECS.Components.GridConfigComponent>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<GameFramework.ECS.Components.GridPositionComponent>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<GameFramework.ECS.Components.IslandTag>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<GameFramework.ECS.Components.ObjectSizeComponent>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<GameFramework.ECS.Components.PlaceObjectRequest>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<GameFramework.ECS.Components.PlacementStateComponent>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<GameFramework.ECS.Components.PreviewTag>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<Unity.Entities.EndSimulationEntityCommandBufferSystem.Singleton>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<Unity.Entities.Prefab>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<Unity.Transforms.LocalTransform>()
		// System.Void Unity.Entities.TypeManager.ManagedException<GameFramework.ECS.Components.BuildingTag>()
		// System.Void Unity.Entities.TypeManager.ManagedException<GameFramework.ECS.Components.DestroyTag>()
		// System.Void Unity.Entities.TypeManager.ManagedException<GameFramework.ECS.Components.GlobalInputComponent>()
		// System.Void Unity.Entities.TypeManager.ManagedException<GameFramework.ECS.Components.GridConfigComponent>()
		// System.Void Unity.Entities.TypeManager.ManagedException<GameFramework.ECS.Components.GridPositionComponent>()
		// System.Void Unity.Entities.TypeManager.ManagedException<GameFramework.ECS.Components.IslandTag>()
		// System.Void Unity.Entities.TypeManager.ManagedException<GameFramework.ECS.Components.ObjectSizeComponent>()
		// System.Void Unity.Entities.TypeManager.ManagedException<GameFramework.ECS.Components.PlaceObjectRequest>()
		// System.Void Unity.Entities.TypeManager.ManagedException<GameFramework.ECS.Components.PlacementStateComponent>()
		// System.Void Unity.Entities.TypeManager.ManagedException<GameFramework.ECS.Components.PreviewTag>()
		// System.Void Unity.Entities.TypeManager.ManagedException<Unity.Entities.EndSimulationEntityCommandBufferSystem.Singleton>()
		// System.Void Unity.Entities.TypeManager.ManagedException<Unity.Entities.Prefab>()
		// System.Void Unity.Entities.TypeManager.ManagedException<Unity.Transforms.LocalTransform>()
		// System.Void Unity.Entities.TypeManager.ManagedException<object>()
		// Unity.Entities.SystemHandle Unity.Entities.World.CreateSystem<object>()
		// object Unity.Entities.World.GetExistingSystemManaged<object>()
		// object Unity.Entities.World.GetOrCreateSystemManaged<object>()
		// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<object>(object)
		// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.AddressableAssets.AddressablesImpl.LoadAssetAsync<object>(object)
		// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.AddressableAssets.AddressablesImpl.LoadAssetWithChain<object>(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle,object)
		// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.AddressableAssets.AddressablesImpl.TrackHandle<object>(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object>)
		// object UnityEngine.Component.GetComponent<object>()
		// object[] UnityEngine.Component.GetComponents<object>()
		// object UnityEngine.GameObject.AddComponent<object>()
		// object UnityEngine.GameObject.GetComponent<object>()
		// object UnityEngine.GameObject.GetComponentInChildren<object>()
		// object UnityEngine.GameObject.GetComponentInChildren<object>(bool)
		// object[] UnityEngine.GameObject.GetComponents<object>()
		// object UnityEngine.Object.FindObjectOfType<object>()
		// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle.Convert<object>()
		// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.ResourceManagement.ResourceManager.CreateChainOperation<object>(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle,System.Func<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle,UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object>>)
		// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.ResourceManagement.ResourceManager.CreateCompletedOperationInternal<object>(object,bool,System.Exception,bool)
		// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.ResourceManagement.ResourceManager.CreateCompletedOperationWithException<object>(object,System.Exception)
		// object UnityEngine.ResourceManagement.ResourceManager.CreateOperation<object>(System.Type,int,UnityEngine.ResourceManagement.Util.IOperationCacheKey,System.Action<UnityEngine.ResourceManagement.AsyncOperations.IAsyncOperation>)
		// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.ResourceManagement.ResourceManager.ProvideResource<object>(UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation)
		// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.ResourceManagement.ResourceManager.StartOperation<object>(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationBase<object>,UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle)
	}
}