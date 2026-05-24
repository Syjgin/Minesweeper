using SevenBoldPencil.EasyEvents;
using UnityEngine;

namespace Events
{
    public struct CellClickedEvent : IEventSingleton
    {
        public readonly Vector2Int CellIndex;
        
        public  CellClickedEvent(Vector2Int cellIndex)
        {
            CellIndex = cellIndex;
        }
    }
}