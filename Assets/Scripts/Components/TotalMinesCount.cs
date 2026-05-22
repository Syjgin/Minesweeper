namespace Components
{
    public struct TotalMinesCount
    {
        public int Value {get; private set;}

        public TotalMinesCount Init(int value)
        {
            Value = value;
            return this;
        }
    }
}