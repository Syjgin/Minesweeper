using Components;
using Events;
using Leopotam.EcsLite;
using View;

namespace Systems.Cells
{
    public class CellClickHandleSystem : BaseCellClickSystem, IEcsRunSystem
    {
        private EcsFilter _gameStartedFilter;
        private EcsPool<GameStartedComponent> _gameStartedPool;
        private EcsPool<CalculateMinesComponent> _calculateMinesPool;
        
        public override void Init(IEcsSystems systems)
        {
            base.Init(systems);
            _gameStartedFilter = World.Filter<GameStartedComponent>().End();
            _calculateMinesPool = World.GetPool<CalculateMinesComponent>();
            _gameStartedPool = World.GetPool<GameStartedComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            if(!EventsBus.HasEventSingleton<CellClickedEvent>(out var clickData))
                return;
            if (_gameStartedFilter.GetEntitiesCount() == 0)
            {
                HandleFirstClick(clickData.MouseClickData);
            }
            else
            {
                HandleOrdinalClick(clickData.MouseClickData);
            }
        }

        private void HandleFirstClick(MouseClickData clickData)
        {
            var gameStartedEntity = World.NewEntity();
            _gameStartedPool.Add(gameStartedEntity);
            foreach (var entity in CellsFilter)
            {
                ref var cell = ref CoordsPool.Get(entity);
                if (cell.X == clickData.Position.x && cell.Y == clickData.Position.y)
                {
                    ref var state = ref StatesPool.Get(entity);
                    state.UpdateVisual(clickData.IsLeftButton ? CellVisual.Opened : CellVisual.Flag);
                    DirtyPool.Add(entity);
                }
                else
                {
                    _calculateMinesPool.Add(entity);
                }
            }
        }
    }
}