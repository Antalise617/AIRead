using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using GameFramework.ECS.Components; // 引用 PlaceObjectRequest
using GameFramework.ECS.Systems; // 引用 PlacementType
using HotUpdate.Core; // 引用 ConfigManager
using cfg; // 引用 Luban 配置表
using GameFramework.Managers;

namespace GameFramework.Core
{
    public class GameWorldLoader : MonoBehaviour
    {
        private void Start()
        {
            if (NetworkManager.Instance != null)
            {
                NetworkManager.Instance.OnJoinGameSuccess += OnJoinGameSuccess;
            }
        }

        private void OnDestroy()
        {
            if (NetworkManager.Instance != null)
            {
                NetworkManager.Instance.OnJoinGameSuccess -= OnJoinGameSuccess;
            }
        }

        /// <summary>
        /// 收到加入游戏成功事件，开始加载世界
        /// </summary>
        private void OnJoinGameSuccess(GamesDTO data)
        {
            Debug.Log($"[GameWorldLoader] 开始初始化世界... 地块数: {data.Tile?.Count ?? 0}, 建筑数: {data.Building?.Count ?? 0}");

            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            // 1. 初始化岛屿 (Tile)
            if (data.Tile != null)
            {
                foreach (var tile in data.Tile)
                {
                    CreateIslandRequest(entityManager, tile);
                }
            }

            // 2. 初始化建筑 (Building) - 可选，为了完整性
            if (data.Building != null)
            {
                foreach (var build in data.Building)
                {
                    CreateBuildingRequest(entityManager, build);
                }
            }
        }

        private void CreateIslandRequest(EntityManager em, TileDTO tile)
        {
            // 查表获取岛屿尺寸
            // 假设 tile.tile_id 对应 TbIsland 表的 ID
            var islandCfg = ConfigManager.Instance.Tables.TbIsland.GetOrDefault(tile.tile_id);
            if (islandCfg == null)
            {
                Debug.LogError($"[GameWorldLoader] 找不到岛屿配置 ID: {tile.tile_id}");
                return;
            }

            // 创建 ECS 请求实体
            var requestEntity = em.CreateEntity();

            // 构造尺寸 (Length=X, Height=Y, Width=Z)
            int3 size = new int3(islandCfg.Length, islandCfg.Height, islandCfg.Width);

            em.AddComponentData(requestEntity, new PlaceObjectRequest
            {
                ObjectId = tile.tile_id,
                Position = new int3(tile.posX, tile.posY, tile.posZ),
                Type = PlacementType.Island, // 
                Size = size,
                Rotation = quaternion.identity, // 岛屿通常不旋转
                RotationIndex = 0,
                AirspaceHeight = islandCfg.AirHeight
            });
        }

        private void CreateBuildingRequest(EntityManager em, BuildingDTO build)
        {
            var buildCfg = ConfigManager.Instance.Tables.TbBuild.GetOrDefault(build.building_id);
            if (buildCfg == null) return;

            var requestEntity = em.CreateEntity();

            // 建筑通常只有长宽，高度默认为1 (根据之前 PlacementSystem 的逻辑)
            // 如果有旋转，且旋转为 1 或 3 (90/270度)，长宽需要对调
            int length = buildCfg.Length;
            int width = buildCfg.Width;

            if (build.rotate % 2 != 0)
            {
                // 交换长宽
                int temp = length; length = width; width = temp;
            }

            // 计算旋转四元数 (0=0度, 1=90度...)
            quaternion rot = quaternion.RotateY(math.radians(90 * build.rotate));

            em.AddComponentData(requestEntity, new PlaceObjectRequest
            {
                ObjectId = build.building_id,
                Position = new int3(build.posX, build.posY, build.posZ),
                Type = PlacementType.Building,
                Size = new int3(length, 1, width),
                Rotation = rot,
                RotationIndex = build.rotate,
                AirspaceHeight = 0
            });
        }
    }
}