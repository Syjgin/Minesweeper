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
    public class MainUiInputSystem : IEcsInitSystem, IEcsDestroySystem
    {
        private ObjectPool<MainUi> _objectPool;
        private EcsFilter _uiFilter;
        private EventsBus _eventsBus;
        private EcsFilter _paramsFilter;
        private EcsPool<SavedParamsComponent>  _paramsPool;
        private EcsFilter _gameStartedFilter;
        private EcsWorld _ecsWorld;
        
        public void Init(IEcsSystems systems)
        {
            _ecsWorld = systems.GetWorld();
            _uiFilter = _ecsWorld.Filter<MainUiComponent>().End();
            var sharedData = systems.GetShared<SharedData>();
            var poolSet = sharedData.PoolSet;
            _eventsBus = sharedData.EventsBus;
            _paramsPool = _ecsWorld.GetPool<SavedParamsComponent>();
            _paramsFilter = _ecsWorld.Filter<SavedParamsComponent>().End();
            _gameStartedFilter = _ecsWorld.Filter<GameStartedComponent>().End();
            
            if(!poolSet.TryGetPool(PrefabType.MainUi, out _objectPool))
                return;
            foreach (var entity in _uiFilter)
            {
                if(!_objectPool.TryGetObjectByEntity(entity, out var mainUi))
                    return;
                mainUi.PauseButton.onClick.AddListener(OnPauseClick);
                mainUi.RestartButton.onClick.AddListener(OnRestartClick);
            }
        }

        private void OnRestartClick()
        {
            RestartUtils.RestartCurrentGame(_paramsFilter, _paramsPool, _eventsBus);
        }

        private void OnPauseClick()
        {
            _eventsBus.NewEvent<WindowStateChangeRequest>() =
                new WindowStateChangeRequest(WindowType.Pause, true);
            foreach (var entity in _gameStartedFilter)
            {
                _ecsWorld.DelEntity(entity);
            }
        }

        public void Destroy(IEcsSystems systems)
        {
            foreach (var entity in _uiFilter)
            {
                if(!_objectPool.TryGetObjectByEntity(entity, out var mainUi))
                    return;
                mainUi.PauseButton.onClick.RemoveAllListeners();
                mainUi.RestartButton.onClick.RemoveAllListeners();
            }
        }
    }
}