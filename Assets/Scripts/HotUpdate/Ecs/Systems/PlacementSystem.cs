using Cysharp.Threading.Tasks;
using GameFramework.Core;
using GameFramework.ECS.Components;
using GameFramework.Managers;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.EventSystems;
using Lean.Touch;
using GameFramework.Events;
using RaycastHit = Unity.Physics.RaycastHit;
using cfg;

namespace GameFramework.ECS.Systems
{
    public partial class PlacementSystem : SystemBase
    {
        private GridSystem _gridSystem;
        private GridEntityVisualizationSystem _gridVisSystem;
        private Camera _mainCamera;

        private GameObject _previewObject;
        private int _lastLoadedObjectId = -1;
        private PlacementType _lastPlacementType;
        private bool _isResourceLoading = false;
        private bool _isFirstFrameAfterLoad = false;

        private quaternion _defaultRotation = quaternion.identity;
        private int _currentPlacementLayer = 4;

        private UnityEngine.Material _validMat;
        private UnityEngine.Material _invalidMat;

        private IPlacementConfirmPanel _confirmPanel;

        private float _twistAccumulator = 0f;

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

            LoadConfirmPanel().Forget();
        }

        private async UniTaskVoid LoadConfirmPanel()
        {
            await UniTask.WaitUntil(() => UIManager.Instance.IsInitialized);

            var panel = await UIManager.Instance.ShowPanelAsync<UIPanel>("PlacementConfirmPanel", UILayer.Normal);

            if (panel is IPlacementConfirmPanel confirmPanel)
            {
                _confirmPanel = confirmPanel;
                _confirmPanel.Hide();
            }
            else
            {
                Debug.LogError("[PlacementSystem] 加载的面板没有实现 IPlacementConfirmPanel 接口！");
            }
        }

        protected override void OnUpdate()
        {
            var gridConfig = SystemAPI.GetSingleton<GridConfigComponent>();
            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            var stateRef = SystemAPI.GetSingletonRW<PlacementStateComponent>();
            ref var state = ref stateRef.ValueRW;

            HandleKeyboardInput(ref state, gridConfig);

            if (!state.IsActive)
            {
                if (_previewObject != null) CleanupPreview();
                if (_confirmPanel != null && _confirmPanel.IsVisible) _confirmPanel.Hide();
                return;
            }

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

            if (_isFirstFrameAfterLoad)
            {
                _isFirstFrameAfterLoad = false;
                InitializePreview(physicsWorld, ref state, gridConfig);
                if (_confirmPanel != null) _confirmPanel.Show();
            }

            // === 交互逻辑 ===
            var fingers = LeanTouch.GetFingers(true, false);
            if (fingers.Count >= 2)
            {
                HandleTouchRotation(fingers, ref state);
                return;
            }

            if (Input.GetMouseButton(0))
            {
                if (IsPointerOverUI()) return;
                if (_confirmPanel != null) _confirmPanel.Hide();

                UpdateObjectPosition(physicsWorld, Input.mousePosition, ref state, gridConfig);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (_previewObject.activeSelf && _confirmPanel != null)
                {
                    _confirmPanel.Show();
                }
            }

            if (_confirmPanel != null && _confirmPanel.IsVisible && _previewObject != null)
            {
                _confirmPanel.UpdatePosition(_previewObject.transform.position);
            }
        }

        // --- 辅助方法 ---

        public void RotatePreview()
        {
            if (!SystemAPI.HasSingleton<PlacementStateComponent>()) return;
            var stateRef = SystemAPI.GetSingletonRW<PlacementStateComponent>();
            ref var state = ref stateRef.ValueRW;

            if (!state.IsActive || _previewObject == null) return;

            state.RotationIndex = (state.RotationIndex + 1) % 4;
            var gridConfig = SystemAPI.GetSingleton<GridConfigComponent>();
            int3 baseSize = GetObjectSizeFromConfig(state.CurrentObjectId, state.Type);
            int3 finalSize = (state.RotationIndex % 2 == 1) ? new int3(baseSize.z, baseSize.y, baseSize.x) : baseSize;
            state.IsPositionValid = ValidatePosition(state.Type, state.CurrentGridPos, finalSize);

            UpdatePreviewTransform(state.CurrentGridPos, finalSize, state.RotationIndex, gridConfig.CellSize);
            UpdatePreviewMaterial(state.IsPositionValid);
        }

