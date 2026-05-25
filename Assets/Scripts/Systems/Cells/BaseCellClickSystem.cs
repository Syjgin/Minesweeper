using Bootstrap;
using Components;
using Events;
using Leopotam.EcsLite;
using SevenBoldPencil.EasyEvents;
using View;

namespace Systems.Cells
{
    public class BaseCellClickSystem : IEcsInitSystem
    {
        protected EventsBus EventsBus;
        protected EcsFilter CellsFilter;
        protected EcsPool<CellCoordinateComponent> CoordsPool;
        protected EcsPool<CellVisualStateComponent> StatesPool;
        protected EcsPool<DirtyComponent> DirtyPool;
        protected EcsWorld World;
        protected EcsPool<MineComponent> MinesPool;
        protected EcsFilter GameStartedFilter;
        protected EcsPool<GameStartedComponent> GameStartedPool;
        protected EcsPool<CalculateMinesComponent> CalculateMinesPool;
        
        public virtual void Init(IEcsSystems systems)
        {
            var sharedData = systems.GetShared<SharedData>();
            EventsBus = sharedData.EventsBus;
            World = systems.GetWorld();
            CoordsPool = World.GetPool<CellCoordinateComponent>();
            CellsFilter = World.Filter<CellCoordinateComponent>().End();
            StatesPool = World.GetPool<CellVisualStateComponent>();
            DirtyPool = World.GetPool<DirtyComponent>();
            MinesPool = World.GetPool<MineComponent>();
            GameStartedFilter = World.Filter<GameStartedComponent>().End();
            GameStartedPool = World.GetPool<GameStartedComponent>();
            CalculateMinesPool = World.GetPool<CalculateMinesComponent>();
        }
        
        protected void HandleOrdinalClick(MouseClickData clickData)
        {
            var wasGameOver = false;
            var isFlagMode = !clickData.IsLeftButton;
            if (isFlagMode)
            {
                foreach (var entity in CellsFilter)
                {
                    ref var cell = ref CoordsPool.Get(entity);
                    if (cell.Coordinates.x != clickData.Position.x || cell.Coordinates.y != clickData.Position.y) 
                        continue;
                    ref var state = ref StatesPool.Get(entity);
                    state.UpdateVisual(CellVisual.Flag);
                    DirtyPool.Add(entity);
                    break;
                }
    
            }
            else
            {
                foreach (var entity in CellsFilter)
                {
                    ref var cell = ref CoordsPool.Get(entity);
                    if (cell.Coordinates.x == clickData.Position.x && cell.Coordinates.y == clickData.Position.y)
                    {
                        ref var state = ref StatesPool.Get(entity);
                        if (MinesPool.Has(entity))
                        {
                            state.UpdateVisual(CellVisual.Mine);
                            wasGameOver = true;    
                        }
                        else
                        {
                            state.UpdateVisual(CellVisual.Opened);
                        }
                        DirtyPool.Add(entity);   
                    }
                }   
            }

            if (!wasGameOver) 
                return;
            foreach (var entity in GameStartedFilter)
            {
                GameStartedPool.Del(entity);
            }
            EventsBus.NewEvent<WindowStateChangeRequest>() =
                new WindowStateChangeRequest(WindowType.GameOver, true);
        }
    }
}