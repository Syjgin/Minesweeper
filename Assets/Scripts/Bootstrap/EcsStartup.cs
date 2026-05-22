using System;
using Config;
using Events;
using Leopotam.EcsLite;
using Pools;
using SevenBoldPencil.EasyEvents;
using Systems;
using Systems.Camera;
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
            var sharedData = new SharedData(eventsBus, poolSet, _playerInput, new StartNewGameEvent(_initialData.GridSize, _initialData.MinesCount), _initialData.InitialCameraHeight);
            _updateRunSystems = new EcsSystems(_world, sharedData);
            _updateRunSystems.Add(new LoadInitialDataSystem())
                .Add(new RestartGameSystem())
                .Add(new CameraDragSystem())
                .Add(new CameraZoomSystem())
                .Add(new ApplyMoveCameraSystem())
#if UNITY_EDITOR
                .Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem())
#endif
                .Add(eventsBus.GetDestroyEventsSystem()
                    .IncSingleton<StartNewGameEvent>()
                )
                .Init();
        }
        
        private PoolSet CreatePrefabPoolSet()
        {
            var poolSet = new PoolSet();
            var cellPool = new ObjectPool<CellView>(_prefabLocator.GetPrefabByType(PrefabType.Cell), 512);
            var createWindowPool = new ObjectPool<CreateLevelWindow>(_prefabLocator.GetPrefabByType(PrefabType.CreateWindow), 1);
            var pauseWindowPool = new ObjectPool<PauseWindow>(_prefabLocator.GetPrefabByType(PrefabType.PauseWindow), 1);
            var uiPool = new ObjectPool<MainUi>(_prefabLocator.GetPrefabByType(PrefabType.MainUi), 1, _uiRoot.transform);
            var fieldPool = new ObjectPool<Field>(_prefabLocator.GetPrefabByType(PrefabType.Field), 1);
            var cameraPool = new ObjectPool<MainCamera>(_prefabLocator.GetPrefabByType(PrefabType.Camera), 1);
            
            poolSet.RegisterPool(PrefabType.Cell, cellPool);
            poolSet.RegisterPool(PrefabType.CreateWindow, createWindowPool);
            poolSet.RegisterPool(PrefabType.PauseWindow, pauseWindowPool);
            poolSet.RegisterPool(PrefabType.MainUi, uiPool);
            poolSet.RegisterPool(PrefabType.Field, fieldPool);
            poolSet.RegisterPool(PrefabType.Camera, cameraPool);
            return poolSet;
        }
    }
}