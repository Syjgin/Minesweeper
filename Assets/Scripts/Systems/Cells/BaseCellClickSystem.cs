using System;
using System.Collections.Generic;
using Bootstrap;
using Components;
using Events;
using Leopotam.EcsLite;
using SevenBoldPencil.EasyEvents;
using UnityEngine;
using UnityEngine.Pool;
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
        protected EcsFilter SavedParamsFilter;
        protected EcsPool<SavedParamsComponent> SavedParamsPool;
        
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
            SavedParamsFilter = World.Filter<SavedParamsComponent>().End();
            SavedParamsPool = World.GetPool<SavedParamsComponent>();
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
                var coordinatesCache = DictionaryPool<Vector2Int, int>.Get();
                
                var gridSize = 0;
                foreach (var entity in SavedParamsFilter)
                {
                    ref var savedParams = ref SavedParamsPool.Get(entity);
                    gridSize = savedParams.GridSize;
                    break;
                }
                
                foreach (var entity in CellsFilter)
                {
                    ref var cell = ref CoordsPool.Get(entity);
                    coordinatesCache[cell.Coordinates] = entity;
                    if (cell.Coordinates.x == clickData.Position.x && cell.Coordinates.y == clickData.Position.y)
                    {
                        ref var state = ref StatesPool.Get(entity);
                        if (MinesPool.Has(entity))
                        {
                            state.UpdateVisual(CellVisual.Mine);
                            wasGameOver = true;
                            DirtyPool.Add(entity); 
                            break;
                        }

                        state.UpdateVisual(CellVisual.Opened);
                        DirtyPool.Add(entity);
                    }
                }

                if (!wasGameOver)
                {
                    OpenAllNonMineNeighbors(clickData.Position, gridSize, coordinatesCache);
                }
                DictionaryPool<Vector2Int, int>.Release(coordinatesCache);
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
        
        private readonly Vector2Int[] _neighbourArray = new Vector2Int[8];
        private readonly List<Vector2Int> _snapshot = new List<Vector2Int>(8);
        
        private void OpenAllNonMineNeighbors(Vector2Int coordinates, int gridSize, Dictionary<Vector2Int, int> entitiesByCoordinates)
        {
            Span<Vector2Int> localSnapshot = stackalloc Vector2Int[8];
            var count = CoordinateUtils.FillNeighbourCoordinatesToArrayToSpan(coordinates, localSnapshot, gridSize);

            foreach (var coordinate in localSnapshot[..count])
            {
                var cellEntity = entitiesByCoordinates[coordinate];
                if (MinesPool.Has(cellEntity))
                    continue;

                ref var state = ref StatesPool.Get(cellEntity);
                if (state.Visual == CellVisual.Opened)
                    continue;

                state.UpdateVisual(CellVisual.Opened);
                DirtyPool.Add(cellEntity);

                if (state.NearMinesCount == 0)
                    OpenAllNonMineNeighbors(coordinate, gridSize, entitiesByCoordinates);
            }
        }
    }
}