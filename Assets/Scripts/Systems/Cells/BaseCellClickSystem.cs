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
        protected EcsWorld World;
        protected EcsPool<MineComponent> MinesPool;
        protected EcsFilter GameStartedFilter;
        protected EcsPool<GameStartedComponent> GameStartedPool;
        protected EcsPool<CalculateMinesComponent> CalculateMinesPool;
        protected EcsFilter SavedParamsFilter;
        protected EcsPool<SavedParamsComponent> SavedParamsPool;
        private EcsFilter _minesFilter;
        private EcsFilter _nonMinesFilter;
        private EcsPool<DirtyComponent> _dirtyPool;
        private EcsPool<FreeFlagsComponent> _flagsPool;
        private EcsFilter _flagsFilter;

        public virtual void Init(IEcsSystems systems)
        {
            var sharedData = systems.GetShared<SharedData>();
            EventsBus = sharedData.EventsBus;
            World = systems.GetWorld();
            CoordsPool = World.GetPool<CellCoordinateComponent>();
            CellsFilter = World.Filter<CellCoordinateComponent>().End();
            StatesPool = World.GetPool<CellVisualStateComponent>();
            _dirtyPool = World.GetPool<DirtyComponent>();
            MinesPool = World.GetPool<MineComponent>();
            _minesFilter = World.Filter<MineComponent>().End();
            GameStartedFilter = World.Filter<GameStartedComponent>().End();
            GameStartedPool = World.GetPool<GameStartedComponent>();
            CalculateMinesPool = World.GetPool<CalculateMinesComponent>();
            SavedParamsFilter = World.Filter<SavedParamsComponent>().End();
            SavedParamsPool = World.GetPool<SavedParamsComponent>();
            _nonMinesFilter = World.Filter<CellVisualStateComponent>().Exc<MineComponent>().End();
            _flagsPool = World.GetPool<FreeFlagsComponent>();
            _flagsFilter = World.Filter<FreeFlagsComponent>().End();
        }

        protected void HandleOrdinalClick(MouseClickData clickData)
        {
            var wasGameOver = false;
            var isFlagMode = !clickData.IsLeftButton;
            if (isFlagMode)
            {
                HandleFlags(clickData);
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
                            _dirtyPool.Add(entity);
                            break;
                        }

                        state.UpdateVisual(CellVisual.Opened);
                        _dirtyPool.Add(entity);
                    }
                }

                if (!wasGameOver)
                {
                    OpenAllNonMineNeighbors(clickData.Position, gridSize, coordinatesCache);
                }

                DictionaryPool<Vector2Int, int>.Release(coordinatesCache);
            }

            if (wasGameOver)
            {
                Lose();
            }
            else
            {
                CheckIsWin();
            }
        }

        private void HandleFlags(MouseClickData clickData)
        {
            var wasFlagAdded = false;
            var wasFlagRemoved = false;
            foreach (var entity in CellsFilter)
            {
                ref var cell = ref CoordsPool.Get(entity);
                if (cell.Coordinates.x != clickData.Position.x || cell.Coordinates.y != clickData.Position.y)
                    continue;
                ref var state = ref StatesPool.Get(entity);
                if (state.Visual == CellVisual.Closed)
                {
                    ref var flag = ref _flagsPool.Get(_flagsFilter.GetRawEntities()[0]);
                    if (flag.Amount <= 0)
                        return;
                    state.UpdateVisual(CellVisual.Flag);
                    wasFlagRemoved = true;
                }
                else
                {
                    state.UpdateVisual(CellVisual.Closed);
                    wasFlagAdded = true;
                }

                _dirtyPool.Add(entity);
                break;
            }

            foreach (var entity in _flagsFilter)
            {
                ref var flag = ref _flagsPool.Get(entity);
                if (wasFlagAdded)
                {
                    flag.SetAmount(flag.Amount + 1);
                    _dirtyPool.Add(entity);
                }
                else if (wasFlagRemoved)
                {
                    flag.SetAmount(flag.Amount - 1);
                    _dirtyPool.Add(entity);
                }
            }
        }

        private void Lose()
        {
            foreach (var entity in GameStartedFilter)
            {
                GameStartedPool.Del(entity);
            }

            EventsBus.NewEvent<WindowStateChangeRequest>() =
                new WindowStateChangeRequest(WindowType.GameOver, true);
        }

        private void CheckIsWin()
        {
            var wasNonFlaggerMineFound = false;
            foreach (var entity in _minesFilter)
            {
                ref var cell = ref StatesPool.Get(entity);
                if (cell.Visual != CellVisual.Flag)
                {
                    wasNonFlaggerMineFound = true;
                    break;
                }
            }

            if (wasNonFlaggerMineFound)
                return;
            foreach (var entity in _nonMinesFilter)
            {
                ref var cell = ref StatesPool.Get(entity);
                if (cell.Visual != CellVisual.Opened)
                    return;
            }

            foreach (var entity in GameStartedFilter)
            {
                GameStartedPool.Del(entity);
            }

            EventsBus.NewEvent<WindowStateChangeRequest>() =
                new WindowStateChangeRequest(WindowType.Win, true);
        }

        private void OpenAllNonMineNeighbors(Vector2Int coordinates, int gridSize,
            Dictionary<Vector2Int, int> entitiesByCoordinates)
        {
            Span<Vector2Int> localSnapshot = stackalloc Vector2Int[8];
            var count = CoordinateUtils.FillNeighbourCoordinates(coordinates, localSnapshot, gridSize);

            foreach (var coordinate in localSnapshot[..count])
            {
                var cellEntity = entitiesByCoordinates[coordinate];
                if (MinesPool.Has(cellEntity))
                    continue;

                ref var state = ref StatesPool.Get(cellEntity);
                if (state.Visual == CellVisual.Opened)
                    continue;

                state.UpdateVisual(CellVisual.Opened);
                _dirtyPool.Add(cellEntity);

                if (state.NearMinesCount == 0)
                    OpenAllNonMineNeighbors(coordinate, gridSize, entitiesByCoordinates);
            }
        }
    }
}