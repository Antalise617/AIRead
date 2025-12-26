using UnityEngine;
using Unity.Entities;
using GameFramework.ECS.Components;

namespace GameFramework.Core
{
    /// <summary>
    /// 用于强制 IL2CPP 生成 AOT 泛型代码
    /// 这个类不需要挂载，也不需要被调用
    /// </summary>
    public class AOTGenericReferences : MonoBehaviour
    {
        public void Ref_ECS_Generics()
        {
            EntityManager em = default;
            Entity e = default;

            // ★★★ 核心修复：显式调用热更中会用到的泛型方法 ★★★

            // 1. 修复当前的 GetComponentData<PlacementStateComponent> 报错
            em.GetComponentData<PlacementStateComponent>(e);
            em.SetComponentData(e, new PlacementStateComponent());

            // 2. 预防性修复：其他可能在热更 UI 中用到的组件操作
            // 如果你的 UI 还会读写其他组件，最好也在这里加一下
            em.GetComponentData<GridConfigComponent>(e);

            // 如果用了 Singleton 访问，也要加
            // SystemAPI.GetSingleton<PlacementStateComponent>(); // 这个需要静态方法上下文，可以用下面这个替代：
            var q = em.CreateEntityQuery(typeof(PlacementStateComponent));
            q.GetSingleton<PlacementStateComponent>();
            q.GetSingletonRW<PlacementStateComponent>();
        }
    }
}