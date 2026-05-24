using SevenBoldPencil.EasyEvents;
using UnityEngine;

namespace Events
{
    public struct FieldDragEvent : IEventSingleton
    {
        public readonly Vector2 Delta;
        
        public FieldDragEvent(Vector2 delta)
        {
            Delta = delta;
        }
    }
}