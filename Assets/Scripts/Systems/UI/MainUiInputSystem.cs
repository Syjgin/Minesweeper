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
        
        public void Init(IEcsSystems systems)
        {
            var ecsWorld = systems.GetWorld();
            _uiFilter = ecsWorld.Filter<MainUiComponent>().End();
            var sharedData = systems.GetShared<SharedData>();
            var poolSet = sharedData.PoolSet;
            _eventsBus = sharedData.EventsBus;
            _paramsPool = ecsWorld.GetPool<SavedParamsComponent>();
            _paramsFilter = ecsWorld.Filter<SavedParamsComponent>().End();
            
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