using Bootstrap;
using Components;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Systems.Camera
{
    public abstract class CameraInputSystem : IEcsInitSystem, IEcsDestroySystem
    {
        protected abstract string ActionName { get; }
        private InputAction _action;
        protected EcsFilter CameraFilter;
        protected EcsPool<CameraComponent> CameraPool;
        protected EcsPool<Dirty> DirtyPool;
        protected ReadOnlySettings ReadOnlySettings;
        protected EcsPool<FieldCharacteristics>  FieldCharacteristicsPool;
        protected EcsFilter FieldCharacteristicsFilter;

        public void Init(IEcsSystems systems)
        {
            var sharedData = systems.GetShared<SharedData>();
            var playerInput = sharedData.PlayerInput;
            var world = systems.GetWorld();
            ReadOnlySettings = sharedData.ReadOnlySettings;
            CameraFilter = world.Filter<CameraComponent>().Inc<CameraComponent>().End();
            CameraPool = world.GetPool<CameraComponent>();
            DirtyPool = world.GetPool<Dirty>();
            FieldCharacteristicsPool = world.GetPool<FieldCharacteristics>();
            FieldCharacteristicsFilter = world.Filter<FieldCharacteristics>().End();
            _action = playerInput.actions.FindAction(ActionName);
            _action.performed += ActionOnPerformed;
        }

        protected abstract void ActionOnPerformed(InputAction.CallbackContext obj);

        public void Destroy(IEcsSystems systems)
        {
            _action.performed -= ActionOnPerformed;
        }
    }
}