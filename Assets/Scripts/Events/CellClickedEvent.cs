using SevenBoldPencil.EasyEvents;
using UnityEngine;
using View;

namespace Events
{
    public struct CellClickedEvent : IEventSingleton
    {
        public readonly MouseClickData MouseClickData;
        
        public  CellClickedEvent(MouseClickData mouseClickData)
        {
            MouseClickData = mouseClickData;
        }
    }
}