using SevenBoldPencil.EasyEvents;

namespace Events
{
    public struct PauseRequest : IEventSingleton
    {
        public bool IsPaused { get; private set; }
        public PauseRequest(bool isPaused)
        {
            IsPaused = isPaused;
        }
    }
}