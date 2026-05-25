using Bootstrap;
using Components;
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
        
        public virtual void Init(IEcsSystems systems)
        {
            var sharedData = systems.GetShared<SharedData>();
            EventsBus = sharedData.EventsBus;
            World = systems.GetWorld();
            CoordsPool = World.GetPool<CellCoordinateComponent>();
            CellsFilter = World.Filter<CellCoordinateComponent>().End();
            StatesPool = World.GetPool<CellVisualStateComponent>();
            DirtyPool = World.GetPool<DirtyComponent>();
        }
        
        protected void HandleOrdinalClick(MouseClickData clickData)
        {
            
        }
    }
}