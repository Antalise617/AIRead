using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using GameFramework.ECS.Components;
using GameFramework.ECS.Systems;
using HotUpdate.Core;
using cfg;
using GameFramework.Managers;
using Cysharp.Threading.Tasks;

namespace GameFramework.Core
{
    public class GameWorldLoader : MonoBehaviour
    {
        private void Awake()
        {
            Debug.Log("<color=cyan>[GameWorldLoader] Awake: 脚本已启动，准备接收数据。</color>");
        }

        private void Start()
        {
            if (NetworkManager.Instance != null)
            {
                NetworkManager.Instance.OnGameDataReceived += OnGameDataReceived;

                if (NetworkManager.Instance.CurrentGameData != null)
                {
                    Debug.Log("[GameWorldLoader] Start: 发现缓存数据，立即生成！");
                    OnGameDataReceived(NetworkManager.Instance.CurrentGameData);
                }
            }
        }

        private void OnDestroy()
        {
            if (NetworkManager.Instance != null)
            {
                NetworkManager.Instance.OnGameDataReceived -= OnGameDataReceived;
            }
        }

        private void OnGameDataReceived(GamesDTO data)
        {
            Debug.Log($"[GameWorldLoader] 开始生成流程 -> Tile: {data.Tile?.Count}, Building: {data.Building?.Count}");
            LoadWorldRoutine(data).Forget();
        }

        private async UniTaskVoid LoadWorldRoutine(GamesDTO data)
        {
            // [新增] 初始等待：确保 ECS 世界和 GridSystem 完成初始化
            await UniTask.NextFrame();

            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            int3? firstIslandPos = null;

            // ========================================================================
            // 阶段 1: 优先生成岛屿 (填充 Grid 数据)
            // ========================================================================
            if (data.Tile != null)
            {
                Debug.Log($"[GameWorldLoader] 阶段1: 发送 {data.Tile.Count} 个岛屿生成请求...");
                foreach (var tile in data.Tile)
                {
                    CreateIslandRequest(entityManager, tile);
                    // [坐标系修正] 记录相机焦点时，也要进行 Y/Z 互换
                    // Server(X, Y, Z_Height) -> Unity(X, Y_Height, Z)
                    // 对应 DTO(posX, posY, posZ) -> Unity(posX, posZ, posY)
                    if (firstIslandPos == null)
                    {
                        firstIslandPos = new int3(tile.posX, tile.posZ, tile.posY);
                    }
                }
            }

            // [核心修复] 
            // 必须等待一帧！
            // 此时 ECS 系统会处理上述 Island 请求，GridSystem 会注册岛屿信息。
            // 只有这一步完成后，WorldGrid 中对应坐标才会变成 "IsBuildable = true"。
            await UniTask.NextFrame();

            // ========================================================================
            // 阶段 2: 生成建筑 (读取 Grid 数据)
            // ========================================================================
            if (data.Building != null)
            {
                Debug.Log($"[GameWorldLoader] 阶段2: 岛屿数据已就绪，开始发送 {data.Building.Count} 个建筑生成请求...");
                foreach (var build in data.Building)
                {
                    CreateBuildingRequest(entityManager, build);
                }
            }

            // 再等待一帧，让建筑请求被 ECS 处理完 (可选，主要是为了确保后续相机聚焦等逻辑在物体生成后执行)
            await UniTask.NextFrame();

            // 3. 聚焦相机
            if (firstIslandPos.HasValue)
            {
                FocusCameraOnIsland(firstIslandPos.Value);
            }
            else
            {
                Debug.LogWarning("[GameWorldLoader] 没有岛屿数据，相机将聚焦到原点");
                FocusCameraOnIsland(int3.zero);
            }

            // 4. 切换到 Playing 状态，正式开始游戏
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.ChangeState(GameState.Playing);
                UIManager.Instance.ShowPanelAsync<UIPanel>("MainPanel", UILayer.Normal).Forget();
            }
        }

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
            var islandCfg = ConfigManager.Instance.Tables.TbIsland.GetOrDefault(tile.tile_id);
            if (islandCfg == null) return;

            var requestEntity = em.CreateEntity();

            // [坐标系修正] Server(X, Y, Z_Height) -> Unity(X, Y_Height, Z)
            int3 unityPos = new int3(tile.posX, tile.posZ, tile.posY);

            // 1. 添加生成请求组件
            em.AddComponentData(requestEntity, new PlaceObjectRequest
            {
                ObjectId = tile.tile_id,
                Position = unityPos,
                Type = PlacementType.Island,
                Size = new int3(islandCfg.Length, islandCfg.Height, islandCfg.Width),
                Rotation = quaternion.identity,
                RotationIndex = 0,
                AirspaceHeight = islandCfg.AirHeight
            });

            // 2. 添加状态机组件，携带后端时间数据
            em.AddComponentData(requestEntity, new IslandStatusComponent
            {
                State = tile.state,
                StartTime = tile.start_time,
                EndTime = tile.end_time,
                CreateTime = tile.create_time
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

            // [坐标系修正] Server(X, Y, Z_Height) -> Unity(X, Y_Height, Z)
            // 修正前: new int3(build.posX, build.posY, build.posZ)
            // 修正后: new int3(build.posX, build.posZ, build.posY)
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