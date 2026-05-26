using Components;
using Events;
using Leopotam.EcsLite;
using UI;

namespace Systems.UI
{
    public class PauseWindowInputSystem : BaseWindowInputSystem<PauseWindow>
    {
        private EcsFilter _paramsFilter;
        private EcsPool<SavedParamsComponent> _paramsPool;
        private EcsFilter _gameStartedFilter;
        private EcsPool<GameStartedComponent> _gameStartedPool;

        public override void Init(IEcsSystems systems)
        {
            base.Init(systems);
            _paramsFilter = World.Filter<SavedParamsComponent>().End();
            _paramsPool = World.GetPool<SavedParamsComponent>();
            _gameStartedFilter = World.Filter<GameStartedComponent>().End();
            _gameStartedPool = World.GetPool<GameStartedComponent>();
        }

        protected override WindowType WindowType => Components.WindowType.Pause;

        protected override void Subscribe()
        {
            var windowObject = GetWindow();
            windowObject.RestartButton.onClick.AddListener(OnRestartClick);
            windowObject.ContinueButton.onClick.AddListener(OnHideClick);
            windowObject.ExitButton.onClick.AddListener(OnExitClick);
        }

        protected override void Unsubscribe()
        {
            var windowObject = GetWindow();
            windowObject.RestartButton.onClick.RemoveAllListeners();
            windowObject.ContinueButton.onClick.RemoveAllListeners();
            windowObject.ExitButton.onClick.RemoveAllListeners();
        }

        private void OnExitClick()
        {
            EventsBus.NewEvent<WindowStateChangeRequest>() = new WindowStateChangeRequest(Components.WindowType.Pause, false);
            EventsBus.NewEvent<WindowStateChangeRequest>() = new WindowStateChangeRequest(Components.WindowType.NewGame, true);
            foreach (var entity in _gameStartedFilter)
            {
                World.DelEntity(entity);
            }
        }

        private void OnHideClick()
        {
            EventsBus.NewEvent<WindowStateChangeRequest>() = new WindowStateChangeRequest(Components.WindowType.Pause, false);
            var entity = World.NewEntity();
            _gameStartedPool.Add(entity);
        }

        private void OnRestartClick()
        {
            EventsBus.NewEvent<WindowStateChangeRequest>() = new WindowStateChangeRequest(Components.WindowType.Pause, false);
            RestartUtils.RestartCurrentGame(_paramsFilter, _paramsPool, EventsBus);
        }
    }
}