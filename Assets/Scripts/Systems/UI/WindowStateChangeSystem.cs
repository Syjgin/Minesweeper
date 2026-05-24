using Bootstrap;
using Components;
using Config;
using Events;
using Leopotam.EcsLite;
using Pools;
using SevenBoldPencil.EasyEvents;
using UI;

namespace Systems.UI
{
    public class WindowStateChangeSystem : IEcsInitSystem, IEcsRunSystem
    {
        private SharedData _sharedData;
        private EventsBus _eventsBus;
        private PoolSet _poolSet;
        private EcsFilter _windowFilter;
        private EcsPool<WindowComponent> _windowPool;
        
        public void Init(IEcsSystems systems)
        {
            _sharedData = systems.GetShared<SharedData>();
            _eventsBus = _sharedData.EventsBus;
            _poolSet = _sharedData.PoolSet;
            var world = systems.GetWorld();
            _windowFilter = world.Filter<WindowComponent>().End();
            _windowPool = world.GetPool<WindowComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            if(!_eventsBus.HasEvents<WindowStateChangeRequest>())
                return;
            foreach (var eventEntity in _eventsBus.GetEventBodies<WindowStateChangeRequest>(out var eventPool))
            {
                ref var eventBody = ref eventPool.Get(eventEntity);
                HandleStateChangeEvent(ref eventBody);
            }
        }

        private void HandleStateChangeEvent(ref WindowStateChangeRequest eventBody)
        {
            foreach (var entity in _windowFilter)
            {
                ref var windowComponent = ref _windowPool.Get(entity);
                if(windowComponent.WindowType != eventBody.WindowType)
                    continue;
                if(windowComponent.IsOpen == eventBody.IsOpen)
                    continue;
                windowComponent.SetOpened(eventBody.IsOpen);
                var prefabType = WindowUtils.WindowTypeToPrefabType(eventBody.WindowType);
                BaseWindow targetWindow = null;
                switch (prefabType)
                {
                    case PrefabType.NewGameWindow:
                    {
                        targetWindow = TryGetWindow<NewGameWindow>(PrefabType.NewGameWindow, entity);
                    }
                        break;
                    case PrefabType.PauseWindow:
                    {
                        targetWindow = TryGetWindow<PauseWindow>(PrefabType.PauseWindow, entity);
                    }
                        break;
                    default:
                        break;
                }
                if(!targetWindow)
                    return;
                if (windowComponent.IsOpen)
                {
                    targetWindow.Show();
                }
                else
                {
                    targetWindow.Hide();
                }
            }
        }

        private T TryGetWindow<T>(PrefabType prefabType, int entity) where T : BaseWindow
        {
            _poolSet.TryGetPool<T>(prefabType, out var pool);
            return pool.TryGetObjectByEntity(entity, out var rawWindow) ? rawWindow : null;
        }
    }
}