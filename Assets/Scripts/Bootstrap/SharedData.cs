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
        public readonly float InitialCameraHeight;

        public SharedData(EventsBus eventsBus, PoolSet poolSet, PlayerInput playerInput, StartNewGameEvent startNewGameEvent, float initialCameraHeight)
        {
            EventsBus = eventsBus;
            PoolSet = poolSet;
            PlayerInput = playerInput;
            InitialData = startNewGameEvent;
            InitialCameraHeight = initialCameraHeight;
        }
    }
}