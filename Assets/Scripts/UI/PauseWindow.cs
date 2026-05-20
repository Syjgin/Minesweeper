using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class PauseWindow : UIBehaviour
    {
        [field: SerializeField] public Button RestartButton { get; private set; }
        [field: SerializeField] public Button ExitButton { get; private set; }
        [field: SerializeField] public Button ContinueButton { get; private set; }
    }
}