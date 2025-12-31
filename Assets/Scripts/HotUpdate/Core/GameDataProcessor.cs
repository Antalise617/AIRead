using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using GameFramework.ECS.Components;
using GameFramework.ECS.Systems;
using HotUpdate.Core;
using cfg;
using GameFramework.Managers;
using Cysharp.Threading.Tasks;
using Unity.Collections;
using System.Collections.Generic;
using HotUpdate.UI; // 引用 UI 命名空间
using Game.HotUpdate;

namespace GameFramework.Core
{
    /// <summary>
    /// [重构] 游戏数据处理器 (原 GameWorldLoader)
    /// 作用：监听服务器返回的 GamesDTO 全量/增量数据，并分发给对应的系统模块
    /// </summary>
    public class GameDataProcessor : MonoBehaviour
    {
        private void Awake()
        {
            Debug.Log("<color=cyan>[GameDataProcessor] 服务数据处理器启动，准备接收数据...</color>");
        }

        private void Start()
        {
            if (NetworkManager.Instance != null)
            {
                // 订阅通用数据接收事件
                NetworkManager.Instance.OnGameDataReceived += ProcessGameData;

                // 如果启动时已有缓存数据（通常是登录后的初始数据），立即处理
                if (NetworkManager.Instance.CurrentGameData != null)
                {
                    Debug.Log("[GameDataProcessor] 检测到初始缓存数据，开始加载...");
                    ProcessGameData(NetworkManager.Instance.CurrentGameData);
                }
            }
        }

        private void OnDestroy()
        {
            if (NetworkManager.Instance != null)
            {
                NetworkManager.Instance.OnGameDataReceived -= ProcessGameData;
            }
        }

        // ========================================================================
        // [核心入口] 解析并分发数据
        // ========================================================================
        private void ProcessGameData(GamesDTO data)
        {
            if (data == null) return;

            // 1. 分发玩家基础信息 (如体力、繁荣度)
            if (data.Player != null && !string.IsNullOrEmpty(data.Player._id))
            {
                ProcessPlayerInfo(data.Player);
            }

            // 2. 分发背包/道具数据
            if (data.Item != null && data.Item.Count > 0)
            {
                ProcessItems(data.Item);
            }

            // 3. 分发场景物体 (岛屿 & 建筑)
            // 只有当包含 Tile 或 Building 数据时才触发场景刷新逻辑
            if ((data.Tile != null && data.Tile.Count > 0) || (data.Building != null && data.Building.Count > 0))
            {
                ProcessWorldObjects(data).Forget();
            }

            // 4. (预留) 分发任务数据
            if (data.Quest != null && data.Quest.Count > 0)
            {
                // SimpleQuestManager.Instance.UpdateQuests(data.Quest);
            }

            // 5. (预留) 分发解锁数据
            if (data.BuildingUnlock != null)
            {
                // UnlockManager.Instance.UpdateUnlock(data.BuildingUnlock);
            }
        }

        // ========================================================================
        // [模块处理] 各子系统逻辑
        // ========================================================================

        private void ProcessPlayerInfo(PlayerDTO player)
        {
            // 更新本地玩家数据缓存
            Debug.Log($"[GameDataProcessor] 更新玩家信息: {player.name}, 繁荣度: {player.thriving}");

            // TODO: 如果有 PlayerDataManager，在这里调用 Update
            // 示例: PlayerDataManager.Instance.UpdateData(player);
        }

        private void ProcessItems(List<ItemDTO> items)
        {
            // 更新全局背包
            if (GlobalInventoryManager.Instance != null)
            {
                Debug.Log($"[GameDataProcessor] 同步背包数据: 更新 {items.Count} 个物品");
                GlobalInventoryManager.Instance.UpdateItems(items);
            }
        }

        // [核心场景逻辑] 处理岛屿和建筑的生成/更新
        private async UniTaskVoid ProcessWorldObjects(GamesDTO data)
        {
            // 判断是否是“初始加载”阶段
            // 如果当前不在 Playing 状态 (比如在 MainMenu 或 Loading)，则视为初始加载
            // 初始加载会触发：相机聚焦、状态切换、打开主UI
            bool isInitialLoad = (GameStateManager.Instance.CurrentState != GameState.Playing);

            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            int3? firstIslandPos = null;

            // --- 阶段 A: 处理岛屿 (Tile) ---
            if (data.Tile != null)
            {
                if (isInitialLoad) Debug.Log($"[GameDataProcessor] 初始加载: 准备生成 {data.Tile.Count} 个岛屿");

                foreach (var tile in data.Tile)
                {
                    // 创建生成请求 (ECS System 会自动处理去重或状态更新)
                    CreateIslandRequest(entityManager, tile);

                    if (firstIslandPos == null)
                    {
                        firstIslandPos = new int3(tile.posX, tile.posZ, tile.posY);
                    }
                }
            }

            // 等待一帧，确保岛屿请求被 ObjectSpawningSystem 捕获并注册网格
            // 这样后续的建筑生成才能检测到合法的地面
            await UniTask.NextFrame();

            // --- 阶段 B: 处理建筑 (Building) ---
            if (data.Building != null)
            {
                foreach (var build in data.Building)
                {
                    CreateBuildingRequest(entityManager, build);
                }
            }

            await UniTask.NextFrame();

            // --- 阶段 C: 初始加载的收尾工作 ---
            if (isInitialLoad)
            {
                // 1. 相机聚焦
                if (firstIslandPos.HasValue) FocusCameraOnIsland(firstIslandPos.Value);
                else FocusCameraOnIsland(int3.zero);

                // 2. 切换游戏状态 & 打开主界面
                if (GameStateManager.Instance != null)
                {
                    GameStateManager.Instance.ChangeState(GameState.Playing);

                    // 确保 MainPanel 显示
                    UIManager.Instance.ShowPanelAsync<UIPanel>("MainPanel", UILayer.Normal).Forget();
                }
            }
        }

