namespace Components
{
    public struct FieldCharacteristics
    {
        public int MinesCount { get; private set; }
        public float MaxOffset {get; private set;}

        public FieldCharacteristics Init(int value, float maxOffset)
        {
            MinesCount = value;
            MaxOffset = maxOffset;
            return this;
        }
    }
}