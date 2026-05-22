namespace Components
{
    public struct CellComponent
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public CellComponent Init(int x, int y)
        {
            X = x;
            Y = y;
            return this;
        }
    }
}