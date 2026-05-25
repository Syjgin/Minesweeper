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
    public class PauseWindowInputSystem : IEcsInitSystem, IEcsDestroySystem
    {
        private PoolSet _poolSet;
        private EcsFilter _windowFilter;
        private EcsPool<WindowComponent> _windowPool;
        private EventsBus _eventsBus;
        private EcsFilter _paramsFilter;
        private EcsPool<SavedParamsComponent>  _paramsPool;
        private EcsFilter _gameStartedFilter;
        private EcsWorld _world;
        
        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            _windowFilter = _world.Filter<WindowComponent>().End();
            _windowPool = _world.GetPool<WindowComponent>();
            var sharedData = systems.GetShared<SharedData>();
            _poolSet = sharedData.PoolSet;
            _eventsBus = sharedData.EventsBus;
            _paramsFilter = _world.Filter<SavedParamsComponent>().End();
            _paramsPool = _world.GetPool<SavedParamsComponent>();
            _gameStartedFilter = _world.Filter<GameStartedComponent>().End();
            
            var windowObject = GetWindow();
            windowObject.RestartButton.onClick.AddListener(OnRestartClick);
            windowObject.ContinueButton.onClick.AddListener(OnHideClick);
            windowObject.ExitButton.onClick.AddListener(OnStartNewGameClick);
        }

        private void OnStartNewGameClick()
        {
            _eventsBus.NewEvent<WindowStateChangeRequest>() = new WindowStateChangeRequest(WindowType.Pause, false);
            _eventsBus.NewEvent<WindowStateChangeRequest>() = new WindowStateChangeRequest(WindowType.NewGame, true);
            foreach (var entity in _gameStartedFilter)
            {
                _world.DelEntity(entity);
            }
        }

        private void OnHideClick()
        {
            _eventsBus.NewEvent<WindowStateChangeRequest>() = new WindowStateChangeRequest(WindowType.Pause, false);
        }

        private void OnRestartClick()
        {
            _eventsBus.NewEvent<WindowStateChangeRequest>() = new WindowStateChangeRequest(WindowType.Pause, false);
            RestartUtils.RestartCurrentGame(_paramsFilter, _paramsPool, _eventsBus);
        }

        public void Destroy(IEcsSystems systems)
        {
            var windowObject = GetWindow();
            windowObject.RestartButton.onClick.RemoveAllListeners();
            windowObject.ContinueButton.onClick.RemoveAllListeners();
            windowObject.ExitButton.onClick.RemoveAllListeners();
        }
        
        private PauseWindow GetWindow()
        {
            return WindowUtils.GetWindow<PauseWindow>(_poolSet, PrefabType.PauseWindow, _windowFilter, _windowPool);
        }
    }
}