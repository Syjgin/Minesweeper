using Bootstrap;
using Components;
using Config;
using Events;
using Leopotam.EcsLite;
using Pools;
using SevenBoldPencil.EasyEvents;
using UI;
using UnityEngine;

namespace Systems.UI
{
    public class NewGameWindowInputSystem : IEcsInitSystem, IEcsDestroySystem
    {
        private const int MinMinesCount = 1;
        private PoolSet _poolSet;
        private EcsFilter _windowFilter;
        private EcsPool<WindowComponent> _windowPool;
        private EventsBus _eventsBus;
        private ReadOnlySettings _readOnlySettings;
        
        public void Init(IEcsSystems systems)
        {
            var ecsWorld = systems.GetWorld();
            _windowFilter = ecsWorld.Filter<WindowComponent>().End();
            _windowPool = ecsWorld.GetPool<WindowComponent>();
            var sharedData = systems.GetShared<SharedData>();
            _poolSet = sharedData.PoolSet;
            _eventsBus = sharedData.EventsBus;
            _readOnlySettings = sharedData.ReadOnlySettings;
            
            var windowObject = GetWindow();
            windowObject.LevelSizeInput.onEndEdit.AddListener(OnLevelSizeEditFinished);
            windowObject.MinesCountInput.onEndEdit.AddListener(OnMinesCountInput);
            windowObject.StartButton.onClick.AddListener(OnStartNewGameClick);
        }
        
        public void Destroy(IEcsSystems systems)
        {
            var windowObject = GetWindow();
            windowObject.LevelSizeInput.onEndEdit.RemoveAllListeners();
            windowObject.MinesCountInput.onEndEdit.RemoveAllListeners(); 
            windowObject.StartButton.onClick.RemoveAllListeners();
        }

        private void OnStartNewGameClick()
        {
            var windowObject = GetWindow();
            var gridSize = NormalizeGridSize(windowObject.LevelSizeInput.text);
            var minesCount = NormalizeMinesCount(windowObject.MinesCountInput.text, gridSize);
            _eventsBus.NewEventSingleton<StartNewGameEvent>() = new StartNewGameEvent(gridSize, minesCount);
            _eventsBus.NewEvent<WindowStateChangeRequest>() =
                new WindowStateChangeRequest(WindowType.NewGame, false);
        }

        private int NormalizeGridSize(string value)
        {
            return Mathf.Clamp(int.Parse(value),  _readOnlySettings.MinGridSize, _readOnlySettings.MaxGridSize);
        }

        private static int NormalizeMinesCount(string value, int gridSize)
        {
            var totalCellsCount = gridSize * gridSize;
            var maxMines = Mathf.FloorToInt(totalCellsCount * 0.5f);
            return Mathf.Clamp(int.Parse(value), MinMinesCount, maxMines);
        }

        private void OnLevelSizeEditFinished(string value)
        {
            var windowObject = GetWindow();
            windowObject.LevelSizeInput.text = NormalizeGridSize(value).ToString();
        }

        private void OnMinesCountInput(string value)
        {
            var windowObject = GetWindow();
            var gridSize = NormalizeGridSize(windowObject.LevelSizeInput.text);
            windowObject.MinesCountInput.text = NormalizeMinesCount(value, gridSize).ToString();
        }

        private NewGameWindow GetWindow()
        {
            return WindowUtils.GetWindow<NewGameWindow>(_poolSet, PrefabType.NewGameWindow, _windowFilter, _windowPool);
        }
    }
}