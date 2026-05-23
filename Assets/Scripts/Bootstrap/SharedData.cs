using Events;
using Pools;
using SevenBoldPencil.EasyEvents;
using UnityEngine.InputSystem;

namespace Bootstrap
{
    public class SharedData
    {
        public readonly EventsBus EventsBus;
        public readonly PoolSet PoolSet;
        public readonly PlayerInput PlayerInput;
        public readonly StartNewGameEvent InitialData;
        public readonly ReadOnlySettings ReadOnlySettings;

        public SharedData(EventsBus eventsBus, PoolSet poolSet, PlayerInput playerInput,
            StartNewGameEvent startNewGameEvent, ReadOnlySettings readOnlySettings)
        {
            EventsBus = eventsBus;
            PoolSet = poolSet;
            PlayerInput = playerInput;
            InitialData = startNewGameEvent;
            ReadOnlySettings = readOnlySettings;
        }
    }
}