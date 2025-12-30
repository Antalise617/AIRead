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
                // [修复] 事件名称改为 OnGameDataReceived
                NetworkManager.Instance.OnGameDataReceived += OnGameDataReceived;

                // 检查是否已经错过了事件（如果有缓存数据，直接加载）
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
                // [修复] 事件名称改为 OnGameDataReceived
                NetworkManager.Instance.OnGameDataReceived -= OnGameDataReceived;
            }
        }

        // [可选] 方法名也可以顺手改成 OnGameDataReceived，保持一致
        private void OnGameDataReceived(GamesDTO data)
        {
            Debug.Log($"[GameWorldLoader] 开始生成流程 -> Tile: {data.Tile?.Count}, Building: {data.Building?.Count}");
            LoadWorldRoutine(data).Forget();
        }

        private async UniTaskVoid LoadWorldRoutine(GamesDTO data)
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            int3? firstIslandPos = null;

            // 1. 生成岛屿
            if (data.Tile != null)
            {
                foreach (var tile in data.Tile)
                {
                    CreateIslandRequest(entityManager, tile);
                    if (firstIslandPos == null) firstIslandPos = new int3(tile.posX, tile.posY, tile.posZ);
                }
            }

            // 2. 生成建筑
            if (data.Building != null)
            {
                foreach (var build in data.Building)
                {
                    CreateBuildingRequest(entityManager, build);
                }
            }

            // 等待一帧，让 ECS 系统处理请求
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
            em.AddComponentData(requestEntity, new PlaceObjectRequest
            {
                ObjectId = tile.tile_id,
                Position = new int3(tile.posX, tile.posY, tile.posZ),
                Type = PlacementType.Island,
                Size = new int3(islandCfg.Length, islandCfg.Height, islandCfg.Width),
                Rotation = quaternion.identity,
                RotationIndex = 0,
                AirspaceHeight = islandCfg.AirHeight
            });
        }

        private void CreateBuildingRequest(EntityManager em, BuildingDTO build)
        {
            var buildCfg = ConfigManager.Instance.Tables.TbBuild.GetOrDefault(build.building_id);
            if (buildCfg == null) return;

            var requestEntity = em.CreateEntity();
            int length = buildCfg.Length;
            int width = buildCfg.Width;

            // [修复] 现在 DTO 包含 rotate 字段了，这里不会报错
            if (build.rotate % 2 != 0) { int t = length; length = width; width = t; }

            em.AddComponentData(requestEntity, new PlaceObjectRequest
            {
                ObjectId = build.building_id,
                Position = new int3(build.posX, build.posY, build.posZ),
                Type = PlacementType.Building,
                Size = new int3(length, 1, width),
                Rotation = quaternion.RotateY(math.radians(90 * build.rotate)),
                RotationIndex = build.rotate,
                AirspaceHeight = 0
            });
        }
    }
}