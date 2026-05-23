using UnityEngine;
using UnityEngine.InputSystem;

namespace Systems.Camera
{
    public class CameraZoomSystem : CameraInputSystem
    {
        protected override string ActionName => "ScrollWheel";
        protected override void ActionOnPerformed(InputAction.CallbackContext obj)
        {
            var amount = obj.ReadValue<Vector2>();
            foreach (var entity in CameraFilter)
            {
                ref var cameraComponent = ref CameraPool.Get(entity);
                var newValue = cameraComponent.OrthoSize + amount.y;
                if(newValue > ReadOnlySettings.MaxCameraOrthoSize || newValue < ReadOnlySettings.MinCameraOrhtoSize)
                    break;
                cameraComponent.Zoom(amount.y);
                DirtyPool.Add(entity);
                break;
            }
        }
    }
}