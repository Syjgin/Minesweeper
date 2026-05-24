namespace Components
{
    public struct CellCoordinateComponent
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public CellCoordinateComponent Init(int x, int y)
        {
            X = x;
            Y = y;
            return this;
        }
    }
}