namespace Components
{
    public struct WindowComponent
    {
        public bool IsOpen { get; private set; }
        public WindowType WindowType { get; private set; }

        public void SetWindowType(WindowType windowType)
        {
            WindowType = windowType;
        }

        public void SetOpened(bool isOpen)
        {
            IsOpen = isOpen;
        }
    }
}