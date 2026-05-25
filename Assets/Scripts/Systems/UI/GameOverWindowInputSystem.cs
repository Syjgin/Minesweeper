using Components;
using Config;
using UI;

namespace Systems.UI
{
    public class GameOverWindowInputSystem : BaseEndGameWindowInputSystem<GameOverWindow>
    {
        protected override WindowType CurrentWindowType => WindowType.GameOver;
        protected override PrefabType GetPrefabType() => PrefabType.GameOverWindow;
    }
}
