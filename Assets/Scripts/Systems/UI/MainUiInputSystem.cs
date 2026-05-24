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
        private EcsFilter _characteristicsFilter;
        private EcsPool<CurrentGameCharacteristicsComponent>  _characteristicsPool; 
        
        public void Init(IEcsSystems systems)
        {
            var ecsWorld = systems.GetWorld();
            _uiFilter = ecsWorld.Filter<MainUiComponent>().End();
            var sharedData = systems.GetShared<SharedData>();
            var poolSet = sharedData.PoolSet;
            _eventsBus = sharedData.EventsBus;
            _characteristicsPool = ecsWorld.GetPool<CurrentGameCharacteristicsComponent>();
            _characteristicsFilter = ecsWorld.Filter<CurrentGameCharacteristicsComponent>().End();
            
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
            foreach (var entity in _characteristicsFilter)
            {
                ref var characteristics = ref _characteristicsPool.Get(entity);
                _eventsBus.NewEventSingleton<StartNewGameEvent>() = new StartNewGameEvent(characteristics.GridSize, characteristics.MinesCount);
            }
        }

        private void OnPauseClick()
        {
            _eventsBus.NewEventSingleton<WindowStateChangeRequest>() =
                new WindowStateChangeRequest(WindowType.Pause, true);
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