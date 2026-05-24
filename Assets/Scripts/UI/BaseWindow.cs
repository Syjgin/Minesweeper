using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class BaseWindow : UIBehaviour
    {
        [SerializeField] private GameObject[] _uiToShow;
        public void Show()
        {
            ChangeVisibility(true);
        }

        public void Hide()
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