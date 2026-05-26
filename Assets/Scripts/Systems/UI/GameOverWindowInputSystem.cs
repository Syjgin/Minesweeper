using Components;
using UI;

namespace Systems.UI
{
    public class GameOverWindowInputSystem : BaseEndGameWindowInputSystem<GameOverWindow>
    {
        protected override WindowType CurrentWindowType => WindowType.GameOver;
    }
}
