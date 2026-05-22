using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace View
{
    public class CellView : MonoBehaviour, IPointerClickHandler
    {
        [field: SerializeField] public Image BackgroundImage { get; private set; }
        [field: SerializeField] public TMP_Text MinesNearCountText {get; private set;}
        [field: SerializeField] public GameObject ActivatedView { get; private set; }
        
        public event Action<CellView> OnClick;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick?.Invoke(this);
        }
    }
}
