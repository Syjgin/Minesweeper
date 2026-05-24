namespace Components
{
    public struct SavedParamsComponent
    {
        public int GridSize { get; private set; }
        public int MinesCount { get; private set; }

        public SavedParamsComponent Init(int gridSize, int minesCount)
        {
            GridSize = gridSize;
            MinesCount = minesCount;
            return this;
        }
    }
}