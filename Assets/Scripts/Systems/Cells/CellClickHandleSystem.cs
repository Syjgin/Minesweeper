using Components;
using Events;
using Leopotam.EcsLite;
using View;

namespace Systems.Cells
{
    public class CellClickHandleSystem : BaseCellClickSystem, IEcsRunSystem
    {
        private EcsFilter _windowStates;
        private EcsPool<WindowComponent> _windowComponentPool;

        public override void Init(IEcsSystems systems)
        {
            base.Init(systems);
            _windowComponentPool = World.GetPool<WindowComponent>();
            _windowStates = World.Filter<WindowComponent>().End();
        }

        public void Run(IEcsSystems systems)
        {
            if(!EventsBus.HasEventSingleton<CellClickedEvent>(out var clickData))
                return;
            foreach (var windowState in _windowStates)
            {
                ref var window = ref _windowComponentPool.Get(windowState);
                if(!window.IsOpen)
                    continue;
                if(window.WindowType is WindowType.GameOver or WindowType.Win)
                    return;
            }
            if (GameStartedFilter.GetEntitiesCount() == 0)
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
            GameStartedPool.Add(gameStartedEntity);
            foreach (var entity in CellsFilter)
            {
                ref var cell = ref CoordsPool.Get(entity);
                if (cell.Coordinates.x != clickData.Position.x || cell.Coordinates.y != clickData.Position.y)
                {
                    CalculateMinesPool.Add(entity);
                }
            }
        }
    }
}