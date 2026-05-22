using SevenBoldPencil.EasyEvents;

namespace Events
{
    public struct StartNewGameEvent : IEventSingleton
    {
        public readonly int GridSize;
        public readonly int MinesCount;

        public StartNewGameEvent(int gridSize, int minesCount)
        {
            GridSize = gridSize;
            MinesCount = minesCount;
        }
    }
}