using Cysharp.Threading.Tasks;
using GameFramework.Core;
using GameFramework.ECS.Systems;
using GameFramework.Events;
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
            if (_instance != null && _instance != this) { Destroy(gameObject); return; }
            _instance = this;
            DontDestroyOnLoad(gameObject);
            await InitializeGame();
        }

        private async UniTask InitializeGame()
        {
            Debug.Log("=== 游戏启动 ===");
            ConfigManager.Instance.Initialize(gameConfig);
            await ConfigManager.Instance.LoadLubanTablesAsync();

            GameConfigBridge.Service = new HotfixConfigService(); // 注入实现

            await ResourceManager.Instance.InitializeAsync();
            await UIManager.Instance.InitializeAsync();
            InputManager.Instance.Initialize();
            SaveManager.Instance.Initialize();
            PoolManager.Instance.Initialize();
            SimpleQuestManager.Instance.Init();

            // Entities 1.0+: 自动初始化默认世界
            var world = World.DefaultGameObjectInjectionWorld ?? DefaultWorldInitialization.Initialize("Default World");
            // 移除旧 API UpdatePlayerLoop

            var setup = FindObjectOfType<TestSceneSetup>() ?? new GameObject("TestSceneSetup").AddComponent<TestSceneSetup>();
            setup.SetupTestScene();

            GameStateManager.Instance.ChangeState(GameState.Playing);
            Debug.Log("=== 游戏启动完成 ===");
        }
    }
}