using System;
using Config;
using Events;
using Leopotam.EcsLite;
using Pools;
using SevenBoldPencil.EasyEvents;
using Systems;
using Systems.Camera;
using Systems.Cells;
using Systems.UI;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using View;

namespace Bootstrap
{
    public sealed class EcsStartup : MonoBehaviour
    {
        [SerializeField] private PrefabLocator _prefabLocator;
        [SerializeField] private PlayerInput _playerInput;
        [SerializeField] private GameObject _uiRoot;
        [SerializeField] private InitialData _initialData;

        private EcsWorld _world;
        private IEcsSystems _updateRunSystems;

        private void Start()
        {
            _prefabLocator.InitializeCache();
            _world = new EcsWorld();

            var eventsBus = new EventsBus();

            var poolSet = CreatePrefabPoolSet();
            var sharedData = new SharedData(eventsBus, poolSet, _playerInput,
                new StartNewGameEvent(_initialData.GridSize, _initialData.MinesCount),
                new ReadOnlySettings(_initialData.MinGridSize, _initialData.MaxGridSize,
                    _initialData.ScrollWheelSensibility, _initialData.DragSensibility));
            _updateRunSystems = new EcsSystems(_world, sharedData);
            _updateRunSystems.Add(new LoadInitialDataSystem())
                .Add(new RestartGameSystem())
                .Add(new CameraDragSystem())
                .Add(new CameraZoomSystem())
                .Add(new UpdateCellVisualSystem())
                .Add(new ApplyMoveCameraSystem())
                .Add(new WindowStateChangeSystem())
                .Add(new MainUiInputSystem())
                .Add(new NewGameWindowInputSystem())
                .Add(new PauseWindowInputSystem())
                .Add(new CellClickHandleSystem())
#if UNITY_EDITOR
                .Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem())
#endif
                .Add(eventsBus.GetDestroyEventsSystem()
                    .IncSingleton<StartNewGameEvent>()
                    .IncSingleton<FieldDragEvent>()
                    .IncSingleton<CellClickedEvent>()
                    .IncReplicant<WindowStateChangeRequest>()
                    .IncSingleton<PauseRequest>()
                )
                .Init();
        }

        private PoolSet CreatePrefabPoolSet()
        {
            var poolSet = new PoolSet();

            poolSet.RegisterPool(new ObjectPool<CellView>(_prefabLocator, PrefabType.Cell, 512));
            poolSet.RegisterPool(new ObjectPool<NewGameWindow>(_prefabLocator, PrefabType.NewGameWindow, 1, _uiRoot.transform));
            poolSet.RegisterPool(new ObjectPool<PauseWindow>(_prefabLocator, PrefabType.PauseWindow, 1, _uiRoot.transform));
            poolSet.RegisterPool(new ObjectPool<MainUi>(_prefabLocator, PrefabType.MainUi, 1, _uiRoot.transform));
            poolSet.RegisterPool(new ObjectPool<FieldView>(_prefabLocator, PrefabType.Field, 1));
            poolSet.RegisterPool(new ObjectPool<MainCamera>(_prefabLocator, PrefabType.Camera, 1));
            return poolSet;
        }

        private void Update()
        {
            _updateRunSystems?.Run();
        }

        void OnDestroy()
        {
            if (_updateRunSystems != null)
            {
                _updateRunSystems.Destroy();
                _updateRunSystems = null;
            }

            if (_world != null)
            {
                _world.Destroy();
                _world = null;
            }
        }
    }
}