namespace Components
{
    public struct FieldComponent
    {
        public int MinesCount { get; private set; }
        public float MaxOffset {get; private set;}

        public FieldComponent Init(int value, float maxOffset)
        {
            MinesCount = value;
            MaxOffset = maxOffset;
            return this;
        }
    }
}