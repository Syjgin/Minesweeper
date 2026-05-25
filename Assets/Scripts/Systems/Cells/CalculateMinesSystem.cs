using Components;
using Events;
using Leopotam.EcsLite;
using UnityEngine;

namespace Systems.Cells
{
    public class CalculateMinesSystem : BaseCellClickSystem, IEcsRunSystem
    {
        private EcsFilter _calculateMinesFilter;
        private EcsFilter _savedParamsFilter;
        private EcsPool<SavedParamsComponent> _savedParamsPool;
        
        public override void Init(IEcsSystems systems)
        {
            base.Init(systems);
            _calculateMinesFilter = World.Filter<CalculateMinesComponent>().End();
            _savedParamsFilter = World.Filter<SavedParamsComponent>().End();
            _savedParamsPool = World.GetPool<SavedParamsComponent>();
            MinesPool = World.GetPool<MineComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            if(_calculateMinesFilter.GetEntitiesCount() == 0)
                return;
            if(!EventsBus.HasEventSingleton<CellClickedEvent>(out var clickData))
                return;
            var totalCells = _calculateMinesFilter.GetEntitiesCount() + 1;
            var minePossibility = 0f;
            var remainMines = 0;
            foreach (var entity in _savedParamsFilter)
            {
                ref var savedParams = ref _savedParamsPool.Get(entity);
                minePossibility = (float)savedParams.MinesCount / totalCells;
                remainMines = savedParams.MinesCount;
            }

            while (remainMines > 0)
            {
                foreach (var entity in _calculateMinesFilter)
                {
                    if(remainMines == 0)
                        break;
                    if(!CalculateMinesPool.Has(entity))
                        continue;
                    if(MinesPool.Has(entity))
                        continue;
                    var isMine = Random.Range(0f, 1f) <= minePossibility;
                    if (!isMine) 
                        continue;
                    MinesPool.Add(entity);
                    ref var coords = ref CoordsPool.Get(entity);
                    Debug.Log($"Mine: {coords.X}, {coords.Y}");
                    remainMines--;
                }   
            }

            foreach (var entity in _calculateMinesFilter)
            {
                CalculateMinesPool.Del(entity);
            }
            
            HandleOrdinalClick(clickData.MouseClickData);
        }
    }
}