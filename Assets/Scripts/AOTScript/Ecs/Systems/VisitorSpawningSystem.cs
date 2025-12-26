using Cysharp.Threading.Tasks;
using GameFramework.ECS.Components;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace GameFramework.ECS.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class VisitorSpawningSystem : SystemBase
    {
        private EntityFactory _entityFactory;
        private Random _random;

        // 游客配置定义 (实际项目中应从配置表读取)
        private const int VISITOR_CONFIG_ID = 90001;
        private const string VISITOR_PREFAB_PATH = "Tourist";

        // 防止重复加载的标志集合
        private HashSet<int> _loadingAssets = new HashSet<int>();

        // [新增] 用于临时存储生成请求的结构体
        private struct PendingSpawn
        {
            public int3 GridPos;
            public float3 WorldPos;
        }

        protected override void OnCreate()
        {
            // 初始化工厂
            _entityFactory = new EntityFactory(EntityManager);
            _random = new Random(1234);
            RequireForUpdate<GridConfigComponent>();
        }

        protected override void OnDestroy()
        {
            _entityFactory.Dispose();
            base.OnDestroy();
        }

        protected override void OnUpdate()
        {
            float dt = SystemAPI.Time.DeltaTime;
            float cellSize = SystemAPI.GetSingleton<GridConfigComponent>().CellSize;

            // [核心修复] 创建一个临时列表来存储本帧需要生成的请求
            // Allocator.Temp 表示这帧结束就会释放，非常高效
            using var pendingSpawns = new NativeList<PendingSpawn>(Allocator.Temp);

            // 1. 第一步：遍历逻辑与决策 (不能做结构性修改)
            foreach (var (center, gridPos, entity) in SystemAPI.Query<RefRW<VisitorCenterComponent>, RefRO<GridPositionComponent>>()
                         .WithEntityAccess())
            {
                // 检查库存
                if (center.ValueRO.UnspawnedVisitorCount <= 0) continue;

                // 更新计时器 (修改组件数据是允许的，不属于结构性变化)
                center.ValueRW.SpawnTimer -= dt;
                if (center.ValueRO.SpawnTimer > 0) continue;

                // 检查工厂是否有原型缓存
                if (_entityFactory.HasEntity(VISITOR_CONFIG_ID))
                {
                    // === 决策通过：记录生成请求 ===

                    // 扣除库存，重置计时器 (先在逻辑层完成状态更新)
                    center.ValueRW.UnspawnedVisitorCount--;
                    center.ValueRW.SpawnTimer = center.ValueRO.SpawnInterval;

                    // 计算世界坐标
                    float3 spawnPos = new float3(
                        gridPos.ValueRO.Value.x * cellSize,
                        gridPos.ValueRO.Value.y * cellSize + 0.5f,
                        gridPos.ValueRO.Value.z * cellSize
                    );

                    // 添加到待处理列表
                    pendingSpawns.Add(new PendingSpawn
                    {
                        GridPos = gridPos.ValueRO.Value,
                        WorldPos = spawnPos
                    });
                }
                else
                {
                    // === 资源未加载：触发加载 ===
                    if (!_loadingAssets.Contains(VISITOR_CONFIG_ID))
                    {
                        _loadingAssets.Add(VISITOR_CONFIG_ID);
                        Debug.Log($"[VisitorSystem] 开始加载游客资源: {VISITOR_PREFAB_PATH}");
                        LoadAssetAndCleanState(VISITOR_CONFIG_ID, VISITOR_PREFAB_PATH).Forget();
                    }
                }
            }

            // 2. 第二步：执行结构性变化 (此时 Query 循环已结束，World 解锁)
            foreach (var req in pendingSpawns)
            {
                // 通过工厂同步生成实体 (这里调用 Instantiate 是安全的)
                Entity visitorEntity = _entityFactory.SpawnEntity(
                    VISITOR_CONFIG_ID,
                    req.WorldPos,
                    quaternion.identity
                );

                if (visitorEntity != Entity.Null)
                {
                    // 挂载/初始化游客组件
                    EntityManager.AddComponentData(visitorEntity, new VisitorComponent
                    {
                        Name = new FixedString64Bytes($"Visitor_{_random.NextInt(1000, 9999)}"),
                        Age = _random.NextInt(18, 90),
                        MoveSpeed = _random.NextFloat(1.5f, 3.5f),
                        CurrentState = VisitorState.Idle,
                    });

                    // 确保有 GridPositionComponent
                    EntityManager.AddComponentData(visitorEntity, new GridPositionComponent
                    {
                        Value = req.GridPos
                    });

                    // [重要] 添加之前的移动系统所需的 Buffer
                    EntityManager.AddBuffer<PathBufferElement>(visitorEntity);
                    EntityManager.AddBuffer<VisitedBuildingElement>(visitorEntity);
                    EntityManager.AddComponent<PathfindingRequest>(visitorEntity);

                    Debug.Log($"[VisitorSystem] 成功生成游客 @ {req.GridPos}");
                }
            }
        }

        // 异步加载辅助方法
        private async UniTaskVoid LoadAssetAndCleanState(int id, string path)
        {
            var entity = await _entityFactory.LoadEntityArchetypeAsync(id, path);
            if (entity != Entity.Null)
            {
                Debug.Log($"[VisitorSystem] 资源加载完成: {id}");
            }
            else
            {
                Debug.LogError($"[VisitorSystem] 资源加载失败: {path}");
            }
            _loadingAssets.Remove(id);
        }
    }
}