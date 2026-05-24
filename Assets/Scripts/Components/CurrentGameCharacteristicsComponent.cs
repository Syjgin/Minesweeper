namespace Components
{
    public struct CurrentGameCharacteristicsComponent
    {
        public int GridSize { get; private set; }
        public int MinesCount { get; private set; }

        public CurrentGameCharacteristicsComponent Init(int gridSize, int minesCount)
        {
            GridSize = gridSize;
            MinesCount = minesCount;
            return this;
        }
    }
}