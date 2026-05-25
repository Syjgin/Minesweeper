using Bootstrap;
using Components;
using Config;
using Leopotam.EcsLite;
using Pools;
using UI;
using UnityEngine;

namespace Systems.UI
{
    public class UpdateTimerSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsFilter _gameStartedFilter;
        private EcsFilter _timeFilter;
        private EcsPool<TimerComponent> _timerPool;
        private EcsFilter _mainUiFilter;
        private PoolSet _poolSet;
        
        public void Init(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            _gameStartedFilter = world.Filter<GameStartedComponent>().End();
            _timeFilter = world.Filter<TimerComponent>().End();
            _timerPool = world.GetPool<TimerComponent>();
            _mainUiFilter = world.Filter<MainUiComponent>().End();
            _poolSet = systems.GetShared<SharedData>().PoolSet;
        }

        public void Run(IEcsSystems systems)
        {
            if (_gameStartedFilter.GetEntitiesCount() <= 0)
                return;

            foreach (var entity in _timeFilter)
            {
                ref var timer = ref _timerPool.Get(entity);
                
                var previousWholeSeconds = timer.WholeSeconds;
                
                timer.AddDelta(Time.deltaTime);
                
                if (timer.WholeSeconds != previousWholeSeconds)
                {
                    if (!_poolSet.TryGetPool<MainUi>(PrefabType.MainUi, out var mainUiPool))
                        return;
                    
                    if (!mainUiPool.TryGetObjectByEntity(_mainUiFilter.GetRawEntities()[0], out var mainUi))
                        continue;
                    
                    mainUi.Timer.text = timer.WholeSeconds.ToString();
                }
                break;
            }
        }
    }
}
