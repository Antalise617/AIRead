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
        private int _currentPlacementLayer = 5;

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

            UpdatePreviewTransform(state.CurrentGridPos, finalSize, state.RotationIndex, gridConfig.CellSize, state.Type);
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

            UpdatePreviewTransform(targetGridPos, finalSize, state.RotationIndex, gridConfig.CellSize, state.Type);
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
                UpdatePreviewTransform(state.CurrentGridPos, finalSize, state.RotationIndex, gridConfig.CellSize, state.Type);
                UpdatePreviewMaterial(state.IsPositionValid);
            }
        }

        // [核心修改] 异步等待网格真正更新后再刷新
        private async UniTaskVoid RefreshBridgeVisualsAsync(int3 checkPos)
        {
            // 设置超时，防止死循环 (3秒)
            float timeout = 3.0f;
            float timer = 0f;

            // 循环等待，直到 GridSystem 认为该位置已经“不可建造桥梁”（说明桥梁已经成功注册占位）
            // 注意：IsBridgeBuildable 返回 true 代表可以造，返回 false 代表被占用或不可造
            while (_gridSystem.IsBridgeBuildable(checkPos) && timer < timeout)
            {
                await UniTask.NextFrame();
                // [修复1] 显式使用 UnityEngine.Time，解决 SystemBase.Time 冲突
                timer += UnityEngine.Time.deltaTime;
            }

            // 确保玩家还在造桥模式中
            if (!SystemAPI.HasSingleton<PlacementStateComponent>()) return;

            // [修复2] 异步方法中不能使用 RefRW (ref struct)，改用 Get/Set Singleton 副本
            var state = SystemAPI.GetSingleton<PlacementStateComponent>();

            if (state.IsActive && state.Type == PlacementType.Bridge)
            {
                // 1. 刷新网格显示（此时新桥梁的四周应该已经是可建造状态）
                _gridVisSystem?.ShowBridgeableGrids(true);

                // 2. 强制重新检测当前虚影位置的有效性
                // 因为桥刚造好，当前位置应该变红（无效）
                var gridConfig = SystemAPI.GetSingleton<GridConfigComponent>();
                int3 baseSize = GetObjectSizeFromConfig(state.CurrentObjectId, state.Type);
                int3 finalSize = (state.RotationIndex % 2 == 1) ? new int3(baseSize.z, baseSize.y, baseSize.x) : baseSize;

                state.IsPositionValid = ValidatePosition(state.Type, state.CurrentGridPos, finalSize);

                // 将修改后的状态写回 ECS
                SystemAPI.SetSingleton(state);

                // 更新虚影颜色 (Green -> Red)
                UpdatePreviewTransform(state.CurrentGridPos, finalSize, state.RotationIndex, gridConfig.CellSize, state.Type);
                UpdatePreviewMaterial(state.IsPositionValid);

                Debug.Log($"[PlacementSystem] 桥梁网格刷新完成，当前位置有效性: {state.IsPositionValid}");
            }
        }
        private (cfg.Island config, int visualId) GetIslandConfigWithFallback(int objectId)
        {
            // 1. 尝试直接查找
            var cfg = ConfigManager.Instance.Tables.TbIsland.GetOrDefault(objectId);
            if (cfg != null) return (cfg, objectId);

            // 2. 检查是否是有效的等级ID (例如 102001)
            var levelCfg = ConfigManager.Instance.Tables.TbIslandLevel.GetOrDefault(objectId);
            if (levelCfg != null)
            {
                // 3. 回退到默认 ID 101001 获取视觉数据
                var defaultCfg = ConfigManager.Instance.Tables.TbIsland.GetOrDefault(101001);
                if (defaultCfg != null) return (defaultCfg, 101001);
            }

            return (null, 0);
        }
        public async UniTask ConfirmPlacement()
        {
            if (!SystemAPI.HasSingleton<PlacementStateComponent>()) return;

            var stateRef = SystemAPI.GetSingletonRW<PlacementStateComponent>();
            var state = stateRef.ValueRO;

            if (!state.IsActive || !state.IsPositionValid)
            {
                Debug.LogWarning("放置状态无效或位置非法，无法确认放置");
                return;
            }

            int objectId = state.CurrentObjectId;
            int3 gridPos = state.CurrentGridPos;
            int rotation = state.RotationIndex;
            PlacementType type = state.Type;
            bool isSuccess = false;

            // 获取配置用于后续逻辑
            cfg.Island islandCfg = null;
            int visualId = objectId;
            if (type == PlacementType.Island)
            {
                var result = GetIslandConfigWithFallback(objectId);
                islandCfg = result.config;
                visualId = result.visualId; // 如果回退了，这里就是 101001
            }

            try
            {
                if (type == PlacementType.Building || type == PlacementType.Bridge)
                {
                    // ... (建筑请求逻辑保持不变) ...
                    var dto = new BuildingCreateDTO
                    {
                        building_id = objectId,
                        posX = gridPos.x,
                        posY = gridPos.z,
                        posZ = gridPos.y,
                        rotate = rotation
                    };
                    var result = await NetworkManager.Instance.SendAsync<GamesDTO>("/building/create", dto);
                    isSuccess = (result != null);
                }
                else if (type == PlacementType.Island)
                {
                    // 岛屿请求
                    var dto = new TileCreateDTO
                    {
                        // [注意] 这里发送用户选中的真实 ID (102001)，让服务器去处理它是哪个 Type
                        // 或者是如果服务器只认 Type，这里可能需要传 visualId (101001)
                        // 根据你的描述 "岛屿表只有101001...默认数据"，通常传具体 ID (102001) 更符合逻辑，服务器会返回对应的 tile_type
                        tile_type = objectId,
                        posX = gridPos.x,
                        posY = gridPos.z,
                        posZ = gridPos.y
                    };

                    Debug.Log($"[PlacementSystem] 发送地块请求: ID={objectId} -> ServerPos=({dto.posX},{dto.posY},{dto.posZ})");
                    var result = await NetworkManager.Instance.SendAsync<GamesDTO>("/tile/create", dto);
                    isSuccess = (result != null);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[PlacementSystem] 网络请求异常: {e.Message}");
                return;
            }

            if (isSuccess)
            {
                var query = EntityManager.CreateEntityQuery(typeof(PlacementStateComponent));
                if (!query.IsEmptyIgnoreFilter)
                {
                    var entity = query.GetSingletonEntity();
                    var currentState = EntityManager.GetComponentData<PlacementStateComponent>(entity);
                    var gridConfig = SystemAPI.GetSingleton<GridConfigComponent>();

                    // 获取尺寸 (如果是岛屿，使用回退后的配置)
                    int3 baseSize;
                    int airSpace = 4;

                    if (type == PlacementType.Island)
                    {
                        // 使用之前解析好的 config
                        baseSize = islandCfg != null ? new int3(islandCfg.Length, islandCfg.Height, islandCfg.Width) : new int3(1, 1, 1);
                        airSpace = islandCfg != null ? islandCfg.AirHeight : 4;
                    }
                    else
                    {
                        baseSize = GetObjectSizeFromConfig(objectId, type);
                    }

                    int3 finalSize = (rotation % 2 == 1) ? new int3(baseSize.z, baseSize.y, baseSize.x) : baseSize;

                    // [关键] 本地生成时，如果是岛屿，必须使用 visualId (101001)，否则 SpawningSystem 加载不到资源
                    int spawnId = (type == PlacementType.Island) ? visualId : objectId;

                    SendPlacementRequest(spawnId, type, gridPos, finalSize, rotation, airSpace);
                    EventManager.Instance.Publish(new ObjectBuiltEvent { Type = type });

                    if (type == PlacementType.Bridge)
                    {
                        RefreshBridgeVisualsAsync(gridPos).Forget();
                        return;
                    }

                    currentState.IsActive = false;
                    EntityManager.SetComponentData(entity, currentState);
                    CleanupPreview();
                }
            }
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
                // 使用回退逻辑获取资源名
                var result = GetIslandConfigWithFallback(configId);
                if (result.config != null) resourcePath = result.config.ResourceName;
            }
            else if (type == PlacementType.Bridge)
            {
                // ... (桥梁逻辑保持不变)
                var cfg = ConfigManager.Instance.Tables.TbBridgeConfig.GetOrDefault(configId);
                resourcePath = cfg != null ? cfg.ResourceName : $"bridge_{configId}";
            }

            if (!string.IsNullOrEmpty(resourcePath))
            {
                // ... (加载资源逻辑保持不变)
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
                catch { /*...*/ }
            }
            _isResourceLoading = false;
        }

        private void UpdatePreviewTransform(int3 gridPos, int3 size, int rotIndex, float cellSize, PlacementType type)
        {
            if (_previewObject == null) return;
            if (!_previewObject.activeSelf) _previewObject.SetActive(true);

            float3 worldPos = _gridSystem.CalculateObjectCenterWorldPosition(gridPos, size);

            if (type == PlacementType.Island)
            {
                worldPos.y += 1f;
            }

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
                    // 使用回退逻辑
                    var result = GetIslandConfigWithFallback(objectId);
                    var iCfg = result.config;
                    return iCfg != null ? new int3(iCfg.Length, iCfg.Height, iCfg.Width) : new int3(1, 1, 1);
            }
            return new int3(1, 1, 1);
        }
    }
}