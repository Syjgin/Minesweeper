using Bootstrap;
using Components;
using Config;
using Leopotam.EcsLite;
using Pools;
using View;

namespace Systems.Cells
{
    public class UpdateCellVisualSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsFilter _cellsFilter;
        private EcsPool<CellVisualStateComponent> _cellsPool;
        private PoolSet _poolSet;
        private EcsPool<DirtyComponent> _dirtyPool;
        
        public void Init(IEcsSystems systems)
        {
            _poolSet = systems.GetShared<SharedData>().PoolSet;
            var world = systems.GetWorld();
            _cellsFilter = world.Filter<CellVisualStateComponent>().Inc<DirtyComponent>().End();
            _cellsPool = world.GetPool<CellVisualStateComponent>();
            _dirtyPool = world.GetPool<DirtyComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            if(!_poolSet.TryGetPool<CellView>(PrefabType.Cell, out var objectPool))
                return;
            foreach (var entity in _cellsFilter)
            {
                ref var cellVisual = ref _cellsPool.Get(entity);
                if(!objectPool.TryGetObjectByEntity(entity, out var view))
                    continue;
                switch (cellVisual.Visual)
                {
                    case CellVisual.Closed:
                        view.MinesNearCountText.text = "";
                        view.BackgroundImage.sprite = view.ClosedSprite;
                        break;
                    case CellVisual.Flag:
                        view.MinesNearCountText.text = "";
                        view.BackgroundImage.sprite = view.FlagSprite;
                        break;
                    case CellVisual.Opened:
                        view.MinesNearCountText.text = cellVisual.NearMinesCount > 0 ? cellVisual.NearMinesCount.ToString() : "";
                        view.BackgroundImage.sprite = view.OpenedSprite;
                        break;
                    case CellVisual.Mine:
                        view.MinesNearCountText.text = "";
                        view.BackgroundImage.sprite = view.MineSprite;
                        break;
                    default:
                        break;
                }
                _dirtyPool.Del(entity);
            }
        }
    }
}