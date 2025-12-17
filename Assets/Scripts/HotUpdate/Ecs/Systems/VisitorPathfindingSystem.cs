using GameFramework.ECS.Components;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace GameFramework.ECS.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class VisitorPathfindingSystem : SystemBase
    {
        private GridSystem _gridSystem;

        protected override void OnStartRunning()
        {
            _gridSystem = World.GetExistingSystemManaged<GridSystem>();
        }

        protected override void OnUpdate()
        {
            // 遍历所有有待处理请求的实体
            foreach (var (req, visitor, pathBuffer) in
                     SystemAPI.Query<RefRW<PathfindingRequest>, RefRW<VisitorComponent>, DynamicBuffer<PathBufferElement>>())
            {
                if (!req.ValueRO.IsPending) continue;

                // 1. 执行 A* 寻路
                List<int3> path = _gridSystem.FindPath(req.ValueRO.StartPos, req.ValueRO.EndPos);

                // 2. 清理旧路径
                pathBuffer.Clear();

                if (path != null && path.Count > 0)
                {
                    // 3. 填充路径 (跳过起点 path[0]，从 path[1] 开始走)
                    for (int i = 1; i < path.Count; i++)
                    {
                        pathBuffer.Add(new PathBufferElement { GridPos = path[i] });
                    }

                    // 4. 切换状态 -> 移动中
                    visitor.ValueRW.CurrentState = VisitorState.Moving;
                    Debug.Log($"[Pathfinding] 寻路成功，步数: {pathBuffer.Length}");
                }
                else
                {
                    // 寻路失败，回退到 Idle，休息 3 秒
                    visitor.ValueRW.CurrentState = VisitorState.Idle;
                    visitor.ValueRW.StateTimer = 3.0f;
                    Debug.LogWarning($"[Pathfinding] 寻路失败: {req.ValueRO.StartPos} -> {req.ValueRO.EndPos}");
                }

                // 5. 关闭请求标记
                req.ValueRW.IsPending = false;
            }
        }
    }
}