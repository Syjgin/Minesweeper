using Components;
using UI;

namespace Systems.UI
{
    public class WinWindowInputSystem : BaseEndGameWindowInputSystem<WinWindow>
    {
        protected override WindowType CurrentWindowType => WindowType.Win;
    }
}
