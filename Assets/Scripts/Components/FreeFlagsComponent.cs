namespace Components
{
    public struct FreeFlagsComponent
    {
        public int Amount { get; private set; }
        
        public FreeFlagsComponent SetAmount(int amount)
        {
            Amount = amount;
            return this;
        }
    }
}