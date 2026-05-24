namespace Components
{
    public struct CellVisualStateComponent
    {
        public CellVisual Visual { get; private set; }
        public int NearMinesCount { get; private set; }

        public CellVisualStateComponent UpdateVisual(CellVisual visualComponent)
        {
            Visual = visualComponent;
            return this;
        }

        public CellVisualStateComponent UpdateNearMinesCount(int nearMinesCount)
        {
            NearMinesCount = nearMinesCount;
            return this;
        }
    }
}