using UnityEngine;

namespace Components
{
    public struct CameraComponent
    {
        public Vector2 Position { get; private set; }
        public float OrthoSize { get; private set; }

        public CameraComponent Init(Vector2 position, float orthoSize)
        {
            Position = position;
            OrthoSize = orthoSize;
            return this;
        }

        public CameraComponent Drag(Vector2 delta)
        {
            var currentPos = Position;
            return Init(new Vector2(currentPos.x + delta.x, currentPos.y + delta.y), OrthoSize);
        }

        public CameraComponent Zoom(float newOrthoSize)
        {
            var currentPos = Position;
            return Init(new Vector2(currentPos.x, currentPos.y), newOrthoSize);
        }
    }
}