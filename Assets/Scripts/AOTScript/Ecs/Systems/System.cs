using GameFramework.ECS.Components;
using GameFramework.Managers;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace GameFramework.ECS.Systems
{
    // 移动系统
    [BurstCompile]
    [UpdateInGroup(typeof(GameplaySystemGroup))]
    public partial struct MoveSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;

            /*foreach (var (transform, move) in
                SystemAPI.Query<RefRW<LocalTransform>, RefRO<MoveComponent>>())
            {
                if (math.lengthsq(move.ValueRO.Direction) > 0.001f)
                {
                    float3 movement = math.normalize(move.ValueRO.Direction)
                        * move.ValueRO.Speed * deltaTime;
                    transform.ValueRW.Position += movement;
                }
            }*/
        }
    }

    // ======================================================================================
    // 1. 输入同步系统 (主线程运行)
    // 职责：仅负责从 InputManager 搬运数据到 ECS World，不做任何游戏逻辑。
    // ======================================================================================
    [UpdateInGroup(typeof(InitializationSystemGroup))] // 在游戏逻辑开始前尽早更新
    public partial class InputSyncSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<GridConfigComponent>(); // 等待网格配置初始化
            if (!SystemAPI.HasSingleton<GlobalInputComponent>())
            {
                EntityManager.CreateSingleton<GlobalInputComponent>();
            }
        }

        protected override void OnUpdate()
        {
            var inputData = InputManager.Instance.GetInputData();
            var gridConfig = SystemAPI.GetSingleton<GridConfigComponent>();

            // --- 射线检测逻辑 (使用 UnityEngine 的物理系统) ---
            int3 hoverGridPos = int3.zero;
            bool hasTarget = false;

            if (UnityEngine.Camera.main != null)
            {
                UnityEngine.Ray ray = UnityEngine.Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
                // 这里假设 y=0 是基准平面，你可以替换为 LayerMask 检测特定层
                UnityEngine.Plane groundPlane = new UnityEngine.Plane(UnityEngine.Vector3.up, UnityEngine.Vector3.zero);

                if (groundPlane.Raycast(ray, out float enter))
                {
                    UnityEngine.Vector3 hitPoint = ray.GetPoint(enter);
                    // 将世界坐标转换为网格坐标
                    hoverGridPos = new int3(
                        (int)math.round(hitPoint.x / gridConfig.CellSize),
                        0, // 暂时默认在地面，如果是多层需要根据hitPoint.y计算
                        (int)math.round(hitPoint.z / gridConfig.CellSize)
                    );
                    hasTarget = true;
                }
            }

            // 更新 ECS 中的输入单例
            SystemAPI.SetSingleton(new GlobalInputComponent
            {
                // 映射按键
                IsConfirmPlace = UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Mouse0) || UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Space),
                IsCancelPlace = UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Escape),
                HoverGridPosition = hoverGridPos,
                HasHoverTarget = hasTarget
            });
        }
    }
    // ======================================================================================
    // 4. 销毁系统 (优化版 - 使用 ECB 单例)
    // ======================================================================================
    [BurstCompile]
    [UpdateInGroup(typeof(GameplaySystemGroup))]
    //[UpdateAfter(typeof(DamageSystem))]
    public partial struct DestroySystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            new DestroyEntityJob
            {
                Ecb = ecb.AsParallelWriter()
            }.ScheduleParallel();
        }

        [BurstCompile]
        partial struct DestroyEntityJob : IJobEntity
        {
            public EntityCommandBuffer.ParallelWriter Ecb;

            void Execute(Entity entity, [EntityIndexInQuery] int sortKey, in DestroyTag tag)
            {
                Ecb.DestroyEntity(sortKey, entity);
            }
        }
    }

}
