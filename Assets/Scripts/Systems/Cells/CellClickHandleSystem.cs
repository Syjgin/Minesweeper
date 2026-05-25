using Components;
using Events;
using Leopotam.EcsLite;
using View;

namespace Systems.Cells
{
    public class CellClickHandleSystem : BaseCellClickSystem, IEcsRunSystem
    {

        public void Run(IEcsSystems systems)
        {
            if(!EventsBus.HasEventSingleton<CellClickedEvent>(out var clickData))
                return;
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