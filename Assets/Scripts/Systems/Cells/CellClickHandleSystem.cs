using Bootstrap;
using Components;
using Events;
using Leopotam.EcsLite;
using SevenBoldPencil.EasyEvents;
using UnityEngine;

namespace Systems.Cells
{
    public class CellClickHandleSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsFilter _gameStartedFilter;
        private EcsPool<GameStartedComponent> _gameStartedPool;
        private EventsBus _eventsBus;
        private EcsFilter _cellsFilter;
        private EcsPool<CellCoordinateComponent> _coordsPool;
        private EcsPool<CellVisualStateComponent> _statesPool;
        private EcsPool<DirtyComponent> _dirtyPool;
        private EcsPool<CalculateMinesComponent> _calculateMinesPool;
        private EcsWorld _world;
        
        public void Init(IEcsSystems systems)
        {
            var sharedData = systems.GetShared<SharedData>();
            _eventsBus = sharedData.EventsBus;
            _world = systems.GetWorld();
            _gameStartedFilter = _world.Filter<GameStartedComponent>().End();
            _coordsPool = _world.GetPool<CellCoordinateComponent>();
            _cellsFilter = _world.Filter<CellCoordinateComponent>().End();
            _statesPool = _world.GetPool<CellVisualStateComponent>();
            _dirtyPool = _world.GetPool<DirtyComponent>();
            _calculateMinesPool = _world.GetPool<CalculateMinesComponent>();
            _gameStartedPool = _world.GetPool<GameStartedComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            if(!_eventsBus.HasEventSingleton<CellClickedEvent>(out var clickData))
                return;
            if (_gameStartedFilter.GetEntitiesCount() == 0)
            {
                HandleFirstClick(clickData.CellIndex);
            }
            else
            {
                HandleOrdinalClick(clickData.CellIndex);
            }
        }

        private void HandleOrdinalClick(Vector2Int cellIndex)
        {
            
        }

        private void HandleFirstClick(Vector2Int cellIndex)
        {
            var gameStartedEntity = _world.NewEntity();
            _gameStartedPool.Add(gameStartedEntity);
            foreach (var entity in _cellsFilter)
            {
                ref var cell = ref _coordsPool.Get(entity);
                if (cell.X == cellIndex.x && cell.Y == cellIndex.y)
                {
                    ref var state = ref _statesPool.Get(entity);
                    state.UpdateVisual(CellVisual.Opened);
                    _dirtyPool.Add(entity);
                }
                else
                {
                    _calculateMinesPool.Add(entity);
                }
            }
        }
    }
}