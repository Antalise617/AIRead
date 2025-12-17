using Unity.Mathematics;
using UnityEngine;
using Unity.Entities;

namespace GameFramework.Utilities
{
    public static class Extensions
    {
        // Vector扩展
        public static float3 ToFloat3(this Vector3 vector)
        {
            return new float3(vector.x, vector.y, vector.z);
        }

        public static Vector3 ToVector3(this float3 vector)
        {
            return new Vector3(vector.x, vector.y, vector.z);
        }

        public static float2 ToFloat2(this Vector2 vector)
        {
            return new float2(vector.x, vector.y);
        }

        public static Vector2 ToVector2(this float2 vector)
        {
            return new Vector2(vector.x, vector.y);
        }

        // Transform扩展
        public static void Reset(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        public static void SetX(this Transform transform, float x)
        {
            var pos = transform.position;
            pos.x = x;
            transform.position = pos;
        }

        public static void SetY(this Transform transform, float y)
        {
            var pos = transform.position;
            pos.y = y;
            transform.position = pos;
        }

        public static void SetZ(this Transform transform, float z)
        {
            var pos = transform.position;
            pos.z = z;
            transform.position = pos;
        }

        // GameObject扩展
        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            var component = go.GetComponent<T>();
            if (component == null)
            {
                component = go.AddComponent<T>();
            }
            return component;
        }

        public static void SetLayerRecursively(this GameObject go, int layer)
        {
            go.layer = layer;
            foreach (Transform child in go.transform)
            {
                child.gameObject.SetLayerRecursively(layer);
            }
        }

        // Entity扩展
        public static bool HasComponent<T>(this Entity entity, EntityManager entityManager)
        {
            return entityManager.HasComponent<T>(entity);
        }

        public static void SafeAddComponent<T>(this Entity entity, EntityManager entityManager, T component)
            where T : unmanaged, IComponentData
        {
            if (!entityManager.HasComponent<T>(entity))
            {
                entityManager.AddComponentData(entity, component);
            }
        }

        public static void SafeRemoveComponent<T>(this Entity entity, EntityManager entityManager)
            where T : unmanaged, IComponentData
        {
            if (entityManager.HasComponent<T>(entity))
            {
                entityManager.RemoveComponent<T>(entity);
            }
        }

        // Color扩展
        public static Color WithAlpha(this Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }

        public static string ToHex(this Color color)
        {
            return $"#{ColorUtility.ToHtmlStringRGB(color)}";
        }

        // 字符串扩展
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static string Truncate(this string str, int maxLength)
        {
            if (string.IsNullOrEmpty(str) || str.Length <= maxLength)
                return str;

            return str.Substring(0, maxLength) + "...";
        }
    }
}