        private bool IsPointerOverUI()
        {
            if (EventSystem.current == null) return false;
            if (Input.touchCount > 0) return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
            return EventSystem.current.IsPointerOverGameObject();
        }

        private void InitializePreview(PhysicsWorldSingleton physicsWorld, ref PlacementStateComponent state, GridConfigComponent gridConfig)
        {
            Vector2 centerScreen = new Vector2(Screen.width / 2f, Screen.height / 2f);
            _previewObject.SetActive(true);

            if (PerformRaycast(physicsWorld.CollisionWorld, state.Type, centerScreen, out int3 hitGridPos))
            {
                ApplyGridPosition(hitGridPos, ref state, gridConfig);
            }
            else
            {
                int3 defaultPos = int3.zero;
                if (state.Type == PlacementType.Island) defaultPos.y = _currentPlacementLayer;
                ApplyGridPosition(defaultPos, ref state, gridConfig);
            }
        }

        private void UpdateObjectPosition(PhysicsWorldSingleton physicsWorld, Vector2 screenPos, ref PlacementStateComponent state, GridConfigComponent gridConfig)
        {
            if (PerformRaycast(physicsWorld.CollisionWorld, state.Type, screenPos, out int3 hitGridPos))
            {
                ApplyGridPosition(hitGridPos, ref state, gridConfig);
                if (!_previewObject.activeSelf) _previewObject.SetActive(true);
            }
        }

        private void ApplyGridPosition(int3 hitGridPos, ref PlacementStateComponent state, GridConfigComponent gridConfig)
        {
            int3 baseSize = GetObjectSizeFromConfig(state.CurrentObjectId, state.Type);
            int3 finalSize = (state.RotationIndex % 2 == 1) ? new int3(baseSize.z, baseSize.y, baseSize.x) : baseSize;

            int3 targetGridPos = CalculateFinalPlacementPosition(hitGridPos, finalSize, state.Type);
            state.CurrentGridPos = targetGridPos;
            state.IsPositionValid = ValidatePosition(state.Type, targetGridPos, finalSize);

            UpdatePreviewTransform(targetGridPos, finalSize, state.RotationIndex, gridConfig.CellSize);
            UpdatePreviewMaterial(state.IsPositionValid);
        }

        private bool PerformRaycast(CollisionWorld collisionWorld, PlacementType type, Vector2 screenPosition, out int3 gridPos)
        {
            gridPos = int3.zero;
            UnityEngine.Ray unityRay = _mainCamera.ScreenPointToRay(screenPosition);
            RaycastInput rayInput = new RaycastInput
            {
                Start = unityRay.origin,
                End = unityRay.origin + unityRay.direction * 5000f,
                Filter = CollisionFilter.Default
            };

            if (collisionWorld.CastRay(rayInput, out RaycastHit hit))
            {
                if (EntityManager.HasComponent<GridPositionComponent>(hit.Entity))
                {
                    gridPos = EntityManager.GetComponentData<GridPositionComponent>(hit.Entity).Value;
                    if (type == PlacementType.Island)
                    {
                        gridPos.y = _currentPlacementLayer;
                    }
                    return true;
                }
            }
            return false;
        }

        private void CleanupPreview()
        {
            if (_previewObject != null)
            {
                Object.Destroy(_previewObject);
                _previewObject = null;
            }
            _lastLoadedObjectId = -1;
            _gridVisSystem?.SetVisualizationRange(-1, -1);
            _defaultRotation = quaternion.identity;
            _lastPlacementType = PlacementType.None;

            if (_confirmPanel != null) _confirmPanel.Hide();
        }

