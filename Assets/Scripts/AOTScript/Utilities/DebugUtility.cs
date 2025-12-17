using UnityEngine;
using Unity.Mathematics;

namespace GameFramework.Utilities
{
    public static class DebugUtility
    {
        private static bool _debugEnabled = true;

        public static void SetDebugEnabled(bool enabled)
        {
            _debugEnabled = enabled;
        }

        // 绘制圆形
        public static void DrawCircle(float3 center, float radius, Color color,
            float duration = 0f, int segments = 32)
        {
            if (!_debugEnabled) return;

            float angleStep = 360f / segments;
            float3 prevPoint = center + new float3(radius, 0, 0);

            for (int i = 1; i <= segments; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                float3 newPoint = center + new float3(
                    math.cos(angle) * radius,
                    0,
                    math.sin(angle) * radius
                );

                Debug.DrawLine(prevPoint.ToVector3(), newPoint.ToVector3(), color, duration);
                prevPoint = newPoint;
            }
        }

        // 绘制球体
        public static void DrawSphere(float3 center, float radius, Color color,
            float duration = 0f, int segments = 16)
        {
            if (!_debugEnabled) return;

            // XY平面圆
            DrawCircleOnPlane(center, radius, float3.zero, new float3(0, 0, 1), color, duration, segments);
            // XZ平面圆
            DrawCircleOnPlane(center, radius, float3.zero, new float3(0, 1, 0), color, duration, segments);
            // YZ平面圆
            DrawCircleOnPlane(center, radius, float3.zero, new float3(1, 0, 0), color, duration, segments);
        }

        private static void DrawCircleOnPlane(float3 center, float radius, float3 offset,
            float3 normal, Color color, float duration, int segments)
        {
            float angleStep = 360f / segments;
            quaternion rotation = quaternion.LookRotation(normal, math.up());
            float3 prevPoint = center + math.mul(rotation, new float3(radius, 0, 0)) + offset;

            for (int i = 1; i <= segments; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                float3 localPoint = new float3(
                    math.cos(angle) * radius,
                    math.sin(angle) * radius,
                    0
                );
                float3 newPoint = center + math.mul(rotation, localPoint) + offset;

                Debug.DrawLine(prevPoint.ToVector3(), newPoint.ToVector3(), color, duration);
                prevPoint = newPoint;
            }
        }

        // 绘制方向箭头
        public static void DrawArrow(float3 start, float3 direction, Color color,
            float duration = 0f, float arrowHeadLength = 0.3f, float arrowHeadAngle = 20f)
        {
            if (!_debugEnabled) return;

            float3 end = start + direction;
            Debug.DrawLine(start.ToVector3(), end.ToVector3(), color, duration);

            if (math.lengthsq(direction) > 0.001f)
            {
                float3 right = math.cross(direction, math.up());
                if (math.lengthsq(right) < 0.001f)
                {
                    right = math.cross(direction, math.right());
                }
                right = math.normalize(right);

                float3 forward = math.normalize(direction);
                quaternion rotation1 = quaternion.AxisAngle(right, arrowHeadAngle * Mathf.Deg2Rad);
                quaternion rotation2 = quaternion.AxisAngle(right, -arrowHeadAngle * Mathf.Deg2Rad);

                float3 arrowPoint1 = end + math.mul(rotation1, -forward * arrowHeadLength);
                float3 arrowPoint2 = end + math.mul(rotation2, -forward * arrowHeadLength);

                Debug.DrawLine(end.ToVector3(), arrowPoint1.ToVector3(), color, duration);
                Debug.DrawLine(end.ToVector3(), arrowPoint2.ToVector3(), color, duration);
            }
        }

        // 绘制盒子
        public static void DrawBox(float3 center, float3 size, Color color, float duration = 0f)
        {
            if (!_debugEnabled) return;

            float3 halfSize = size * 0.5f;

            // 底面四个角
            float3 p0 = center + new float3(-halfSize.x, -halfSize.y, -halfSize.z);
            float3 p1 = center + new float3(halfSize.x, -halfSize.y, -halfSize.z);
            float3 p2 = center + new float3(halfSize.x, -halfSize.y, halfSize.z);
            float3 p3 = center + new float3(-halfSize.x, -halfSize.y, halfSize.z);

            // 顶面四个角
            float3 p4 = center + new float3(-halfSize.x, halfSize.y, -halfSize.z);
            float3 p5 = center + new float3(halfSize.x, halfSize.y, -halfSize.z);
            float3 p6 = center + new float3(halfSize.x, halfSize.y, halfSize.z);
            float3 p7 = center + new float3(-halfSize.x, halfSize.y, halfSize.z);

            // 底面
            Debug.DrawLine(p0.ToVector3(), p1.ToVector3(), color, duration);
            Debug.DrawLine(p1.ToVector3(), p2.ToVector3(), color, duration);
            Debug.DrawLine(p2.ToVector3(), p3.ToVector3(), color, duration);
            Debug.DrawLine(p3.ToVector3(), p0.ToVector3(), color, duration);

            // 顶面
            Debug.DrawLine(p4.ToVector3(), p5.ToVector3(), color, duration);
            Debug.DrawLine(p5.ToVector3(), p6.ToVector3(), color, duration);
            Debug.DrawLine(p6.ToVector3(), p7.ToVector3(), color, duration);
            Debug.DrawLine(p7.ToVector3(), p4.ToVector3(), color, duration);

            // 连接线
            Debug.DrawLine(p0.ToVector3(), p4.ToVector3(), color, duration);
            Debug.DrawLine(p1.ToVector3(), p5.ToVector3(), color, duration);
            Debug.DrawLine(p2.ToVector3(), p6.ToVector3(), color, duration);
            Debug.DrawLine(p3.ToVector3(), p7.ToVector3(), color, duration);
        }

        // 绘制路径
        public static void DrawPath(float3[] points, Color color, float duration = 0f, bool closed = false)
        {
            if (!_debugEnabled || points == null || points.Length < 2) return;

            for (int i = 0; i < points.Length - 1; i++)
            {
                Debug.DrawLine(points[i].ToVector3(), points[i + 1].ToVector3(), color, duration);
            }

            if (closed && points.Length > 2)
            {
                Debug.DrawLine(points[points.Length - 1].ToVector3(), points[0].ToVector3(), color, duration);
            }
        }

        // 性能日志
        public static void LogPerformance(string message, System.Diagnostics.Stopwatch stopwatch)
        {
            if (!_debugEnabled) return;

            Debug.Log($"[Performance] {message}: {stopwatch.ElapsedMilliseconds}ms");
        }
    }
}
