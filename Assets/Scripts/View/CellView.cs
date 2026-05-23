using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class CellView : MonoBehaviour
    {
        [field: SerializeField] public Image BackgroundImage { get; private set; }
        [field: SerializeField] public TMP_Text MinesNearCountText {get; private set;}
        [field: SerializeField] public GameObject ActivatedView { get; private set; }
    }
}
