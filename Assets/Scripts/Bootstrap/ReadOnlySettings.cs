namespace Bootstrap
{
    public struct ReadOnlySettings
    {
        public readonly float MinGridSize;
        public readonly float MaxGridSize;
        public readonly float ScrollWheelSensibility;
        public readonly float DragSensibility;

        public ReadOnlySettings(float maxGridSize, float minGridSize,  float scrollWheelSensibility,  float dragSensibility)
        {
            MaxGridSize = maxGridSize;
            MinGridSize = minGridSize;
            ScrollWheelSensibility = scrollWheelSensibility;
            DragSensibility = dragSensibility;
        }
    }
}