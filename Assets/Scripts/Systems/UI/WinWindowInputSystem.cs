using Components;
using Config;
using UI;

namespace Systems.UI
{
    public class WinWindowInputSystem : BaseEndGameWindowInputSystem<WinWindow>
    {
        protected override WindowType CurrentWindowType => WindowType.Win;
        protected override PrefabType GetPrefabType() => PrefabType.WinWindow;
    }
}
