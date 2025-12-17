using cfg;
using Cysharp.Threading.Tasks;
using GameFramework.Core;
using GameFramework.ECS.Components;
using GameFramework.Managers;
using HotUpdate.Core;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine.EventSystems;
using Unity.Transforms;
using UnityEngine;
using RaycastHit = Unity.Physics.RaycastHit;

namespace GameFramework.ECS.Systems
{
    [UpdateInGroup(typeof(GameplaySystemGroup))]
    public partial class PlacementSystem : SystemBase
    {
        private GridSystem _gridSystem;
        private GridEntityVisualizationSystem _gridVisSystem;
        private Camera _mainCamera;

        private GameObject _previewObject;
        private int _lastLoadedObjectId = -1;
        private PlacementType _lastPlacementType;
        private bool _isResourceLoading = false;

        private quaternion _defaultRotation = quaternion.identity;
        private int _currentPlacementLayer = 4;

        private UnityEngine.Material _validMat;
        private UnityEngine.Material _invalidMat;

        protected override void OnCreate()
        {
            RequireForUpdate<GridConfigComponent>();
            RequireForUpdate<PlacementStateComponent>();
            RequireForUpdate<PhysicsWorldSingleton>();

            if (!SystemAPI.HasSingleton<PlacementStateComponent>())
            {
                EntityManager.CreateSingleton<PlacementStateComponent>();
            }

            _validMat = Resources.Load<UnityEngine.Material>("Green");
            _invalidMat = Resources.Load<UnityEngine.Material>("Red");
        }

        protected override void OnStartRunning()
        {
            _gridSystem = World.GetExistingSystemManaged<GridSystem>();
            _gridVisSystem = World.GetExistingSystemManaged<GridEntityVisualizationSystem>();
            _mainCamera = Camera.main;
        }

        protected override void OnUpdate()
        {
            var gridConfig = SystemAPI.GetSingleton<GridConfigComponent>();
            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            var stateRef = SystemAPI.GetSingletonRW<PlacementStateComponent>();
            ref var state = ref stateRef.ValueRW;

            HandleInput(ref state, gridConfig);

            if (!state.IsActive)
            {
                if (_previewObject != null) CleanupPreview();
                return;
            }
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                if (_previewObject != null)
                {
                    _previewObject.SetActive(false);
                }
                return; // 直接返回，不再执行后续的射线检测和放置逻辑
            }
            // 模式切换检测
            if (_lastPlacementType != state.Type)
            {
                OnPlacementModeChanged(state.Type);
                _lastPlacementType = state.Type;
            }

            if ((_previewObject == null || _lastLoadedObjectId != state.CurrentObjectId) && !_isResourceLoading)
            {
                CreatePreviewGameObject(state.CurrentObjectId, state.Type).Forget();
                return;
            }

            if (_previewObject == null) return;

            if (Input.GetKeyDown(KeyCode.R))
            {
                state.RotationIndex = (state.RotationIndex + 1) % 4;
            }

            // 传入当前模式，决定是否强制覆盖高度
            bool hasHoverGrid = PerformRaycast(physicsWorld.CollisionWorld, state.Type, out int3 hitGridPos);

