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
        private EcsPool<FieldCharacteristics> _mineCountPool;
        private EcsPool<Dirty> _dirtyPool;
        private EcsFilter _cameraFilter;
        private EcsFilter _fieldFilter;
        private EcsFilter _cellsFilter;
        private EcsFilter _mineCountFilter;
        private SharedData _sharedData;
        
        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            _sharedData = systems.GetShared<SharedData>();
            _eventsBus = _sharedData.EventsBus;
            _ecsCellPool = _world.GetPool<CellComponent>();
            _ecsFieldPool = _world.GetPool<FieldComponent>();
            _ecsCameraPool = _world.GetPool<CameraComponent>();
            _mineCountPool = _world.GetPool<FieldCharacteristics>();
            _dirtyPool = _world.GetPool<Dirty>();
            _poolSet = systems.GetShared<SharedData>().PoolSet;
            _cameraFilter = _world.Filter<CameraComponent>().End();
            _fieldFilter = _world.Filter<FieldComponent>().End();
            _cellsFilter = _world.Filter<CellComponent>().End();
            _mineCountFilter = _world.Filter<FieldCharacteristics>().End();
        }

        public void Run(IEcsSystems systems)
        {
            if(!_eventsBus.HasEventSingleton<StartNewGameEvent>(out var startNewGameEvent))
                return;
            var mainCamera = MoveCameraToInitialPosition();
            FillField(startNewGameEvent.GridSize, startNewGameEvent.MinesCount, mainCamera);
        }

        private void FillField(int gridSize, int minesCountNewValue, UnityEngine.Camera camera)
        {
            if(!_poolSet.TryGetPool<FieldView>(PrefabType.Field, out var fieldObjectPool))
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
            
            var cellSize = _sharedData.ReadOnlySettings.CellSize;
            var fieldOffset = CalculateFieldOffset(gridSize, cellSize);

            
            
            field.Init(gridSize, _sharedData.ReadOnlySettings.CellSize, fieldOffset.Item1, fieldOffset.Item2, camera);
            
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
                    mineCount.Init(minesCountNewValue, fieldOffset.Item1.x);
                    break;
                }
            }
            else
            {
                var mineCount = _world.NewEntity();
                _mineCountPool.Add(mineCount).Init(minesCountNewValue, fieldOffset.Item1.x);
            }
        }

        private UnityEngine.Camera MoveCameraToInitialPosition()
        {
            foreach (var entity in _cameraFilter)
            {
                ref var camera = ref _ecsCameraPool.Get(entity);
                camera.Init(Vector2.zero, _sharedData.ReadOnlySettings.InitialCameraOrthoSize);
                _dirtyPool.Add(entity);
                if (_poolSet.TryGetPool<MainCamera>(PrefabType.Camera, out var cameraObjectPool) &&
                    cameraObjectPool.TryGetObjectByEntity(entity, out var cameraObject))
                {
                    return cameraObject.Camera;
                }
                break;
            }
            return null;
        }

        private (Vector3, float) CalculateFieldOffset(int gridSize, float cellSize)
        {
            var additionalCoef = gridSize % 2 == 0 ? -0.5f : 0f;
            var multiplier = gridSize * 0.5f + additionalCoef;
            var totalSize = gridSize * cellSize;
            var halfSize = totalSize * multiplier;
            return (new Vector3(halfSize, halfSize, 0), multiplier);
        }
    }
}