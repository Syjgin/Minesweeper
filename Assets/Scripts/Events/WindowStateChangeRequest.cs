using Components;
using SevenBoldPencil.EasyEvents;

namespace Events
{
    public struct WindowStateChangeRequest : IEventSingleton
    {
        public readonly bool IsOpen;
        public readonly WindowType WindowType;

        public WindowStateChangeRequest(WindowType windowType, bool isOpen)
        {
            IsOpen = isOpen;
            WindowType = windowType;
        }
    }
}