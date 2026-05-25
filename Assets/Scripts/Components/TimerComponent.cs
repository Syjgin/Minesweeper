namespace Components
{
    public struct TimerComponent
    {
        public int WholeSeconds { get; private set; }
        public float SecondPart {get; private set;}

        public TimerComponent AddDelta(float delta)
        {
            var newValue = SecondPart + delta;
            if (newValue > 1)
            {
                WholeSeconds += 1;
                SecondPart = newValue - 1;
            }
            else
            {
                SecondPart = newValue;
            }
            return this;
        }
    }
}