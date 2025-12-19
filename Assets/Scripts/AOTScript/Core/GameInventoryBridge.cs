using UnityEngine;

namespace GameFramework.Core
{
    // 1. 定义接口
    public interface IGameInventoryService
    {
        bool HasItem(int itemId, long count);
        bool TryConsumeItem(int itemId, long count);
        void AddItem(int itemId, long count);
    }

    // 2. 定义静态桥接类
    public static class GameInventoryBridge
    {
        public static IGameInventoryService Service;

        public static bool HasItem(int itemId, long count) => Service?.HasItem(itemId, count) ?? false;

        public static bool TryConsumeItem(int itemId, long count) => Service?.TryConsumeItem(itemId, count) ?? false;

        public static void AddItem(int itemId, long count) => Service?.AddItem(itemId, count);
    }
}