using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class NewGameWindow : BaseWindow
    {
        [field:SerializeField] public Button StartButton { get; private set; }
        [field: SerializeField] public TMP_InputField LevelSizeInput { get; private set; }
        [field: SerializeField] public TMP_InputField MinesCountInput { get; private set; }
    }
}