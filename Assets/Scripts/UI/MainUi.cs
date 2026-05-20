using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class MainUi : UIBehaviour
    {
        [field: SerializeField] public Button RestartButton { get; private set; }
        [field: SerializeField] public Button PauseButton { get; private set; }
        [field: SerializeField] public TMP_Text Timer { get; private set; }
        [field: SerializeField] public TMP_Text MinesCount { get; private set; }
    }
}