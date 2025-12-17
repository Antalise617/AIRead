using Unity.Entities;
using Unity.Physics;
using Unity.Mathematics;
using UnityEngine;

public class MousePicker : MonoBehaviour
{
    private EntityManager _entityManager;
    private EntityQuery _physicsWorldQuery;

    [Header("Debug Settings")]
    [Tooltip("是否在 Scene 视图绘制射线")]
    public bool drawDebugRay = true;
    [Tooltip("射线颜色")]
    public Color rayColor = Color.red;
    [Tooltip("射线持续显示时间(秒)")]
    public float rayDuration = 1.0f;

    void Start()
    {
        // 1. 获取默认 ECS 世界的 EntityManager
        var world = World.DefaultGameObjectInjectionWorld;
        if (world == null)
        {
            Debug.LogError("ECS World 不存在！");
            enabled = false;
            return;
        }
        _entityManager = world.EntityManager;

        // 2. 预先创建一个查询，专门用于查找 PhysicsWorldSingleton
        _physicsWorldQuery = _entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CheckHit();
        }
    }

    private void CheckHit()
    {
        // 3. 检查物理世界是否已经构建完毕
        if (!_physicsWorldQuery.HasSingleton<PhysicsWorldSingleton>())
        {
            Debug.LogWarning("PhysicsWorldSingleton 未就绪，请确保 PhysicsSystemGroup 正在运行。");
            return;
        }

        // 4. 获取物理世界单例数据
        var physicsWorldSingleton = _physicsWorldQuery.GetSingleton<PhysicsWorldSingleton>();
        var collisionWorld = physicsWorldSingleton.CollisionWorld;

        // 5. 构建射线
        UnityEngine.Ray unityRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        // 将 Unity 的 Vector3 转换为 ECS 的 float3
        float3 rayStart = unityRay.origin;
        // 注意：这里的 1000f 是射线的最大长度，如果场景很大可能需要调整
        float3 rayEnd = unityRay.origin + unityRay.direction * 1000f;

        // ==================================================================================
        // 【新增 debug 绘制功能】
        // 在这里绘制，确保画出来的是接下来立刻要拿去检测的那条线
        if (drawDebugRay)
        {
            // Debug.DrawLine 接收 Vector3，float3 可以隐式转换，但显式强转更清晰
            // 参数：起点, 终点, 颜色, 持续时间
            Debug.DrawLine((Vector3)rayStart, (Vector3)rayEnd, rayColor, rayDuration);
            Debug.Log($"[MousePicker Debug] 绘制射线: Start{rayStart} -> End{rayEnd}");
        }
        // ==================================================================================

        var rayInput = new RaycastInput
        {
            Start = rayStart,
            End = rayEnd,
            Filter = CollisionFilter.Default
        };

        // 6. 执行射线检测
        if (collisionWorld.CastRay(rayInput, out Unity.Physics.RaycastHit hit))
        {
            Entity hitEntity = hit.Entity;
            Debug.Log($"<color=green>[MousePicker] 击中实体!</color> | Index: {hitEntity.Index} | Position: {hit.Position}");

            // 如果击中了，可以在击中点再画个十字标记，方便确认
            if (drawDebugRay)
            {
                Debug.DrawRay((Vector3)hit.Position, Vector3.up * 0.5f, Color.green, rayDuration);
                Debug.DrawRay((Vector3)hit.Position, Vector3.right * 0.5f, Color.green, rayDuration);
                Debug.DrawRay((Vector3)hit.Position, Vector3.forward * 0.5f, Color.green, rayDuration);
            }
        }
        else
        {
            Debug.Log("<color=yellow>[MousePicker] 未击中任何 ECS 物体</color>");
        }
    }
}