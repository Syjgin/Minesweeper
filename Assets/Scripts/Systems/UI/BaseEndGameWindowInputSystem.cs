using Components;
using Events;
using Leopotam.EcsLite;
using UI;

namespace Systems.UI
{
    public abstract class BaseEndGameWindowInputSystem<T> : BaseWindowInputSystem<T> where T: BaseGameEndWindow
    {
        protected EcsFilter ParamsFilter;
        protected EcsPool<SavedParamsComponent> ParamsPool;
        protected EcsFilter GameStartedFilter;
        protected abstract WindowType CurrentWindowType { get; }

        public override void Init(IEcsSystems systems)
        {
            base.Init(systems);
            ParamsFilter = World.Filter<SavedParamsComponent>().End();
            ParamsPool = World.GetPool<SavedParamsComponent>();
            GameStartedFilter = World.Filter<GameStartedComponent>().End();
        }

        protected override void Subscribe()
        {
            var windowObject = GetWindow();
            windowObject.RestartButton.onClick.AddListener(OnRestartClick);
            windowObject.ExitButton.onClick.AddListener(OnExitClick);
        }

        protected override void Unsubscribe()
        {
            var windowObject = GetWindow();
            windowObject.RestartButton.onClick.RemoveAllListeners();
            windowObject.ExitButton.onClick.RemoveAllListeners();
        }

        protected virtual void OnRestartClick()
        {
            EventsBus.NewEvent<WindowStateChangeRequest>() = new WindowStateChangeRequest(CurrentWindowType, false);
            RestartUtils.RestartCurrentGame(ParamsFilter, ParamsPool, EventsBus);
        }

        protected virtual void OnExitClick()
        {
            EventsBus.NewEvent<WindowStateChangeRequest>() = new WindowStateChangeRequest(CurrentWindowType, false);
            EventsBus.NewEvent<WindowStateChangeRequest>() = new WindowStateChangeRequest(WindowType.NewGame, true);
            foreach (var entity in GameStartedFilter)
            {
                World.DelEntity(entity);
            }
        }
    }
}
