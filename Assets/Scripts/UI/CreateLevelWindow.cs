using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class CreateLevelWindow : UIBehaviour
    {
        [field:SerializeField] public Button CreateButton { get; private set; }
        [field: SerializeField] public TMP_InputField LevelSizeInput { get; private set; }
        [field: SerializeField] public TMP_InputField MinesCountInput { get; private set; }
    }
}