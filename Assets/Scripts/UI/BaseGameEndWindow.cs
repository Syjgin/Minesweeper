using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class BaseGameEndWindow : BaseWindow
    {
        [field: SerializeField] public Button RestartButton { get; private set; }
        [field: SerializeField] public Button ExitButton { get; private set; }
    }
}