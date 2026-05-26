using Bootstrap;
using Components;
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
        private EcsFilter _gameStartedFilter;
        private EcsWorld _world;
        
        public void Init(IEcsSystems systems)
        {
            _sharedData = systems.GetShared<SharedData>();
            _eventsBus = _sharedData.EventsBus;
            _poolSet = _sharedData.PoolSet;
            _world = systems.GetWorld();
            _windowFilter = _world.Filter<WindowComponent>().End();
            _windowPool = _world.GetPool<WindowComponent>();
            _gameStartedFilter = _world.Filter<GameStartedComponent>().End();
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
                BaseWindow targetWindow = null;
                var wasGameEnded = false;
                switch (eventBody.WindowType)
                {
                    case WindowType.NewGame:
                    {
                        targetWindow = TryGetWindow<NewGameWindow>(entity);
                    }
                        break;
                    case WindowType.Pause:
                    {
                        targetWindow = TryGetWindow<PauseWindow>(entity);
                    }
                        break;
                    case WindowType.GameOver:
                    {
                        targetWindow = TryGetWindow<GameOverWindow>(entity);
                        wasGameEnded = windowComponent.IsOpen;
                    }
                        break;
                    case WindowType.Win:
                    {
                        targetWindow = TryGetWindow<WinWindow>(entity);
                        wasGameEnded = windowComponent.IsOpen;
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
                if(wasGameEnded)
                    FinishGame();
            }
        }

        private void FinishGame()
        {
            foreach (var entity in _gameStartedFilter)
            {
                _world.DelEntity(entity);
            }
        }

        private T TryGetWindow<T>(int entity) where T : BaseWindow
        {
            _poolSet.TryGetPool<T>(out var pool);
            return pool.TryGetObjectByEntity(entity, out var rawWindow) ? rawWindow : null;
        }
    }
}