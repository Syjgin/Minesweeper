using Bootstrap;
using Components;
using Leopotam.EcsLite;
using Pools;
using SevenBoldPencil.EasyEvents;
using UI;

namespace Systems.UI
{
    public abstract class BaseWindowInputSystem<TWindow> : IEcsInitSystem, IEcsDestroySystem where TWindow : BaseWindow
    {
        protected PoolSet PoolSet;
        protected EcsFilter WindowFilter;
        protected EcsPool<WindowComponent> WindowPool;
        protected EventsBus EventsBus;
        protected EcsWorld World;

        public virtual void Init(IEcsSystems systems)
        {
            World = systems.GetWorld();
            WindowFilter = World.Filter<WindowComponent>().End();
            WindowPool = World.GetPool<WindowComponent>();
            var sharedData = systems.GetShared<SharedData>();
            PoolSet = sharedData.PoolSet;
            EventsBus = sharedData.EventsBus;
            Subscribe();
        }

        public virtual void Destroy(IEcsSystems systems)
        {
            Unsubscribe();
        }

        protected abstract void Subscribe();
        protected abstract void Unsubscribe();
        protected abstract WindowType WindowType { get; }

        protected TWindow GetWindow()
        {
            return WindowUtils.GetWindow<TWindow>(PoolSet, WindowType, WindowFilter, WindowPool);
        }
    }
}
