using System.Collections;
using UnityEngine;

namespace UI
{
    public class GameOverWindow : BaseGameEndWindow
    {
        private readonly WaitForSeconds _wait = new(1f);
        [SerializeField] private GameObject _mainUi;
        public override void Show()
        {
            base.Show();
            StartCoroutine(WaitAndShowMainUi());
        }

        public override void Hide()
        {
            base.Hide();
            _mainUi.SetActive(false);
        }

        private IEnumerator WaitAndShowMainUi()
        {
            yield return _wait;
            _mainUi.SetActive(true);
        }
    }
}