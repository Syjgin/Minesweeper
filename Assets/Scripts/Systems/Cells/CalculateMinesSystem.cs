using Components;
using Events;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.Pool;

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
        }

        public void Run(IEcsSystems systems)
        {
            if(_calculateMinesFilter.GetEntitiesCount() == 0)
                return;
            if(!EventsBus.HasEventSingleton<CellClickedEvent>(out var clickData))
                return;
            PlaceMines();
            HandleOrdinalClick(clickData.MouseClickData);
        }

        private void PlaceMines()
        {
            var mineNeighboursDictionary = DictionaryPool<Vector2Int, int>.Get();
            var neighbourCoordinates = HashSetPool<Vector2Int>.Get();
            var totalCells = _calculateMinesFilter.GetEntitiesCount() + 1;
            var minePossibility = 0f;
            var remainMines = 0;
            var gridSize = 0;
            foreach (var entity in _savedParamsFilter)
            {
                ref var savedParams = ref _savedParamsPool.Get(entity);
                minePossibility = (float)savedParams.MinesCount / totalCells;
                remainMines = savedParams.MinesCount;
                gridSize = savedParams.GridSize;
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
                    CoordinateUtils.FillNeighbourCoordinates(coords.Coordinates, neighbourCoordinates, gridSize);
                    foreach (var neighbourCoordinate in neighbourCoordinates)
                    {
                        if (!mineNeighboursDictionary.TryAdd(neighbourCoordinate, 1))
                        {
                            mineNeighboursDictionary[neighbourCoordinate]++;
                        }
                    }
                    remainMines--;
                }   
            }

            foreach (var entity in CellsFilter)
            {
                ref var coords = ref CoordsPool.Get(entity);
                ref var visual = ref StatesPool.Get(entity);
                if (mineNeighboursDictionary.TryGetValue(coords.Coordinates, out var amount))
                {
                    visual.UpdateNearMinesCount(amount);
                }
            }
            
            DictionaryPool<Vector2Int, int>.Release(mineNeighboursDictionary);
            HashSetPool<Vector2Int>.Release(neighbourCoordinates);
            foreach (var entity in _calculateMinesFilter)
            {
                CalculateMinesPool.Del(entity);
            }
        }
    }
}