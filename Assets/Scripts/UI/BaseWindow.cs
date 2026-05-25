using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class BaseWindow : UIBehaviour
    {
        [SerializeField] private GameObject[] _uiToShow;
        public virtual void Show()
        {
            ChangeVisibility(true);
        }

        public virtual void Hide()
        {
            ChangeVisibility(false);
        }
        
        private void ChangeVisibility(bool isVisible)
        {
            foreach (var go in _uiToShow)
            {
                go.SetActive(isVisible);
            }
        }
    }
}