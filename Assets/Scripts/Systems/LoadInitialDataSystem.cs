using Bootstrap;
using Components;
using Config;
using Events;
using Leopotam.EcsLite;
using Pools;
using UI;
using UnityEngine;
using View;

namespace Systems
{
    public class LoadInitialDataSystem : IEcsInitSystem
    {
        private EcsPool<CameraComponent> _ecsCameraPool;
        private PoolSet _poolSet;
        private EcsWorld _world;
        private SharedData _sharedData;
        private EcsPool<WindowComponent> _ecsWindowPool;

        public void Init(IEcsSystems systems)
        {
            _sharedData = systems.GetShared<SharedData>();
            var eventsBus = _sharedData.EventsBus;
            _world = systems.GetWorld();
            _ecsCameraPool = _world.GetPool<CameraComponent>();
            _ecsWindowPool = _world.GetPool<WindowComponent>();
            _poolSet = _sharedData.PoolSet;
            CreateCamera();
            CreateMainUI();
            CreateNewGameWindow();
            CreatePauseWindow();
            eventsBus.NewEventSingleton<WindowStateChangeRequest>() =
                new WindowStateChangeRequest(WindowType.NewGame, true);
        }

        private void CreateCamera()
        {
            if (!_poolSet.TryGetPool<MainCamera>(PrefabType.Camera, out var cameraPool))
                return;
            var cameraEntity = _world.NewEntity();
            _ecsCameraPool.Add(cameraEntity);
            var mainCamera = cameraPool.CreateObject(cameraEntity);
            _sharedData.PlayerInput.camera = mainCamera.Camera;
        }

        private void CreateNewGameWindow()
        {
            var window = CreateWindow<NewGameWindow>(PrefabType.NewGameWindow, WindowType.NewGame);
            window.LevelSizeInput.text = _sharedData.InitialData.GridSize.ToString();
            window.MinesCountInput.text = _sharedData.InitialData.MinesCount.ToString();
            var windows = _world.Filter<WindowComponent>().End();
            foreach (var window1 in windows)
            {
                ref var windowComponent = ref _ecsWindowPool.Get(window1);
                if (windowComponent.WindowType == WindowType.None)
                {
                    Debug.LogError($"Window has not been created");
                }
            }
        }

        private void CreatePauseWindow()
        {
            CreateWindow<PauseWindow>(PrefabType.PauseWindow, WindowType.Pause);
        }

        private T CreateWindow<T>(PrefabType prefabType, WindowType windowType) where T : BaseWindow
        {
            var window = _world.NewEntity();
            _ecsWindowPool.Add(window).SetWindowType(windowType);
            return !_poolSet.TryGetPool<T>(prefabType, out var newGameWindowPool)
                ? null
                : newGameWindowPool.CreateObject(window);
        }

        private void CreateMainUI()
        {
        }
    }
}