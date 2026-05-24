using UnityEngine;

namespace Components
{
    public struct CameraComponent
    {
        public Vector2 Position { get; private set; }
        public float OrthoSize { get; private set; }
        public float DefaultOrthoSize { get; private set; }

        public CameraComponent Init(Vector2 position, float orthoSize)
        {
            Position = position;
            OrthoSize = orthoSize;
            DefaultOrthoSize = orthoSize;
            return this;
        }

        public CameraComponent Drag(Vector2 delta)
        {
            Position += delta;
            return this;
        }

        public CameraComponent Zoom(float newOrthoSize)
        {
            OrthoSize = newOrthoSize;
            return this;
        }
    }
}