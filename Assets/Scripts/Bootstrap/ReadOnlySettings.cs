namespace Bootstrap
{
    public struct ReadOnlySettings
    {
        public readonly int MinGridSize;
        public readonly int MaxGridSize;
        public readonly float ScrollWheelSensibility;
        public readonly float DragSensibility;

        public ReadOnlySettings(int minGridSize, int maxGridSize,  float scrollWheelSensibility,  float dragSensibility)
        {
            MaxGridSize = maxGridSize;
            MinGridSize = minGridSize;
            ScrollWheelSensibility = scrollWheelSensibility;
            DragSensibility = dragSensibility;
        }
    }
}