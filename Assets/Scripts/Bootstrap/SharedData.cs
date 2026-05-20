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

        public SharedData(EventsBus eventsBus, PoolSet poolSet, PlayerInput playerInput)
        {
            EventsBus = eventsBus;
            PoolSet = poolSet;
            PlayerInput = playerInput;
        }
    }
}