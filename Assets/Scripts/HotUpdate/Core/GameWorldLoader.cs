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
            await UniTask.NextFrame();
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            int3? firstIslandPos = null;

            if (data.Tile != null)
            {
                Debug.Log($"[GameWorldLoader] 阶段1: 发送 {data.Tile.Count} 个岛屿生成请求...");
                foreach (var tile in data.Tile)
                {
                    // [修改] 创建请求时传入 tile (内部会处理 tile_id vs tile_type)
                    CreateIslandRequest(entityManager, tile);

                    if (firstIslandPos == null)
                    {
                        firstIslandPos = new int3(tile.posX, tile.posZ, tile.posY);
                    }
                }
            }

            await UniTask.NextFrame();

            if (data.Building != null)
            {
                Debug.Log($"[GameWorldLoader] 阶段2: 岛屿数据已就绪，开始发送 {data.Building.Count} 个建筑生成请求...");
                foreach (var build in data.Building)
                {
                    CreateBuildingRequest(entityManager, build);
                }
            }

            await UniTask.NextFrame();

            if (firstIslandPos.HasValue) FocusCameraOnIsland(firstIslandPos.Value);
            else FocusCameraOnIsland(int3.zero);

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
            // [核心修复] 
            // 优先尝试用 tile_id 查表 (以防未来配置表扩充了具体ID)
            // 如果查不到，则使用 tile_type (101001) 查表 (这是目前的默认逻辑)
            var islandCfg = ConfigManager.Instance.Tables.TbIsland.GetOrDefault(tile.tile_id);

            int visualObjectId = tile.tile_id; // 默认用 id

            if (islandCfg == null)
            {
                // 尝试用 type 查找 (例如 102001 -> 101001)
                islandCfg = ConfigManager.Instance.Tables.TbIsland.GetOrDefault(tile.tile_type);
                if (islandCfg != null)
                {
                    visualObjectId = tile.tile_type; // 找到了，说明要用 type 作为视觉ID
                }
            }

            // 如果还是找不到，并且是 102001 这种存在于 Level 表的 ID，强制兜底到 101001
            if (islandCfg == null && ConfigManager.Instance.Tables.TbIslandLevel.GetOrDefault(tile.tile_id) != null)
            {
                islandCfg = ConfigManager.Instance.Tables.TbIsland.GetOrDefault(101001);
                if (islandCfg != null) visualObjectId = 101001;
            }

            if (islandCfg == null)
            {
                Debug.LogError($"[GameWorldLoader] 无法找到岛屿配置，跳过生成! ID:{tile.tile_id}, Type:{tile.tile_type}");
                return;
            }

            var requestEntity = em.CreateEntity();

            // [坐标系修正] Server(X, Y, Z_Height) -> Unity(X, Y_Height, Z)
            int3 unityPos = new int3(tile.posX, tile.posZ, tile.posY);

            // 1. 添加生成请求组件
            // 注意：这里 ObjectId 传入 visualObjectId (101001)，确保 ObjectSpawningSystem 能加载到资源
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

            // 2. 添加状态机组件
            em.AddComponentData(requestEntity, new IslandStatusComponent
            {
                State = tile.state,
                StartTime = tile.start_time,
                EndTime = tile.end_time,
                CreateTime = tile.create_time
                // 如果需要，可以在这里把真实的 tile.tile_id 存入组件，供后续逻辑使用
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