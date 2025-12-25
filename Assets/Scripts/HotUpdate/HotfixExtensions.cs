using Unity.Entities;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

public static class HotfixExtensions
{
    // 【关键修改】增加约束 where T : unmanaged, IBufferElementData
    // 只有这样，编译器才允许把 T 用在 DynamicBuffer<T> 里
    public static unsafe void AddSafe<T>(this DynamicBuffer<T> buffer, T value)
        where T : unmanaged, IBufferElementData
    {
        // 1. 扩容
        buffer.ResizeUninitialized(buffer.Length + 1);

        // 2. 获取指针
        byte* basePtr = (byte*)buffer.GetUnsafePtr();
        int lastIndex = buffer.Length - 1;

        // 3. 计算地址
        // SizeOf<T>() 要求 T 是 unmanaged，这回匹配了
        T* destPtr = (T*)(basePtr + lastIndex * UnsafeUtility.SizeOf<T>());

        // 4. 赋值
        *destPtr = value;
    }
}