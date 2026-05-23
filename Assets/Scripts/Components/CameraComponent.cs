using UnityEngine;

namespace Components
{
    public struct CameraComponent
    {
        public Vector3 Position {get; private set;}
        
        public CameraComponent Init(Vector3 position)
        {
            Position = position;
            return this;
        }

        public CameraComponent Drag(Vector2 delta)
        {
            var currentPos = Position;
            return Init(new Vector3(currentPos.x + delta.x, currentPos.y, currentPos.z + delta.y));
        }

        public CameraComponent Zoom(float newZ)
        {
            var currentPos = Position;
            return Init(new Vector3(currentPos.x, newZ, currentPos.z));
        }
    }
}