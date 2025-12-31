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
using HotUpdate.UI;
using Game.HotUpdate;
using GameFramework.Events;
using GameFramework.Gameplay;

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
            if (EventManager.Instance != null)
            {
                // 订阅通用数据接收事件
                EventManager.Instance.Subscribe<GameDataReceivedEvent>(ProcessGameData);
            }

            if (NetworkManager.Instance != null)
            {
                // 如果启动时已有缓存数据（通常是登录后的初始数据），立即处理
                if (NetworkManager.Instance.CurrentGameData != null)
                {
                    Debug.Log("[GameDataProcessor] 检测到初始缓存数据，开始加载...");
                    ProcessGameData(new GameDataReceivedEvent(NetworkManager.Instance.CurrentGameData));
                }
            }
        }

        private void OnDestroy()
        {
            if (EventManager.Instance != null)
            {
                EventManager.Instance.Unsubscribe<GameDataReceivedEvent>(ProcessGameData);
            }
        }

        // ========================================================================
        // [核心入口] 解析并分发数据
        // ========================================================================
        private void ProcessGameData(GameDataReceivedEvent evt)
        {
            var data = evt.Data;
            if (data == null) return;

            // 1. 分发玩家基础信息
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
            if ((data.Tile != null && data.Tile.Count > 0) || (data.Building != null && data.Building.Count > 0))
            {
                ProcessWorldObjects(data).Forget();
            }

            // 4. (预留) 分发任务数据
            if (data.Quest != null && data.Quest.Count > 0)
            {
                // SimpleQuestManager.Instance.UpdateQuests(data.Quest);
            }
        }

        // ========================================================================
        // [模块处理] 各子系统逻辑
        // ========================================================================

        private void ProcessPlayerInfo(PlayerDTO player)
        {
            Debug.Log($"[GameDataProcessor] 更新玩家信息: {player.name}, 繁荣度: {player.thriving}");
        }

        private void ProcessItems(List<ItemDTO> items)
        {
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
            bool isInitialLoad = (GameStateManager.Instance.CurrentState != GameState.Playing);

            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            int3? firstIslandPos = null;
            int3? targetFocusPos = null; // [新增] 用于存储需要聚焦的特定建筑位置

            // --- 阶段 A: 处理岛屿 (Tile) ---
            if (data.Tile != null)
            {
                if (isInitialLoad) Debug.Log($"[GameDataProcessor] 初始加载: 准备生成 {data.Tile.Count} 个岛屿");

                foreach (var tile in data.Tile)
                {
                    CreateIslandRequest(entityManager, tile);

                    if (firstIslandPos == null)
                    {
                        firstIslandPos = new int3(tile.posX, tile.posZ, tile.posY);
                    }
                }
            }

            await UniTask.NextFrame();

            // --- 阶段 B: 处理建筑 (Building) ---
            if (data.Building != null)
            {
                foreach (var build in data.Building)
                {
                    CreateBuildingRequest(entityManager, build);

                    // [新增逻辑] 寻找大类为1 (BuildType=1) 的建筑 (游客中心)
                    // 我们记录它的位置，以便稍后聚焦相机
                    if (build.BuildType == 1)
                    {
                        // Server(X, Y, Z_Height) -> Unity Grid(X, Y_Height, Z)
                        targetFocusPos = new int3(build.posX, build.posZ, build.posY);
                        Debug.Log($"[GameDataProcessor] 找到游客中心 (BuildType:1), 目标位置: {targetFocusPos}");
                    }
                }
            }

            await UniTask.NextFrame();

            // --- 阶段 C: 初始加载的收尾工作 ---
            if (isInitialLoad)
            {
                // 1. 相机聚焦逻辑优化
                if (targetFocusPos.HasValue)
                {
                    // 优先聚焦到游客中心
                    InitializeCamera(targetFocusPos.Value);
                }
                else if (firstIslandPos.HasValue)
                {
                    // 没找到游客中心，退化为聚焦第一个岛屿
                    InitializeCamera(firstIslandPos.Value);
                }
                else
                {
                    // 什么都没有，聚焦零点
                    InitializeCamera(int3.zero);
                }

                // 2. 切换游戏状态 & 打开主界面
                if (GameStateManager.Instance != null)
                {
                    GameStateManager.Instance.ChangeState(GameState.Playing);
                    UIManager.Instance.ShowPanelAsync<UIPanel>("MainPanel", UILayer.Normal).Forget();
                }
            }
        }

        // ========================================================================
        // [辅助方法] 生成 ECS 请求
        // ========================================================================

        // 复用原有的聚焦方法 (传入 Grid 坐标，内部转换为世界坐标)
        private void InitializeCamera(int3 gridPos)
        {
            var camController = FindObjectOfType<StrategyCameraController>();
            if (camController == null) return;

            float cellSize = 2.0f; // 需与 Config 一致
            Vector3 targetWorldPos = new Vector3(gridPos.x * cellSize, 0, gridPos.z * cellSize);

            // 调用专门的初始化方法
            camController.SetInitialView(targetWorldPos);
        }

        private void CreateIslandRequest(EntityManager em, TileDTO tile)
        {
            var islandCfg = ConfigManager.Instance.Tables.TbIsland.GetOrDefault(tile.tile_id);
            int visualObjectId = tile.tile_id;

            if (islandCfg == null)
            {
                islandCfg = ConfigManager.Instance.Tables.TbIsland.GetOrDefault(tile.tile_type);
                if (islandCfg != null) visualObjectId = tile.tile_type;
            }

            if (islandCfg == null && ConfigManager.Instance.Tables.TbIslandLevel.GetOrDefault(tile.tile_id) != null)
            {
                islandCfg = ConfigManager.Instance.Tables.TbIsland.GetOrDefault(101001);
                if (islandCfg != null) visualObjectId = 101001;
            }

            if (islandCfg == null) return;

            var requestEntity = em.CreateEntity();
            int3 unityPos = new int3(tile.posX, tile.posZ, tile.posY);

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