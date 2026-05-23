namespace Bootstrap
{
    public struct ReadOnlySettings
    {
        public readonly float InitialCameraHeight;
        public readonly float CellSize;
        public readonly float MinCameraZ;
        public readonly float MaxCameraZ;

        public ReadOnlySettings(float initialCameraHeight, float cellSize, float minCameraZ, float maxCameraZ)
        {
            InitialCameraHeight = initialCameraHeight;
            CellSize = cellSize;
            MinCameraZ = minCameraZ;
            MaxCameraZ = maxCameraZ;
        }
    }
}