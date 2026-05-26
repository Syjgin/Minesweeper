using Components;
using Leopotam.EcsLite;
using Pools;
using UnityEngine;

namespace Systems.UI
{
    public static class WindowUtils
    {
        public static T GetWindow<T>(PoolSet poolSet, WindowType windowType, EcsFilter windowFilter, EcsPool<WindowComponent> windowPool) where T : Behaviour
        {
            poolSet.TryGetPool<T>(out var windowObjectPool);
            foreach (var entity in windowFilter)
            {
                ref var window = ref windowPool.Get(entity);
                if(window.WindowType != windowType)
                    continue;
                windowObjectPool.TryGetObjectByEntity(entity, out var windowObject);
                return windowObject;
            }
            return null;
        }
    }
}