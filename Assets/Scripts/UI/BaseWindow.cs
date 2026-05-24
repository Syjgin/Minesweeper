using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class BaseWindow : UIBehaviour
    {
        [SerializeField] private GameObject _uiRoot;
        public void Show()
        {
            _uiRoot.SetActive(true);
        }

        public void Hide()
        {
            _uiRoot.SetActive(false);
        }
    }
}