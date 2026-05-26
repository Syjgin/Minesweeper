using Bootstrap;
using Components;
using Config;
using Events;
using Leopotam.EcsLite;
using Pools;
using SevenBoldPencil.EasyEvents;
using UI;
using UnityEngine;
using View;

namespace Systems
{
    public class RestartGameSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _world;
        private EventsBus _eventsBus;
        private PoolSet _poolSet;
        private EcsPool<CellCoordinateComponent> _ecsCellPool;
        private EcsPool<CellVisualStateComponent> _ecsCellVisualStatePool;
        private EcsPool<CameraComponent> _ecsCameraPool;
        private EcsPool<FieldComponent> _fieldPool;
        private EcsPool<DirtyComponent> _dirtyPool;
        private EcsPool<SavedParamsComponent> _ecsCharacteristicPool;
        private EcsFilter _cameraFilter;
        private EcsFilter _cellsFilter;
        private EcsFilter _fieldFilter;
        private EcsFilter _currentGameCharacteristicsFilter;
        private SharedData _sharedData;
        private EcsFilter _gameStartedFilter;
        private EcsPool<FreeFlagsComponent> _flagsPool;
        private EcsFilter _flagsFilter;
        private EcsPool<TimerComponent> _timerPool;
        private EcsFilter _timeFilter;
        private EcsFilter _mainUiFilter;
        
        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            _sharedData = systems.GetShared<SharedData>();
            _eventsBus = _sharedData.EventsBus;
            _ecsCellPool = _world.GetPool<CellCoordinateComponent>();
            _ecsCameraPool = _world.GetPool<CameraComponent>();
            _fieldPool = _world.GetPool<FieldComponent>();
            _dirtyPool = _world.GetPool<DirtyComponent>();
            _poolSet = systems.GetShared<SharedData>().PoolSet;
            _cameraFilter = _world.Filter<CameraComponent>().End();
            _cellsFilter = _world.Filter<CellCoordinateComponent>().End();
            _fieldFilter = _world.Filter<FieldComponent>().End();
            _currentGameCharacteristicsFilter = _world.Filter<SavedParamsComponent>().End();
            _ecsCharacteristicPool = _world.GetPool<SavedParamsComponent>();
            _ecsCellVisualStatePool = _world.GetPool<CellVisualStateComponent>();
            _gameStartedFilter = _world.Filter<GameStartedComponent>().End();
            _flagsPool = _world.GetPool<FreeFlagsComponent>();
            _flagsFilter = _world.Filter<FreeFlagsComponent>().End();
            _timeFilter = _world.Filter<TimerComponent>().End();
            _timerPool = _world.GetPool<TimerComponent>();
            _mainUiFilter = _world.Filter<MainUiComponent>().End();
        }

        public void Run(IEcsSystems systems)
        {
            if(!_eventsBus.HasEventSingleton<StartNewGameEvent>(out var startNewGameEvent))
                return;
            if (_currentGameCharacteristicsFilter.GetEntitiesCount() == 0)
            {
                var characteristicsEntity = _world.NewEntity();
                _ecsCharacteristicPool.Add(characteristicsEntity);
            }

            foreach (var entity in _currentGameCharacteristicsFilter)
            {
                ref var characteristics = ref _ecsCharacteristicPool.Get(entity);
                characteristics.Init(startNewGameEvent.GridSize, startNewGameEvent.MinesCount);
                break;
            }

            HandleFlags(startNewGameEvent);
            RecreateTimer();
            var mainCamera = MoveCameraToInitialPosition(startNewGameEvent.GridSize);
            FillField(startNewGameEvent.GridSize, startNewGameEvent.MinesCount, mainCamera);
        }

        private void RecreateTimer()
        {
            foreach (var entity in _timeFilter)
            {
                _world.DelEntity(entity);       
            }
            var timerEntity = _world.NewEntity();
            _timerPool.Add(timerEntity);
            if (!_poolSet.TryGetPool<MainUi>(out var mainUiPool))
                return;
            if (!mainUiPool.TryGetObjectByEntity(_mainUiFilter.GetRawEntities()[0], out var mainUi))
                return;
            mainUi.Timer.text = "";
        }

        private void HandleFlags(StartNewGameEvent startNewGameEvent)
        {
            if (_flagsFilter.GetEntitiesCount() == 0)
            {
                var flagsEntity = _world.NewEntity();
                _flagsPool.Add(flagsEntity).SetAmount(startNewGameEvent.MinesCount);
                _dirtyPool.Add(flagsEntity);
            }
            else
            {
                foreach (var entity in _flagsFilter)
                {
                    ref var flagsAmount = ref _flagsPool.Get(entity);
                    flagsAmount.SetAmount(startNewGameEvent.MinesCount);
                    _dirtyPool.Add(entity);
                    break;
                }   
            }
        }

        private void FillField(int gridSize, int minesCountNewValue, UnityEngine.Camera camera)
        {
            if(!_poolSet.TryGetPool<FieldView>(out var fieldObjectPool))
                return;
            if(!_poolSet.TryGetPool<CellView>(out var cellViewObjectPool))
                return;
            foreach (var entity in _gameStartedFilter)
            {
                _world.DelEntity(entity);
            }
            foreach (var cellEntity in _cellsFilter)
            {
                cellViewObjectPool.ReturnObject(cellEntity);
                _world.DelEntity(cellEntity);
            }
            foreach (var oldField in _fieldFilter)
            {
                if (fieldObjectPool.TryGetObjectByEntity(oldField, out var oldFieldObject))
                {
                    oldFieldObject.OnDragAction -= OnDragField;
                    oldFieldObject.OnCellClickAction -= OnCellClick;
                }
                fieldObjectPool.ReturnObject(oldField);
                _world.DelEntity(oldField);
            }
            var fieldOffset = CalculateFieldOffset(gridSize);
            
            var fieldEntity = _world.NewEntity();
            _fieldPool.Add(fieldEntity).Init(minesCountNewValue, fieldOffset);
            var field = fieldObjectPool.CreateObject(fieldEntity);
            field.OnDragAction += OnDragField;
            field.OnCellClickAction += OnCellClick;
            
            field.Init(gridSize, Constants.CellSize, fieldOffset, camera);
            
            for (var i = 0; i < gridSize; i++)
            {
                for (var j = 0; j < gridSize; j++)
                {
                    var cellEntity = _world.NewEntity();
                    _ecsCellPool.Add(cellEntity).Init(new Vector2Int(i, j));
                    _ecsCellVisualStatePool.Add(cellEntity).UpdateVisual(CellVisual.Closed);
                    _dirtyPool.Add(cellEntity);
                    var cellObject = cellViewObjectPool.CreateObject(cellEntity);
                    field.AddCell(cellObject);
                }
            }
        }

        private void OnCellClick(MouseClickData mouseClickData)
        {
            _eventsBus.NewEventSingleton<CellClickedEvent>() = new CellClickedEvent(mouseClickData);
        }

        private void OnDragField(Vector2 drag)
        {
            _eventsBus.NewEventSingleton<FieldDragEvent>() = new FieldDragEvent(drag);
        }

        private UnityEngine.Camera MoveCameraToInitialPosition(int gridSize)
        {
            var orthoSize = gridSize * Constants.CellSize / 2f;
            foreach (var entity in _cameraFilter)
            {
                ref var camera = ref _ecsCameraPool.Get(entity);
                camera.Init(Vector2.zero, orthoSize);
                _dirtyPool.Add(entity);
                if (_poolSet.TryGetPool<MainCamera>(out var cameraObjectPool) &&
                    cameraObjectPool.TryGetObjectByEntity(entity, out var cameraObject))
                {
                    return cameraObject.Camera;
                }
                break;
            }
            return null;
        }

        private float CalculateFieldOffset(int gridSize)
        {
            var additionalCoef = gridSize % 2 == 0 ? -0.5f : 0f;
            var multiplier = Mathf.FloorToInt(gridSize * 0.5f) + additionalCoef;
            return multiplier;
        }
    }
}