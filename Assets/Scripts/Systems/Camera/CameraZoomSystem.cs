using Bootstrap;
using Components;
using Config;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Systems.Camera
{
    public class CameraZoomSystem : IEcsInitSystem, IEcsDestroySystem
    {
        private InputAction _action;
        private EcsFilter _cameraFilter;
        private EcsPool<CameraComponent> _cameraPool;
        private EcsPool<Dirty> _dirtyPool;
        private ReadOnlySettings _readOnlySettings;
        
        private const string ActionName = "ScrollWheel";

        public void Init(IEcsSystems systems)
        {
            var sharedData = systems.GetShared<SharedData>();
            var playerInput = sharedData.PlayerInput;
            var world = systems.GetWorld();
            _readOnlySettings = sharedData.ReadOnlySettings;
            _cameraFilter = world.Filter<CameraComponent>().End();
            _cameraPool = world.GetPool<CameraComponent>();
            _dirtyPool = world.GetPool<Dirty>();
            _action = playerInput.actions.FindAction(ActionName);
            _action.performed += ActionOnPerformed;
        }

        public void Destroy(IEcsSystems systems)
        {
            _action.performed -= ActionOnPerformed;
        }
        
        private void ActionOnPerformed(InputAction.CallbackContext obj)
        {
            var amount = obj.ReadValue<Vector2>();
            foreach (var entity in _cameraFilter)
            {
                ref var cameraComponent = ref _cameraPool.Get(entity);
                var newValue = cameraComponent.OrthoSize - (amount.y * _readOnlySettings.ScrollWheelSensibility);
                if(newValue > cameraComponent.DefaultOrthoSize || newValue < Constants.MinCameraOrthoSize)
                    break;
                cameraComponent.Zoom(newValue);
                if(!_dirtyPool.Has(entity))
                    _dirtyPool.Add(entity);
                break;
            }
        }
    }
}