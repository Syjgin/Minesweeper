using UnityEngine;

namespace Components
{
    public struct CameraComponent
    {
        public Vector3 Position {get; private set;}
        
        public CameraComponent Update(Vector3 position)
        {
            Position = position;
            return this;
        }
    }
}