        private void HandleTouchRotation(System.Collections.Generic.List<LeanFinger> fingers, ref PlacementStateComponent state)
        {
            float twistDegrees = LeanGesture.GetTwistDegrees(fingers);
            _twistAccumulator += twistDegrees;
            if (Mathf.Abs(_twistAccumulator) > 45f)
            {
                if (_twistAccumulator > 0) state.RotationIndex = (state.RotationIndex + 1) % 4;
                else state.RotationIndex = (state.RotationIndex - 1 + 4) % 4;
                _twistAccumulator = 0f;
                var gridConfig = SystemAPI.GetSingleton<GridConfigComponent>();
                int3 baseSize = GetObjectSizeFromConfig(state.CurrentObjectId, state.Type);
                int3 finalSize = (state.RotationIndex % 2 == 1) ? new int3(baseSize.z, baseSize.y, baseSize.x) : baseSize;
                state.IsPositionValid = ValidatePosition(state.Type, state.CurrentGridPos, finalSize);
                UpdatePreviewTransform(state.CurrentGridPos, finalSize, state.RotationIndex, gridConfig.CellSize);
                UpdatePreviewMaterial(state.IsPositionValid);
            }
        }

        // [新增] 异步刷新桥梁显示
        private async UniTaskVoid RefreshBridgeVisualsAsync()
        {
            // 等待一帧，确保 PlaceObjectRequest 被处理，GridSystem 数据已更新
            await UniTask.NextFrame();

            // 检查状态：确保玩家还在造桥模式中
            if (!SystemAPI.HasSingleton<PlacementStateComponent>()) return;
            var state = SystemAPI.GetSingleton<PlacementStateComponent>();

            if (state.IsActive && state.Type == PlacementType.Bridge)
            {
                // 这里调用的是带参数的方法，需要 GridEntityVisualizationSystem 对应修改
                _gridVisSystem?.ShowBridgeableGrids(true);
            }
        }

        // [修改] 确认放置逻辑
        public bool ConfirmPlacement()
        {
            if (!SystemAPI.HasSingleton<PlacementStateComponent>()) return true;
            var stateRef = SystemAPI.GetSingletonRW<PlacementStateComponent>();
            ref var state = ref stateRef.ValueRW;
            if (!state.IsActive || !state.IsPositionValid) return false;

            var gridConfig = SystemAPI.GetSingleton<GridConfigComponent>();
            int3 baseSize = GetObjectSizeFromConfig(state.CurrentObjectId, state.Type);
            int3 finalSize = (state.RotationIndex % 2 == 1) ? new int3(baseSize.z, baseSize.y, baseSize.x) : baseSize;

            int airSpace = 4;
            if (state.Type == PlacementType.Island)
            {
                var islandCfg = ConfigManager.Instance.Tables.TbIsland.GetOrDefault(state.CurrentObjectId);
                airSpace = islandCfg != null ? islandCfg.AirHeight : 4;
            }

            SendPlacementRequest(state.CurrentObjectId, state.Type, state.CurrentGridPos, finalSize, state.RotationIndex, airSpace);

            EventManager.Instance.Publish(new ObjectBuiltEvent { Type = state.Type });

            // [核心修改] 如果是桥梁，不退出建造模式，但触发异步刷新网格
            if (state.Type == PlacementType.Bridge)
            {
                Debug.Log("[PlacementSystem] 桥梁已放置，保持建造模式并刷新网格");
                RefreshBridgeVisualsAsync().Forget();
                // 返回 false，告诉 UI 不要关闭
                return false;
            }

            // 其他类型（建筑、岛屿）：正常退出
            state.IsActive = false;
            CleanupPreview();
            return true;
        }

        public void CancelPlacement()
        {
            if (!SystemAPI.HasSingleton<PlacementStateComponent>()) return;
            var stateRef = SystemAPI.GetSingletonRW<PlacementStateComponent>();
            stateRef.ValueRW.IsActive = false;
            CleanupPreview();
        }

