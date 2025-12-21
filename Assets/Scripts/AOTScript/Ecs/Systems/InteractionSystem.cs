using GameFramework.ECS.Components;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;
using Cysharp.Threading.Tasks;
using GameFramework.Managers;
using GameFramework.Core;
using RaycastHit = Unity.Physics.RaycastHit;

namespace GameFramework.ECS.Systems
{
    [UpdateInGroup(typeof(GameplaySystemGroup))]
    public partial class InteractionSystem : SystemBase
    {
        private Camera _mainCamera;

        protected override void OnCreate()
        {
            RequireForUpdate<PhysicsWorldSingleton>();
            RequireForUpdate<GridConfigComponent>();
        }

        protected override void OnStartRunning()
        {
            _mainCamera = Camera.main;
        }

        protected override void OnUpdate()
        {
            // 如果处于放置模式，则不处理交互
            if (SystemAPI.HasSingleton<PlacementStateComponent>())
            {
                if (SystemAPI.GetSingleton<PlacementStateComponent>().IsActive) return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                HandleSelection();
            }
        }

        private void HandleSelection()
        {
            // 1. 无论是否击中，先关闭所有可能存在的交互控件 (确保唯一性)
            CloseAllWidgets();

            Entity hitEntity = PerformRaycast();

            // 2. 如果未击中任何物体，流程结束（UI 已在第 1 步关闭）
            if (hitEntity == Entity.Null) return;

            // 3. 判定击中物体的类型
            if (EntityManager.HasComponent<BuildingComponent>(hitEntity))
            {
                ShowWidgetAsync<UIFollowPanel>(hitEntity, "ClickBuildingWidget").Forget();
            }
            else if (EntityManager.HasComponent<IslandComponent>(hitEntity))
            {
                ShowWidgetAsync<UIFollowPanel>(hitEntity, "ClickIslandWidget").Forget();
            }
            // 4. 如果点击了其他带有 Collider 的物体（如地面/装饰），UI 保持关闭状态
        }

        private void CloseAllWidgets()
        {
            // 直接调用 UIManager 隐藏相关的交互面板 Key
            UIManager.Instance.HidePanel("ClickIslandWidget");
            UIManager.Instance.HidePanel("ClickBuildingWidget");
        }

        private async UniTaskVoid ShowWidgetAsync<T>(Entity entity, string panelKey) where T : UIFollowPanel
        {
            var widget = await UIManager.Instance.ShowPanelAsync<T>(panelKey);
            if (widget != null)
            {
                widget.Bind(entity);
            }
        }

        private Entity PerformRaycast()
        {
            if (!SystemAPI.HasSingleton<PhysicsWorldSingleton>()) return Entity.Null;

            var collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
            UnityEngine.Ray unityRay = _mainCamera.ScreenPointToRay(Input.mousePosition);

            RaycastInput rayInput = new RaycastInput
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