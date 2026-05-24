using Components;
using Events;
using Leopotam.EcsLite;
using SevenBoldPencil.EasyEvents;

namespace Systems.UI
{
    public static class RestartUtils
    {
        public static void RestartCurrentGame(EcsFilter paramsFilter, EcsPool<SavedParamsComponent> pool, EventsBus eventsBus)
        {
            foreach (var entity in paramsFilter)
            {
                ref var characteristics = ref pool.Get(entity);
                eventsBus.NewEventSingleton<StartNewGameEvent>() = new StartNewGameEvent(characteristics.GridSize, characteristics.MinesCount);
            }
        }
    }
}