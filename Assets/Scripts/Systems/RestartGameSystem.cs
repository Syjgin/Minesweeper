using Bootstrap;
using Components;
using Config;
using Events;
using Leopotam.EcsLite;
using Pools;
using SevenBoldPencil.EasyEvents;
using UnityEngine;
using View;

namespace Systems
{
    public class RestartGameSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _world;
        private EventsBus _eventsBus;
        private PoolSet _poolSet;
        private EcsPool<CellComponent> _ecsCellPool;
        private EcsPool<FieldComponent> _ecsFieldPool;
        private EcsPool<CameraComponent> _ecsCameraPool;
        private EcsPool<TotalMinesCount> _mineCountPool;
        private EcsPool<Dirty> _dirtyPool;
        private EcsFilter _cameraFilter;
        private EcsFilter _fieldFilter;
        private EcsFilter _cellsFilter;
        private EcsFilter _mineCountFilter;
        private float _initialCameraHeight;
        
        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            _eventsBus = systems.GetShared<SharedData>().EventsBus;
            _ecsCellPool = _world.GetPool<CellComponent>();
            _ecsFieldPool = _world.GetPool<FieldComponent>();
            _ecsCameraPool = _world.GetPool<CameraComponent>();
            _mineCountPool = _world.GetPool<TotalMinesCount>();
            _dirtyPool = _world.GetPool<Dirty>();
            _poolSet = systems.GetShared<SharedData>().PoolSet;
            _cameraFilter = _world.Filter<CameraComponent>().End();
            _fieldFilter = _world.Filter<FieldComponent>().End();
            _cellsFilter = _world.Filter<CellComponent>().End();
            _mineCountFilter = _world.Filter<TotalMinesCount>().End();
            _initialCameraHeight = systems.GetShared<SharedData>().InitialCameraHeight;
        }

        public void Run(IEcsSystems systems)
        {
            if(!_eventsBus.HasEventSingleton<StartNewGameEvent>(out var startNewGameEvent))
                return;
            MoveCameraToInitialPosition();
            FillField(startNewGameEvent.GridSize, startNewGameEvent.MinesCount);
        }

        private void FillField(int gridSize, int minesCountNewValue)
        {
            if(!_poolSet.TryGetPool<Field>(PrefabType.Field, out var fieldObjectPool))
                return;
            if(!_poolSet.TryGetPool<CellView>(PrefabType.Cell, out var cellViewObjectPool))
                return;
            foreach (var cellEntity in _cellsFilter)
            {
                cellViewObjectPool.ReturnObject(cellEntity);
                _ecsCellPool.Del(cellEntity);
            }
            foreach (var oldField in _fieldFilter)
            {
                fieldObjectPool.ReturnObject(oldField);
                _ecsFieldPool.Del(oldField);
            }
            var fieldEntity = _world.NewEntity();
            _ecsFieldPool.Add(fieldEntity);
            var field = fieldObjectPool.CreateObject(fieldEntity);
            field.transform.position = Vector3.zero;
            for (var i = 0; i < gridSize; i++)
            {
                for (var j = 0; j < gridSize; j++)
                {
                    var cellEntity = _world.NewEntity();
                    _ecsCellPool.Add(cellEntity).Init(i, j);
                    var cellObject = cellViewObjectPool.CreateObject(cellEntity);
                    field.AddCell(cellObject);
                }
            }

            if (_mineCountFilter.GetEntitiesCount() > 0)
            {
                foreach (var entity in _mineCountFilter)
                {
                    ref var mineCount = ref _mineCountPool.Get(entity);
                    mineCount.Init(minesCountNewValue);
                    break;
                }
            }
            else
            {
                var mineCount = _world.NewEntity();
                _mineCountPool.Add(mineCount).Init(minesCountNewValue);
            }
        }

        private void MoveCameraToInitialPosition()
        {
            foreach (var entity in _cameraFilter)
            {
                ref var camera = ref _ecsCameraPool.Get(entity);
                camera.Update(new Vector3(0, 0, _initialCameraHeight));
                _dirtyPool.Add(entity);
                break;
            }
        }
    }
}