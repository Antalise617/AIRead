using Unity.Mathematics;
using UnityEngine;

namespace GameFramework.Utilities
{
    public static class MathUtility
    {
        /// <summary>
        /// 平滑阻尼
        /// </summary>
        public static float SmoothDamp(float current, float target, ref float velocity,
            float smoothTime, float deltaTime, float maxSpeed = Mathf.Infinity)
        {
            smoothTime = math.max(0.0001f, smoothTime);
            float omega = 2f / smoothTime;
            float x = omega * deltaTime;
            float exp = 1f / (1f + x + 0.48f * x * x + 0.235f * x * x * x);
            float change = current - target;
            float originalTo = target;
            float maxChange = maxSpeed * smoothTime;
            change = math.clamp(change, -maxChange, maxChange);
            target = current - change;
            float temp = (velocity + omega * change) * deltaTime;
            velocity = (velocity - omega * temp) * exp;
            float output = target + (change + temp) * exp;

            if (originalTo - current > 0.0f == output > originalTo)
            {
                output = originalTo;
                velocity = (output - originalTo) / deltaTime;
            }

            return output;
        }

        /// <summary>
        /// 3D平滑阻尼
        /// </summary>
        public static float3 SmoothDamp(float3 current, float3 target, ref float3 velocity,
            float smoothTime, float deltaTime, float maxSpeed = Mathf.Infinity)
        {
            return new float3(
                SmoothDamp(current.x, target.x, ref velocity.x, smoothTime, deltaTime, maxSpeed),
                SmoothDamp(current.y, target.y, ref velocity.y, smoothTime, deltaTime, maxSpeed),
                SmoothDamp(current.z, target.z, ref velocity.z, smoothTime, deltaTime, maxSpeed)
            );
        }

        /// <summary>
        /// 弹性插值
        /// </summary>
        public static float EaseOutElastic(float t)
        {
            float p = 0.3f;
            return math.pow(2f, -10f * t) * math.sin((t - p / 4f) * (2f * math.PI) / p) + 1f;
        }

        /// <summary>
        /// 回弹插值
        /// </summary>
        public static float EaseOutBounce(float t)
        {
            if (t < 1f / 2.75f)
            {
                return 7.5625f * t * t;
            }
            else if (t < 2f / 2.75f)
            {
                t -= 1.5f / 2.75f;
                return 7.5625f * t * t + 0.75f;
            }
            else if (t < 2.5f / 2.75f)
            {
                t -= 2.25f / 2.75f;
                return 7.5625f * t * t + 0.9375f;
            }
            else
            {
                t -= 2.625f / 2.75f;
                return 7.5625f * t * t + 0.984375f;
            }
        }

        /// <summary>
        /// 2D向量转角度
        /// </summary>
        public static float Vector2ToAngle(float2 vector)
        {
            return math.atan2(vector.y, vector.x) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// 角度转2D向量
        /// </summary>
        public static float2 AngleToVector2(float angle)
        {
            float radian = angle * Mathf.Deg2Rad;
            return new float2(math.cos(radian), math.sin(radian));
        }

        /// <summary>
        /// 随机圆内点
        /// </summary>
        public static float2 RandomInCircle(float radius, Unity.Mathematics.Random random)
        {
            float angle = random.NextFloat(0, 2f * math.PI);
            float distance = math.sqrt(random.NextFloat()) * radius;
            return new float2(math.cos(angle) * distance, math.sin(angle) * distance);
        }

        /// <summary>
        /// 随机球内点
        /// </summary>
        public static float3 RandomInSphere(float radius, Unity.Mathematics.Random random)
        {
            float theta = random.NextFloat(0, 2f * math.PI);
            float phi = math.acos(2f * random.NextFloat() - 1f);
            float r = math.pow(random.NextFloat(), 1f / 3f) * radius;

            return new float3(
                r * math.sin(phi) * math.cos(theta),
                r * math.sin(phi) * math.sin(theta),
                r * math.cos(phi)
            );
        }

        /// <summary>
        /// 贝塞尔曲线
        /// </summary>
        public static float3 BezierCurve(float3 p0, float3 p1, float3 p2, float3 p3, float t)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            float3 p = uuu * p0;
            p += 3 * uu * t * p1;
            p += 3 * u * tt * p2;
            p += ttt * p3;

            return p;
        }

        /// <summary>
        /// 重映射数值
        /// </summary>
        public static float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            return (value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
        }
    }
}
