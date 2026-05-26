using Bootstrap;
using Components;
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
        private EcsPool<MainUiComponent> _ecsMainUiPool;

        public void Init(IEcsSystems systems)
        {
            _sharedData = systems.GetShared<SharedData>();
            var eventsBus = _sharedData.EventsBus;
            _world = systems.GetWorld();
            _ecsCameraPool = _world.GetPool<CameraComponent>();
            _ecsWindowPool = _world.GetPool<WindowComponent>();
            _ecsMainUiPool = _world.GetPool<MainUiComponent>();
            _poolSet = _sharedData.PoolSet;
            CreateCamera();
            CreateMainUI();
            CreateNewGameWindow();
            CreatePauseWindow();
            CreateWinWindow();
            CreateGameOverWindow();
            eventsBus.NewEvent<WindowStateChangeRequest>() =
                new WindowStateChangeRequest(WindowType.NewGame, true);
        }

        private void CreateGameOverWindow()
        {
            CreateWindow<GameOverWindow>(WindowType.GameOver);
        }

        private void CreateWinWindow()
        {
            CreateWindow<WinWindow>(WindowType.Win);
        }

        private void CreateCamera()
        {
            if (!_poolSet.TryGetPool<MainCamera>(out var cameraPool))
                return;
            var cameraEntity = _world.NewEntity();
            _ecsCameraPool.Add(cameraEntity);
            var mainCamera = cameraPool.CreateObject(cameraEntity);
            _sharedData.PlayerInput.camera = mainCamera.Camera;
        }

        private void CreateNewGameWindow()
        {
            var window = CreateWindow<NewGameWindow>(WindowType.NewGame);
            window.LevelSizeInput.text = _sharedData.InitialData.GridSize.ToString();
            window.MinesCountInput.text = _sharedData.InitialData.MinesCount.ToString();
        }

        private void CreatePauseWindow()
        {
            CreateWindow<PauseWindow>(WindowType.Pause);
        }

        private T CreateWindow<T>(WindowType windowType) where T : BaseWindow
        {
            var window = _world.NewEntity();
            _ecsWindowPool.Add(window).SetWindowType(windowType);
            return !_poolSet.TryGetPool<T>(out var newGameWindowPool)
                ? null
                : newGameWindowPool.CreateObject(window);
        }

        private void CreateMainUI()
        {
            var mainUiEntity = _world.NewEntity();
            if (!_poolSet.TryGetPool<MainUi>(out var mainUiPool))
                return;
            _ecsMainUiPool.Add(mainUiEntity);
            mainUiPool.CreateObject(mainUiEntity);
        }
    }
}