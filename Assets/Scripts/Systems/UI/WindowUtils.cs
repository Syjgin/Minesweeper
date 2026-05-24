using Components;
using Config;
using Leopotam.EcsLite;
using Pools;
using UnityEngine;

namespace Systems.UI
{
    public static class WindowUtils
    {
        public static T GetWindow<T>(PoolSet poolSet, PrefabType prefabType, EcsFilter windowFilter, EcsPool<WindowComponent> windowPool) where T : Behaviour
        {
            poolSet.TryGetPool<T>(prefabType, out var windowObjectPool);
            var windowType = PrefabTypeToWindowType(prefabType);
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
        
        public static PrefabType WindowTypeToPrefabType(WindowType windowType)
        {
            return windowType switch
            {
                WindowType.NewGame => PrefabType.NewGameWindow,
                WindowType.Pause => PrefabType.PauseWindow,
                _ => PrefabType.None
            };
        }
        
        public static WindowType PrefabTypeToWindowType(PrefabType prefabType)
        {
            return prefabType switch
            {
                PrefabType.NewGameWindow => WindowType.NewGame,
                PrefabType.PauseWindow => WindowType.Pause,
                _ => WindowType.None
            };
        }
    }
}