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
    public class NewGameWindowInputSystem : BaseWindowInputSystem<NewGameWindow>
    {
        private const int MinMinesCount = 1;
        private ReadOnlySettings _readOnlySettings;

        public override void Init(IEcsSystems systems)
        {
            base.Init(systems);
            var sharedData = systems.GetShared<SharedData>();
            _readOnlySettings = sharedData.ReadOnlySettings;
        }

        protected override PrefabType GetPrefabType() => PrefabType.NewGameWindow;

        protected override void Subscribe()
        {
            var windowObject = GetWindow();
            windowObject.LevelSizeInput.onEndEdit.AddListener(OnLevelSizeEditFinished);
            windowObject.MinesCountInput.onEndEdit.AddListener(OnMinesCountInput);
            windowObject.StartButton.onClick.AddListener(OnStartNewGameClick);
        }

        protected override void Unsubscribe()
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
            EventsBus.NewEventSingleton<StartNewGameEvent>() = new StartNewGameEvent(gridSize, minesCount);
            EventsBus.NewEvent<WindowStateChangeRequest>() =
                new WindowStateChangeRequest(WindowType.NewGame, false);
        }

        private int NormalizeGridSize(string value)
        {
            return Mathf.Clamp(int.Parse(value), _readOnlySettings.MinGridSize, _readOnlySettings.MaxGridSize);
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
    }
}