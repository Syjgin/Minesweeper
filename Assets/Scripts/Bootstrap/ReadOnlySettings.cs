namespace Bootstrap
{
    public struct ReadOnlySettings
    {
        public readonly float InitialCameraOrthoSize;
        public readonly float CellSize;
        public readonly float MinCameraOrhtoSize;
        public readonly float MaxCameraOrthoSize;

        public ReadOnlySettings(float initialCameraOrthoSize, float cellSize, float minCameraOrhtoSize, float maxCameraOrthoSize)
        {
            InitialCameraOrthoSize = initialCameraOrthoSize;
            CellSize = cellSize;
            MinCameraOrhtoSize = minCameraOrhtoSize;
            MaxCameraOrthoSize = maxCameraOrthoSize;
        }
    }
}