        // ========================================================================
        // [辅助方法] 生成 ECS 请求 (保持原有逻辑)
        // ========================================================================

        private void FocusCameraOnIsland(int3 gridPos)
        {
            if (Camera.main == null) return;

            float cellSize = 2.0f; // 需与 GridConfig 一致
            float3 targetWorldPos = new float3(gridPos.x * cellSize, gridPos.y * cellSize, gridPos.z * cellSize);
            targetWorldPos += new float3(10f, 0, 10f);

            float3 cameraOffset = new float3(-30, 40, -30);
            Camera.main.transform.position = targetWorldPos + cameraOffset;
            Camera.main.transform.LookAt(targetWorldPos);
        }

        private void CreateIslandRequest(EntityManager em, TileDTO tile)
        {
            // 优先尝试用 tile_id 查表，查不到则用 tile_type (101001) 查表
            var islandCfg = ConfigManager.Instance.Tables.TbIsland.GetOrDefault(tile.tile_id);
            int visualObjectId = tile.tile_id;

            if (islandCfg == null)
            {
                islandCfg = ConfigManager.Instance.Tables.TbIsland.GetOrDefault(tile.tile_type);
                if (islandCfg != null) visualObjectId = tile.tile_type;
            }

            // 兜底逻辑
            if (islandCfg == null && ConfigManager.Instance.Tables.TbIslandLevel.GetOrDefault(tile.tile_id) != null)
            {
                islandCfg = ConfigManager.Instance.Tables.TbIsland.GetOrDefault(101001);
                if (islandCfg != null) visualObjectId = 101001;
            }

            if (islandCfg == null)
            {
                Debug.LogError($"[GameDataProcessor] 无法找到岛屿配置，跳过生成! ID:{tile.tile_id}");
                return;
            }

            var requestEntity = em.CreateEntity();
            // Server(X, Y, Z_Height) -> Unity(X, Y_Height, Z)
            int3 unityPos = new int3(tile.posX, tile.posZ, tile.posY);

            // 1. 添加生成请求
            em.AddComponentData(requestEntity, new PlaceObjectRequest
            {
                ObjectId = visualObjectId,
                Position = unityPos,
                Type = PlacementType.Island,
                Size = new int3(islandCfg.Length, islandCfg.Height, islandCfg.Width),
                Rotation = quaternion.identity,
                RotationIndex = 0,
                AirspaceHeight = islandCfg.AirHeight
            });

            // 2. 添加状态机组件 (数据绑定)
            em.AddComponentData(requestEntity, new IslandStatusComponent
            {
                State = tile.state,
                StartTime = tile.start_time / 1000,
                EndTime = tile.end_time / 1000,
                CreateTime = tile.create_time / 1000,
                ServerId = new FixedString64Bytes(tile._id),
                IsRequestSent = false
            });
        }

        private void CreateBuildingRequest(EntityManager em, BuildingDTO build)
        {
            var buildCfg = ConfigManager.Instance.Tables.TbBuild.GetOrDefault(build.building_id);
            if (buildCfg == null) return;

            var requestEntity = em.CreateEntity();
            int length = buildCfg.Length;
            int width = buildCfg.Width;

            if (build.rotate % 2 != 0) { int t = length; length = width; width = t; }

            // Server(X, Y, Z_Height) -> Unity(X, Y_Height, Z)
            int3 unityPos = new int3(build.posX, build.posZ, build.posY);

            em.AddComponentData(requestEntity, new PlaceObjectRequest
            {
                ObjectId = build.building_id,
                Position = unityPos,
                Type = PlacementType.Building,
                Size = new int3(length, 1, width),
                Rotation = quaternion.RotateY(math.radians(90 * build.rotate)),
                RotationIndex = build.rotate,
                AirspaceHeight = 0
            });
        }
    }
}