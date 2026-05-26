using Bootstrap;
using Components;
using Leopotam.EcsLite;
using Pools;
using UI;

namespace Systems.UI
{
    public class UpdateFlagsCountSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsFilter _flagsFilter;
        private EcsPool<FreeFlagsComponent> _flagsPool;
        private EcsPool<DirtyComponent> _dirtyPool;
        private PoolSet _poolSet;
        private EcsFilter _mainUiFilter;
        
        public void Init(IEcsSystems systems)
        {
            _poolSet = systems.GetShared<SharedData>().PoolSet;
            var world = systems.GetWorld();
            _flagsFilter = world.Filter<FreeFlagsComponent>().Inc<DirtyComponent>().End();
            _flagsPool = world.GetPool<FreeFlagsComponent>();
            _dirtyPool = world.GetPool<DirtyComponent>();
            _mainUiFilter = world.Filter<MainUiComponent>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _flagsFilter)
            {
                ref var freeFlagsComponent = ref _flagsPool.Get(entity);
                
                if (!_poolSet.TryGetPool<MainUi>(out var mainUiPool))
                    return;
                
                if (!mainUiPool.TryGetObjectByEntity(_mainUiFilter.GetRawEntities()[0], out var mainUi))
                    continue;
                
                mainUi.FlagsCount.text = freeFlagsComponent.Amount.ToString();
                _dirtyPool.Del(entity);
            }
        }
    }
}
