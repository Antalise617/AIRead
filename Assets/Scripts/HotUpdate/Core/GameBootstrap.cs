using Cysharp.Threading.Tasks;
using GameFramework.Core;
using GameFramework.ECS.Components; // 引用 GridConfigComponent
using GameFramework.ECS.Systems;    // 引用你的热更 Systems
using GameFramework.Events;
using GameFramework.Examples;
using GameFramework.Managers;
using HotUpdate.Core;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
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
            // 单例保护
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);

            // 开始初始化流程
            await InitializeGame();
        }

        private async UniTask InitializeGame()
        {
            Debug.Log("=== [GameBootstrap] 游戏启动流程开始 ===");

            // ------------------------------------------------------------
            // 1. 基础服务初始化 (配置、资源)
            // ------------------------------------------------------------
            ConfigManager.Instance.Initialize(gameConfig);
            await ConfigManager.Instance.LoadLubanTablesAsync(); // 加载配表

            await ResourceManager.Instance.InitializeAsync();    // 加载资源系统
            await UIManager.Instance.InitializeAsync();          // 加载 UI 系统

            InputManager.Instance.Initialize();
            SaveManager.Instance.Initialize();
            PoolManager.Instance.Initialize();
            SimpleQuestManager.Instance.Init();

            // 初始化背包 (传入 null 表示读档或新建)
            //GlobalInventoryManager.Instance.LoadInventory(null);

            // ------------------------------------------------------------
            // 2. ECS World 初始化
            // ------------------------------------------------------------
            Debug.Log("=== [GameBootstrap] 初始化 ECS World ===");

            // 获取或创建默认 World
            var world = World.DefaultGameObjectInjectionWorld ?? DefaultWorldInitialization.Initialize("Default World");

            // 【关键修复】手动注册热更 DLL 中的 System
            // 在打包环境下，Unity 可能无法自动发现热更程序集里的 System，导致逻辑不运行
            RegisterHotfixSystems(world);

            // ------------------------------------------------------------
            // 3. 创建 ECS 全局单例数据
            // ------------------------------------------------------------
            // 【关键修复】创建 GridConfigComponent
            // 如果缺少这个，GridSystem 和 PlacementSystem 的 RequireForUpdate 会导致它们永远不更新
            CreateGlobalComponents(world.EntityManager);

            // ------------------------------------------------------------
            // 4. 启动场景逻辑
            // ------------------------------------------------------------
            // 动态挂载 TestSceneSetup (不要在场景里预先挂载，否则会 Script Missing)
            var sceneSetupGO = new GameObject("TestSceneSetup");
            DontDestroyOnLoad(sceneSetupGO); // 或者是放在场景节点下
            var setup = sceneSetupGO.AddComponent<TestSceneSetup>();
            setup.SetupTestScene();

            // 5. 切换游戏状态，开始游戏
            GameStateManager.Instance.ChangeState(GameState.Playing);

            Debug.Log("=== [GameBootstrap] 游戏启动完成，逻辑已接管 ===");
        }

        /// <summary>
        /// 手动将热更 DLL 里的 System 添加到 World 中
        /// </summary>
        private void RegisterHotfixSystems(World world)
        {
            // 获取系统组 (System Groups)
            var simulationGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            var presentationGroup = world.GetOrCreateSystemManaged<PresentationSystemGroup>();

            // --- 注册你的 System ---

            // 1. 网格与放置逻辑 (通常在 Simulation)
            AddSystem<GridSystem>(world, simulationGroup);
            AddSystem<PlacementSystem>(world, simulationGroup);
            AddSystem<ObjectSpawningSystem>(world, simulationGroup);

            // 2. 可视化/渲染逻辑 (通常在 Presentation)
            AddSystem<GridEntityVisualizationSystem>(world, presentationGroup);

            // 3. 游客/AI 逻辑
            AddSystem<VisitorSpawningSystem>(world, simulationGroup);
            AddSystem<VisitorMovementSystem>(world, simulationGroup);
            AddSystem<VisitorPathfindingSystem>(world, simulationGroup);
            AddSystem<VisitorBehaviorSystem>(world, simulationGroup);

            // 4. 交互逻辑
            AddSystem<InteractionSystem>(world, simulationGroup);
        }

        // 【最终修正版】
        private void AddSystem<T>(World world, ComponentSystemGroup group) where T : ComponentSystemBase
        {
            System.Type systemType = typeof(T);
            try
            {
                // 1. 反射创建实例 (保持不变)
                var systemInstance = System.Activator.CreateInstance(systemType);

                // 2. 【关键修改】 注册到 World
                world.AddSystemManaged((T)systemInstance);
                // 3. 加入更新组
                group.AddSystemToUpdateList((ComponentSystemBase)systemInstance);

                Debug.Log($"[系统注册成功] {systemType.Name}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[系统注册失败] {systemType.Name} \n错误详情: {e}");
            }
        }
        /// <summary>
        /// 创建必要的全局单例组件
        /// </summary>
        private void CreateGlobalComponents(EntityManager em)
        {
            var gridQuery = em.CreateEntityQuery(typeof(GridConfigComponent));

            if (gridQuery.CalculateEntityCount() == 0)
            {
                // 创建 Entity 并添加组件
                var entity = em.CreateEntity(typeof(GridConfigComponent));
                em.SetComponentData(entity, new GridConfigComponent
                {
                    Width = 50,       // 地图宽
                    Length = 50,      // 地图长
                    Height = 10,      // 地图高
                    CellSize = 1.0f   // 格子大小
                });
                Debug.Log("[GameBootstrap] 创建 GridConfigComponent 完成");
            }

            var placementQuery = em.CreateEntityQuery(typeof(PlacementStateComponent));

            if (placementQuery.CalculateEntityCount() == 0)
            {
                // CreateSingleton 不是标准 EntityManager 方法，改用 CreateEntity
                var entity = em.CreateEntity(typeof(PlacementStateComponent));
                em.SetComponentData(entity, new PlacementStateComponent());
            }
        }
    }
}