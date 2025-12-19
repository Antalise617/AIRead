using Cysharp.Threading.Tasks;
using GameFramework.Managers;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameFramework.ECS
{
    public class EntityFactory
    {
        private readonly EntityManager _entityManager;

        // 缓存原型实体：Key = ConfigID, Value = Entity(带有RenderMesh和默认数据的原型)
        private readonly Dictionary<int, Entity> _entityPrefabCache = new Dictionary<int, Entity>();

        public EntityFactory(EntityManager entityManager)
        {
            _entityManager = entityManager;
        }

        public void Dispose()
        {
            _entityPrefabCache.Clear();
        }

        // =========================================================
        // 【新增】查询接口，用于系统判断是否需要触发异步加载
        // =========================================================
        public bool HasEntity(int configId)
        {
            return _entityPrefabCache.ContainsKey(configId);
        }

        #region 核心资源加载与原型构建

        /// <summary>
        /// 【公开接口】预加载实体原型
        /// </summary>
        public async UniTask<Entity> LoadEntityArchetypeAsync(int configId, string resourcePath)
        {
            return await GetOrCreateEntityPrefabAsync(configId, resourcePath);
        }

        /// <summary>
        /// 【公开接口】同步生成实体（必须确保资源已通过 LoadEntityArchetypeAsync 加载）
        /// </summary>
        public Entity SpawnEntity(int configId, float3 position, quaternion rotation, float scale = 1f)
        {
            if (!_entityPrefabCache.TryGetValue(configId, out Entity prefabEntity))
            {
                Debug.LogError($"[EntityFactory] 原型未加载，请先调用 LoadEntityArchetypeAsync. ConfigID: {configId}");
                return Entity.Null;
            }

            // ECS 实例化非常快
            Entity newEntity = _entityManager.Instantiate(prefabEntity);

            // 设置位置信息
            _entityManager.SetComponentData(newEntity, new LocalTransform
            {
                Position = position,
                Rotation = rotation,
                Scale = scale
            });

            // 移除 Prefab 标签，这样它就会被 Systems 处理并渲染
            _entityManager.RemoveComponent<Prefab>(newEntity);

            return newEntity;
        }


        // 在 EntityFactory.cs 中
        // 确保引入命名空间

        public Entity SpawnColliderEntity(int configId, float3 position, quaternion rotation, BoxGeometry ccb, float scale = 1f)
        {
            if (!_entityPrefabCache.TryGetValue(configId, out Entity prefabEntity))
            {
                Debug.LogError($"[EntityFactory] 原型未加载. ConfigID: {configId}");
                return Entity.Null;
            }

            // 1. 创建碰撞体数据
            // 【重要提示】请确保 ccb.Size 不是 (0,0,0)，否则碰撞盒是无限小的，看不到也摸不着
            BlobAssetReference<Unity.Physics.Collider> colliderBlob = Unity.Physics.BoxCollider.Create(ccb, CollisionFilter.Default);

            // 2. 实例化实体
            Entity newEntity = _entityManager.Instantiate(prefabEntity);

            // 3. 设置 Transform
            _entityManager.SetComponentData(newEntity, new LocalTransform
            {
                Position = position,
                Rotation = rotation,
                Scale = scale
            });

            // 4. 添加 PhysicsCollider 组件
            _entityManager.AddComponentData(newEntity, new PhysicsCollider { Value = colliderBlob });

            // =========================================================================================
            // 【核心修复】必须添加 PhysicsWorldIndex，否则物理系统会忽略它！
            // 0 代表默认的主物理世界。这是一个 SharedComponent。
            _entityManager.AddSharedComponent(newEntity, new PhysicsWorldIndex { Value = 0 });
            // =========================================================================================

            // 5. 【保险起见】确保有 LocalToWorld
            // 虽然 System 可能会加，但手动显式添加最稳妥，静态物体依赖它确定世界坐标
            if (!_entityManager.HasComponent<LocalToWorld>(newEntity))
            {
                _entityManager.AddComponent<LocalToWorld>(newEntity);
            }

            // 6. 移除 Prefab 标签
            _entityManager.RemoveComponent<Prefab>(newEntity);

            return newEntity;
        }


        private async UniTask<Entity> GetOrCreateEntityPrefabAsync(int configId, string resourcePath)
        {
            // 1. 检查缓存
            if (_entityPrefabCache.TryGetValue(configId, out Entity prefabEntity))
            {
                if (_entityManager.Exists(prefabEntity)) return prefabEntity;
                _entityPrefabCache.Remove(configId);
            }

            // 2. 加载资源
            GameObject assetGo = await ResourceManager.Instance.LoadAssetAsync<GameObject>(resourcePath);
            if (assetGo == null) return Entity.Null;

            // 3. 创建根实体
            prefabEntity = _entityManager.CreateEntity();

            // 添加 Prefab 标签
            _entityManager.AddComponent<Prefab>(prefabEntity);

            // 【关键修改】使用 List 临时存储所有实体，避免 buffer 句柄失效问题
            var allEntitiesInHierarchy = new List<Entity>();
            allEntitiesInHierarchy.Add(prefabEntity); // 根节点是第一个

            // 4. 递归转换层级 (传入 List)
            ConvertGameObjectToEntityRecursive(assetGo, prefabEntity, allEntitiesInHierarchy);

            // 【关键修改】递归结束后，一次性添加 LinkedEntityGroup
            var linkedGroup = _entityManager.AddBuffer<LinkedEntityGroup>(prefabEntity);
            foreach (var entity in allEntitiesInHierarchy)
            {
                linkedGroup.Add(new LinkedEntityGroup { Value = entity });
            }

            // 5. 缓存
            _entityPrefabCache[configId] = prefabEntity;

            return prefabEntity;
        }

        /// <summary>
        /// 递归遍历 GameObject 层级并转换为 Entity 层级
        /// </summary>
        /// <summary>
        /// 递归遍历 GameObject 层级并转换为 Entity 层级
        /// </summary>
        private void ConvertGameObjectToEntityRecursive(GameObject go, Entity entity, List<Entity> entityList)
        {
            // --- A. 处理变换组件 (LocalTransform) ---
            _entityManager.AddComponentData(entity, LocalTransform.FromPositionRotationScale(
                go.transform.localPosition,
                go.transform.localRotation,
                go.transform.localScale.x
            ));

            // --- B. 处理渲染组件 (Mesh / Sprite) ---
            Mesh mesh = null;
            UnityEngine.Material material = null;

            if (go.TryGetComponent<MeshFilter>(out var mf) && go.TryGetComponent<MeshRenderer>(out var mr))
            {
                mesh = mf.sharedMesh;
                material = mr.sharedMaterial;
            }
            else if (go.TryGetComponent<SpriteRenderer>(out var sr) && sr.sprite != null)
            {
                mesh = CreateMeshFromSprite(sr.sprite);
                material = sr.sharedMaterial;
            }

            if (mesh != null && material != null)
            {
                var renderMeshArray = new RenderMeshArray(new[] { material }, new[] { mesh });
                var renderMeshDescription = new RenderMeshDescription(
                    shadowCastingMode: ShadowCastingMode.Off,
                    receiveShadows: true
                );

                RenderMeshUtility.AddComponents(
                    entity,
                    _entityManager,
                    renderMeshDescription,
                    renderMeshArray,
                    MaterialMeshInfo.FromRenderMeshArrayIndices(0, 0)
                );
            }

            // --- C. 递归处理子物体 ---
            foreach (Transform child in go.transform)
            {
                // 1. 创建子实体 (这里发生了结构性变化，但我们不再持有 Buffer，所以是安全的)
                Entity childEntity = _entityManager.CreateEntity();

                // 2. 设置父子关系
                _entityManager.AddComponentData(childEntity, new Parent { Value = entity });
                _entityManager.AddComponent<LocalToWorld>(childEntity);

                // 3. 【修改】添加到 List 中
                entityList.Add(childEntity);

                // 4. 递归
                ConvertGameObjectToEntityRecursive(child.gameObject, childEntity, entityList);
            }
        }

        /// <summary>
        /// 辅助方法：将 Unity Sprite 数据转换为 Mesh
        /// </summary>
        private Mesh CreateMeshFromSprite(Sprite sprite)
        {
            var mesh = new Mesh();
            mesh.name = sprite.name + "_GeneratedMesh";

            // 获取 Sprite 的顶点数据 (Vector2 -> Vector3)
            var spriteVertices = sprite.vertices;
            var vertices = new Vector3[spriteVertices.Length];
            for (int i = 0; i < spriteVertices.Length; i++)
            {
                vertices[i] = (Vector3)spriteVertices[i];
            }

            // 获取三角形索引 (ushort -> int)
            var spriteTriangles = sprite.triangles;
            var triangles = new int[spriteTriangles.Length];
            for (int i = 0; i < spriteTriangles.Length; i++)
            {
                triangles[i] = (int)spriteTriangles[i];
            }

            mesh.vertices = vertices;
            mesh.uv = sprite.uv; // 直接使用 Sprite 图集中的 UV
            mesh.triangles = triangles;

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        #endregion
    }
}