using UnityEngine;
using UnityEngine.InputSystem;

namespace Systems.Camera
{
    public class CameraDragSystem : CameraInputSystem
    {
        protected override string ActionName => "Drag";

        protected override void ActionOnPerformed(InputAction.CallbackContext obj)
        {
            var amount = obj.ReadValue<Vector2>();
            var maxOffset = 0f;
            foreach (var entity in FieldCharacteristicsFilter)
            {
                ref var characteristics = ref FieldCharacteristicsPool.Get(entity);
                maxOffset = characteristics.MaxOffset;
                break;
            }
            foreach (var entity in CameraFilter)
            {
                ref var cameraComponent = ref CameraPool.Get(entity);
                if(Mathf.Abs(cameraComponent.Position.x + amount.x) > maxOffset)
                    break;
                if(Mathf.Abs(cameraComponent.Position.y + amount.y) > maxOffset)
                    break;
                cameraComponent.Drag(amount);
                DirtyPool.Add(entity);
                break;
            }
        }
    }
}