        private void HandleKeyboardInput(ref PlacementStateComponent state, GridConfigComponent gridConfig)
        {
            if (Input.GetKeyDown(KeyCode.B)) { ToggleMode(ref state, PlacementType.Island, 100005); }
            if (Input.GetKeyDown(KeyCode.K)) { ToggleMode(ref state, PlacementType.Building, 200003); }
            if (Input.GetKeyDown(KeyCode.J)) { ToggleMode(ref state, PlacementType.Building, 200001); }
            if (Input.GetKeyDown(KeyCode.L)) { ToggleMode(ref state, PlacementType.Bridge, 300001); }
            if (Input.GetKeyDown(KeyCode.Escape)) { CancelPlacement(); }
            if (Input.GetKeyDown(KeyCode.R)) RotatePreview();

            if (state.IsActive && state.Type == PlacementType.Island)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeHeightLayer(1, gridConfig.Height, state.Type);
                if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeHeightLayer(-1, gridConfig.Height, state.Type);
            }
        }

        private void ToggleMode(ref PlacementStateComponent state, PlacementType type, int defaultId)
        {
            if (state.IsActive && state.Type == type) state.IsActive = false;
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
            if (_currentPlacementLayer != oldLayer && (type == PlacementType.Island || type == PlacementType.Bridge))
            {
                _gridVisSystem?.SetVisualizationRange(_currentPlacementLayer, _currentPlacementLayer);
            }
        }

        private void OnPlacementModeChanged(PlacementType newType)
        {
            if (newType == PlacementType.Building) _gridVisSystem?.ShowBuildableGrids();
            else if (newType == PlacementType.Island) _gridVisSystem?.SetVisualizationRange(_currentPlacementLayer, _currentPlacementLayer);
            else if (newType == PlacementType.Bridge) _gridVisSystem?.ShowBridgeableGrids();
            else _gridVisSystem?.SetVisualizationRange(-1, -1);
        }

        private int3 CalculateFinalPlacementPosition(int3 hitPos, int3 size, PlacementType type)
        {
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
        }

        private async UniTaskVoid CreatePreviewGameObject(int configId, PlacementType type)
        {
            _isResourceLoading = true;
            _lastLoadedObjectId = configId;

            string resourcePath = "";

            if (type == PlacementType.Building)
            {
                var cfg = ConfigManager.Instance.Tables.TbBuild.GetOrDefault(configId);
                if (cfg != null) resourcePath = cfg.ResourceName;
            }
            else if (type == PlacementType.Island)
            {
                var cfg = ConfigManager.Instance.Tables.TbIsland.GetOrDefault(configId);
                if (cfg != null) resourcePath = cfg.ResourceName;
            }
            else if (type == PlacementType.Bridge)
            {
                var cfg = ConfigManager.Instance.Tables.TbBridgeConfig.GetOrDefault(configId);
                if (cfg != null)
                {
                    resourcePath = cfg.ResourceName;
                }
                else
                {
                    resourcePath = $"bridge_{configId}";
                }
            }

            if (!string.IsNullOrEmpty(resourcePath))
            {
                try
                {
                    var prefab = await ResourceManager.Instance.LoadAssetAsync<GameObject>(resourcePath);
                    if (prefab != null)
                    {
                        if (_previewObject != null) Object.Destroy(_previewObject);
                        _previewObject = Object.Instantiate(prefab);
                        _defaultRotation = _previewObject.transform.rotation;
                        _isFirstFrameAfterLoad = true;
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[PlacementSystem] 资源加载失败 Key: {resourcePath}. Error: {e.Message}");
                }
            }
            _isResourceLoading = false;
        }

        private void UpdatePreviewTransform(int3 gridPos, int3 size, int rotIndex, float cellSize)
        {
            if (_previewObject == null) return;
            if (!_previewObject.activeSelf) _previewObject.SetActive(true);
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
            switch (type)
            {
                case PlacementType.Building:
                    var bCfg = ConfigManager.Instance.Tables.TbBuild.GetOrDefault(objectId);
                    return bCfg != null ? new int3(bCfg.Length, 1, bCfg.Width) : new int3(1, 1, 1);

                case PlacementType.Island:
                    var iCfg = ConfigManager.Instance.Tables.TbIsland.GetOrDefault(objectId);
                    return iCfg != null ? new int3(iCfg.Length, iCfg.Height, iCfg.Width) : new int3(1, 1, 1);
            }
            return new int3(1, 1, 1);
        }
    }
}