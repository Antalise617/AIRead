using GameFramework.ECS.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;
using RaycastHit = Unity.Physics.RaycastHit;

namespace GameFramework.ECS.Systems
{
    /// <summary>
    /// 交互系统：处理对已存在建筑/岛屿的点击
    /// 对应项目1的 Manager 中的 Click/Destroy 逻辑
    /// </summary>
    //[UpdateInGroup(typeof(GameplaySystemGroup))]
    public partial class InteractionSystem : SystemBase
    {
        private GridSystem _gridSystem;
        private Camera _mainCamera;

        protected override void OnCreate()
        {
            RequireForUpdate<PhysicsWorldSingleton>();
            RequireForUpdate<GridConfigComponent>();
        }

        protected override void OnStartRunning()
        {
            _gridSystem = World.GetExistingSystemManaged<GridSystem>();
            _mainCamera = Camera.main;
        }

        protected override void OnUpdate()
        {
            // 如果处于放置模式，则不处理交互（根据需求调整）
            if (SystemAPI.HasSingleton<PlacementStateComponent>())
            {
                var state = SystemAPI.GetSingleton<PlacementStateComponent>();
                if (state.IsActive) return;
            }

            // 鼠标左键点击选择/查看
            if (Input.GetMouseButtonDown(0))
            {
                HandleClick();
            }

            // 示例：按 Delete 键销毁当前鼠标悬停的建筑 (对应项目1的 DestroyBuilding)
            // 实际项目中可能需要先选中再点击 UI 按钮来触发
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                HandleDestroy();
            }
        }

        private void HandleClick()
        {
            Entity hitEntity = PerformRaycast();
            if (hitEntity == Entity.Null) return;

            if (EntityManager.HasComponent<BuildingComponent>(hitEntity))
            {
                var building = EntityManager.GetComponentData<BuildingComponent>(hitEntity);
                Debug.Log($"[Interaction] 选中建筑: ID {building.ConfigId}");
                // TODO: 触发 UI 事件，显示建筑详情
                // EventManager.Instance.TriggerEvent("BuildingClicked", building.ConfigId);
            }
            else if (EntityManager.HasComponent<IslandComponent>(hitEntity))
            {
                var island = EntityManager.GetComponentData<IslandComponent>(hitEntity);
                Debug.Log($"[Interaction] 选中岛屿: ID {island.ConfigId}");
            }
        }

        private void HandleDestroy()
        {
            Entity hitEntity = PerformRaycast();
            if (hitEntity == Entity.Null) return;

            if (EntityManager.HasComponent<BuildingComponent>(hitEntity))
            {
                var building = EntityManager.GetComponentData<BuildingComponent>(hitEntity);
                var pos = EntityManager.GetComponentData<GridPositionComponent>(hitEntity).Value;

                // 1. 数据层注销
                FixedString64Bytes bid = new FixedString64Bytes(building.ConfigId.ToString());
                _gridSystem.UnregisterBuilding(pos, building.Size, bid);

                // 2. 销毁实体
                EntityManager.DestroyEntity(hitEntity);
                Debug.Log($"[Interaction] 拆除建筑: {pos}");
            }
            // 同样可以添加岛屿的拆除逻辑...
        }

        private Entity PerformRaycast()
        {
            var collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
            UnityEngine.Ray unityRay = _mainCamera.ScreenPointToRay(Input.mousePosition);
            var rayInput = new RaycastInput
            {
                Start = unityRay.origin,
                End = unityRay.origin + unityRay.direction * 1000f,
                Filter = CollisionFilter.Default
            };

            if (collisionWorld.CastRay(rayInput, out RaycastHit hit))
            {
                return hit.Entity;
            }
            return Entity.Null;
        }
    }
}