            if (hasHoverGrid)
            {
                int3 baseSize = GetObjectSizeFromConfig(state.CurrentObjectId, state.Type);
                int3 finalSize = (state.RotationIndex % 2 == 1) ? new int3(baseSize.z, baseSize.y, baseSize.x) : baseSize;

                // 计算最终位置 (注意：对于建筑，hitGridPos.y 已经是正确的表面高度)
                int3 targetGridPos = CalculateFinalPlacementPosition(hitGridPos, finalSize, state.Type);

                state.CurrentGridPos = targetGridPos;
                state.IsPositionValid = ValidatePosition(state.Type, targetGridPos, finalSize);

                UpdatePreviewTransform(targetGridPos, finalSize, state.RotationIndex, gridConfig.CellSize);
                UpdatePreviewMaterial(state.IsPositionValid);

                if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) && state.IsPositionValid)
                {
                    int airSpace = 4;
                    if (state.Type == PlacementType.Island && ConfigManager.Instance.Tables != null)
                    {
                        var cfg = ConfigManager.Instance.Tables.IslandCfg.Get(state.CurrentObjectId);
                        if (cfg != null) airSpace = cfg.AirHeight;
                    }
                    SendPlacementRequest(state.CurrentObjectId, state.Type, targetGridPos, finalSize, state.RotationIndex, airSpace);
                    // 直接通知任务系统：玩家成功放置了一个物体
                    GameFramework.Events.EventManager.Instance.Publish(new ObjectBuiltEvent
                    {
                        Type = state.Type
                    });

                    if (state.Type == PlacementType.Bridge)
                    {
                        RefreshBridgeVisuals().Forget();
                    }
                }
            }
            else
            {
                _previewObject.SetActive(false);
            }
        }

        /// <summary>
        /// 延迟刷新桥梁可视化 (等待ECS系统处理完放置请求)
        /// </summary>
        private async UniTaskVoid RefreshBridgeVisuals()
        {
            // 等待一帧，确保 GridSystem 已经处理了 PlaceObjectRequest 并更新了 IsBridgeable 状态
            await UniTask.NextFrame();

            // 安全检查：如果系统已销毁或不再是桥梁模式，则不刷新
            if (_gridVisSystem == null) return;

            // 再次确认当前还是桥梁模式（防止玩家快速切换了模式导致显示错乱）
            // 注意：这里需要通过 SystemAPI 重新获取最新状态，或者简单使用 _lastPlacementType 成员变量判断
            if (_lastPlacementType == PlacementType.Bridge)
            {
                // 强制重新计算并显示所有可造桥的锚点
                _gridVisSystem.ShowBridgeableGrids();
                Debug.Log("[PlacementSystem] 桥梁放置完成，已刷新锚点显示");
            }
        }

        private void HandleInput(ref PlacementStateComponent state, GridConfigComponent gridConfig)
        {
            if (Input.GetKeyDown(KeyCode.B)) { ToggleMode(ref state, PlacementType.Island, 100005); }
            if (Input.GetKeyDown(KeyCode.K)) { ToggleMode(ref state, PlacementType.Building, 200003); }
            if (Input.GetKeyDown(KeyCode.J)) { ToggleMode(ref state, PlacementType.Building, 200001); }
            if (Input.GetKeyDown(KeyCode.L)) { ToggleMode(ref state, PlacementType.Bridge, 300001); }
            if (Input.GetKeyDown(KeyCode.Escape)) { state.IsActive = false; }

            if (state.IsActive)
            {
                // [修改]：只允许在【岛屿模式】下手动调节高度层
                // 桥梁模式现在依赖可视化的锚点，不需要手动切层
                if (state.Type == PlacementType.Island)
                {
                    if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeHeightLayer(1, gridConfig.Height, state.Type);
                    if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeHeightLayer(-1, gridConfig.Height, state.Type);
                }
            }
        }

        private void ToggleMode(ref PlacementStateComponent state, PlacementType type, int defaultId)
        {
            if (state.IsActive && state.Type == type)
            {
                state.IsActive = false;
            }
            else
            {
                state.IsActive = true;
                state.Type = type;
                state.CurrentObjectId = defaultId;
                state.RotationIndex = 0;
                _lastPlacementType = PlacementType.None;
            }
        }

        private void ChangeHeightLayer(int delta, int maxHeight, PlacementType type)
        {
            int oldLayer = _currentPlacementLayer;
            _currentPlacementLayer = math.clamp(_currentPlacementLayer + delta, 0, maxHeight - 1);

            // 只有当显示的不是“全部可建区域”时，才需要刷新 Visualization
            if (_currentPlacementLayer != oldLayer && (type == PlacementType.Island || type == PlacementType.Bridge))
            {
                Debug.Log($"[Placement] 切换高度层: {oldLayer} -> {_currentPlacementLayer}");
                _gridVisSystem?.SetVisualizationRange(_currentPlacementLayer, _currentPlacementLayer);
            }
        }

        // [关键修改] 根据模式决定显示逻辑
        private void OnPlacementModeChanged(PlacementType newType)
        {
            if (newType == PlacementType.Building)
            {
                // 建筑模式：显示所有可建造的网格
                _gridVisSystem?.ShowBuildableGrids();
            }
            else if (newType == PlacementType.Island)
            {
                // 岛屿模式：显示当前层级的网格平面
                _gridVisSystem?.SetVisualizationRange(_currentPlacementLayer, _currentPlacementLayer);
            }
            else if (newType == PlacementType.Bridge)
            {
                // [修改点] 桥梁模式：只显示岛屿边缘的连接点
                _gridVisSystem?.ShowBridgeableGrids();
            }
            else
            {
                // 退出模式：隐藏所有
                _gridVisSystem?.SetVisualizationRange(-1, -1);
            }
        }

        private void CleanupPreview()
        {
            if (_previewObject != null)
            {
                Object.Destroy(_previewObject);
                _previewObject = null;
            }
            _lastLoadedObjectId = -1;
            // 退出时隐藏所有网格
            _gridVisSystem?.SetVisualizationRange(-1, -1);
            _defaultRotation = quaternion.identity;
        }

        private int3 CalculateFinalPlacementPosition(int3 hitPos, int3 size, PlacementType type)
        {
            // 只有岛屿模式需要使用 _currentPlacementLayer 强制高度
            // 建筑和桥梁都应该依附于点击的表面 (hitPos.y)
            int targetY = (type == PlacementType.Island) ? _currentPlacementLayer : hitPos.y;

            int offsetX = (size.x % 2 == 1) ? (size.x / 2) : ((size.x - 1) / 2);
            int offsetZ = (size.z % 2 == 1) ? (size.z / 2) : ((size.z - 1) / 2);
            return new int3(hitPos.x - offsetX, targetY, hitPos.z - offsetZ);
        }

        private bool ValidatePosition(PlacementType type, int3 pos, int3 size)
        {
            int3 endPos = pos + size - new int3(1, 1, 1);
            switch (type)
            {
                case PlacementType.Island: return _gridSystem.CheckIslandPlacement(pos, size, 5);
                case PlacementType.Building: return _gridSystem.IsBuildingBuildable(pos, endPos);
                case PlacementType.Bridge: return _gridSystem.IsBridgeBuildable(pos);
            }
            return false;
        }

        // [修改]：射线穿透逻辑，只拾取网格
        private bool PerformRaycast(CollisionWorld collisionWorld, PlacementType type, out int3 gridPos)
        {
            gridPos = int3.zero;
            UnityEngine.Ray unityRay = _mainCamera.ScreenPointToRay(Input.mousePosition);

            RaycastInput rayInput = new RaycastInput
            {
                Start = unityRay.origin,
                End = unityRay.origin + unityRay.direction * 5000f,
                Filter = CollisionFilter.Default
            };

            // 1. 创建一个列表来接收射线路径上的所有物体
            NativeList<RaycastHit> allHits = new NativeList<RaycastHit>(Allocator.Temp);

            bool targetFound = false;
            float closestDistance = float.MaxValue;

            // 2. 使用 CastRay 的重载版本获取所有击中点
            if (collisionWorld.CastRay(rayInput, ref allHits))
            {
                // 3. 遍历所有击中点进行筛选
                for (int i = 0; i < allHits.Length; i++)
                {
                    var hit = allHits[i];

                    // 【核心逻辑】只处理带有 VisualGridTag 的实体 (也就是可视化网格)
                    // 这会自动忽略掉没有这个 Tag 的建筑、岛屿、玩家等其他碰撞体
                    if (EntityManager.HasComponent<VisualGridTag>(hit.Entity))
                    {
                        // 寻找最近的一个网格 (防止多层网格重叠时的误判)
                        if (hit.Fraction < closestDistance)
                        {
                            if (EntityManager.HasComponent<GridPositionComponent>(hit.Entity))
                            {
                                closestDistance = hit.Fraction;
                                gridPos = EntityManager.GetComponentData<GridPositionComponent>(hit.Entity).Value;
                                targetFound = true;
                            }
                        }
                    }
                }
            }

            // 4. 务必释放 NativeList
            allHits.Dispose();

            if (targetFound)
            {
                // [原有逻辑]：岛屿模式强制使用当前层高度
                if (type == PlacementType.Island)
                {
                    gridPos.y = _currentPlacementLayer;
                }
                return true;
            }

            return false;
        }

        // ... (SendPlacementRequest, CreatePreviewGameObject, UpdatePreviewTransform 等保持不变) ...

        private void SendPlacementRequest(int id, PlacementType type, int3 pos, int3 size, int rotation, int airSpace)
        {
            var requestEntity = EntityManager.CreateEntity();
            quaternion finalRotation = math.mul(quaternion.RotateY(math.radians(90 * rotation)), _defaultRotation);

            EntityManager.AddComponentData(requestEntity, new PlaceObjectRequest
            {
                ObjectId = id,
                Position = pos,
                Type = type,
                Size = size,
                Rotation = finalRotation,
                AirspaceHeight = airSpace,
                RotationIndex = rotation
            });
            Debug.Log($"[Placement] 发送请求: {type} at {pos}");
        }

        private async UniTaskVoid CreatePreviewGameObject(int configId, PlacementType type)
        {
            _isResourceLoading = true;
            _lastLoadedObjectId = configId;
            string resourcePath = null;

            if (ConfigManager.Instance.Tables != null)
            {
                switch (type)
                {
                    case PlacementType.Island: resourcePath = ConfigManager.Instance.Tables.IslandCfg.Get(configId)?.ResourceName; break;
                    case PlacementType.Building: resourcePath = ConfigManager.Instance.Tables.BuildingCfg.Get(configId)?.ResourceName; break;
                    case PlacementType.Bridge: resourcePath = ConfigManager.Instance.Tables.BridgeCfg.Get(configId)?.ResourceName; break;
                }
            }

            if (!string.IsNullOrEmpty(resourcePath))
            {
                var prefab = await ResourceManager.Instance.LoadAssetAsync<GameObject>(resourcePath);
                if (prefab != null)
                {
                    if (_previewObject != null) Object.Destroy(_previewObject);
                    _previewObject = Object.Instantiate(prefab);
                    _defaultRotation = _previewObject.transform.rotation;
                }
            }
            _isResourceLoading = false;
        }

        private void UpdatePreviewTransform(int3 gridPos, int3 size, int rotIndex, float cellSize)
        {
            if (_previewObject == null) return;
            _previewObject.SetActive(true);

            float3 worldPos = _gridSystem.CalculateObjectCenterWorldPosition(gridPos, size);

            _previewObject.transform.position = worldPos;
            _previewObject.transform.rotation = math.mul(quaternion.RotateY(math.radians(90 * rotIndex)), _defaultRotation);
        }

        private void UpdatePreviewMaterial(bool isValid)
        {
            if (_previewObject == null) return;
            var renderers = _previewObject.GetComponentsInChildren<Renderer>();
            UnityEngine.Material targetMat = isValid ? _validMat : _invalidMat;
            foreach (var r in renderers)
            {
                var mats = new UnityEngine.Material[r.sharedMaterials.Length];
                for (int i = 0; i < mats.Length; i++) mats[i] = targetMat;
                r.sharedMaterials = mats;
            }
        }

        private int3 GetObjectSizeFromConfig(int objectId, PlacementType type)
        {
            if (ConfigManager.Instance.Tables == null) return new int3(1, 1, 1);
            switch (type)
            {
                case PlacementType.Building:
                    var bCfg = ConfigManager.Instance.Tables.BuildingCfg.Get(objectId);
                    return bCfg != null ? new int3((int)bCfg.Length, 1, (int)bCfg.Width) : new int3(1, 1, 1);
                case PlacementType.Island:
                    var iCfg = ConfigManager.Instance.Tables.IslandCfg.Get(objectId);
                    return iCfg != null ? new int3((int)iCfg.Length, (int)iCfg.Height, (int)iCfg.Width) : new int3(1, 1, 1);
            }
            return new int3(1, 1, 1);
        }
    }
}