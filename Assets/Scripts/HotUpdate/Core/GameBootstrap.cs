using Cysharp.Threading.Tasks;
using Game.HotUpdate;
using GameFramework.ECS.Components;
using GameFramework.ECS.Systems;
using GameFramework.Examples;
using GameFramework.Managers;
using HotUpdate.Core;
using Unity.Entities;
using UnityEngine;

namespace GameFramework.Core
{
    public class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private GameConfig gameConfig;

        private static GameBootstrap _instance;
        public static GameBootstrap Instance => _instance;

        private async void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);

            await InitializeGame();
        }

        private async UniTask InitializeGame()
        {
            Debug.Log("=== [GameBootstrap] 游戏启动流程开始 ===");

            // 1. 基础服务初始化
            ConfigManager.Instance.Initialize(gameConfig);
            await ConfigManager.Instance.LoadLubanTablesAsync();
            await ResourceManager.Instance.InitializeAsync();
            await UIManager.Instance.InitializeAsync();

            InputManager.Instance.Initialize();
            SaveManager.Instance.Initialize();
            PoolManager.Instance.Initialize();
            SimpleQuestManager.Instance.Init();
            NetworkManager.Instance.Initialize(); // 网络初始化
            GlobalInventoryManager.Instance.LoadInventory(null);

            // 2. ECS World 初始化
            Debug.Log("=== [GameBootstrap] 初始化 ECS World ===");
            var world = World.DefaultGameObjectInjectionWorld ?? DefaultWorldInitialization.Initialize("Default World");
            RegisterHotfixSystems(world);
            CreateGlobalComponents(world.EntityManager);

            // 3. 【关键修复】手动启动 GameDataProcessor (原 GameWorldLoader)
            // 既然是单场景，我们需要确保这个负责解析全量数据的脚本此时已经挂载并运行
            if (FindObjectOfType<GameDataProcessor>() == null)
            {
                GameObject processorGO = new GameObject("GameDataProcessor");
                processorGO.AddComponent<GameDataProcessor>();
                DontDestroyOnLoad(processorGO); // 保证它一直存在
                Debug.Log("=== [GameBootstrap] GameDataProcessor 已动态挂载并启动 ===");
            }

            // 4. 启动场景测试逻辑 (如果需要)
            var sceneSetupGO = new GameObject("TestSceneSetup");
            DontDestroyOnLoad(sceneSetupGO);
            var setup = sceneSetupGO.AddComponent<TestSceneSetup>();
            setup.SetupTestScene();

            // 5. 初始状态为 MainMenu (等待玩家选服)，而不是直接 Playing
            // 只有当选服成功后，我们才切到 Playing
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.ChangeState(GameState.MainMenu);
            }

            Debug.Log("=== [GameBootstrap] 游戏启动完成 ===");
        }

        private void RegisterHotfixSystems(World world)
        {
            var simulationGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            var presentationGroup = world.GetOrCreateSystemManaged<PresentationSystemGroup>();

            AddSystem<GridSystem>(world, simulationGroup);
            AddSystem<PlacementSystem>(world, simulationGroup);
            AddSystem<ObjectSpawningSystem>(world, simulationGroup);
            AddSystem<GridEntityVisualizationSystem>(world, presentationGroup);
            AddSystem<VisitorSpawningSystem>(world, simulationGroup);
            AddSystem<VisitorMovementSystem>(world, simulationGroup);
            AddSystem<VisitorPathfindingSystem>(world, simulationGroup);
            AddSystem<VisitorBehaviorSystem>(world, simulationGroup);
            AddSystem<InteractionSystem>(world, simulationGroup);
        }

        private void AddSystem<T>(World world, ComponentSystemGroup group) where T : ComponentSystemBase
        {
            System.Type systemType = typeof(T);
            try
            {
                var systemInstance = System.Activator.CreateInstance(systemType);
                world.AddSystemManaged((T)systemInstance);
                group.AddSystemToUpdateList((ComponentSystemBase)systemInstance);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[系统注册失败] {systemType.Name}: {e}");
            }
        }

        private void CreateGlobalComponents(EntityManager em)
        {
            var gridQuery = em.CreateEntityQuery(typeof(GridConfigComponent));
            if (gridQuery.CalculateEntityCount() == 0)
            {
                var entity = em.CreateEntity(typeof(GridConfigComponent));
                em.SetComponentData(entity, new GridConfigComponent { Width = 50, Length = 50, Height = 10, CellSize = 1.0f });
            }

            var placementQuery = em.CreateEntityQuery(typeof(PlacementStateComponent));
            if (placementQuery.CalculateEntityCount() == 0)
            {
                var entity = em.CreateEntity(typeof(PlacementStateComponent));
                em.SetComponentData(entity, new PlacementStateComponent());
            }
        }
    }
}