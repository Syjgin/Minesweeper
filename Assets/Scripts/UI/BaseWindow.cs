using UnityEngine.EventSystems;

namespace UI
{
    public class BaseWindow : UIBehaviour
    {
        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}