using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class SingleMineUi : UIBehaviour
    {
        [field:SerializeField] public TMP_Text MinesNearCount { get; private set; }
        [field: SerializeField] public Button ActivateButton { get; private set; }
        [field: SerializeField] public GameObject ActivatedView { get; private set; }
    }
}