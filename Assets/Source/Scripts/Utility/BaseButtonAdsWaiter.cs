using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts.Utility
{
    public abstract class BaseButtonAdsWaiter : MonoBehaviour
    {
        private readonly float _cooldownTime = 10f;

        [SerializeField] private Button _adButton;
        [SerializeField] private Image _adsImage;
        [SerializeField] private Image _waitImage;

        private Coroutine _waitRoutine;

        private void OnEnable()
        {
            _adButton.onClick.AddListener(OnButtonClicked);
            WaitAds();
        }

        private void OnDisable()
        {
            _adButton.onClick.RemoveListener(OnButtonClicked);

            if (_waitRoutine != null)
                StopCoroutine(_waitRoutine);
        }

        protected virtual void LockButton()
        {
            _adButton.interactable = false;
            _adsImage.gameObject.SetActive(false);
            _waitImage.gameObject.SetActive(true);
        }

        protected virtual void UnlockButton()
        {
            _adButton.interactable = true;
            _waitImage.gameObject.SetActive(false);
            _adsImage.gameObject.SetActive(true);
        }

        protected virtual void OnButtonClicked()
        {
            WaitAds();
        }

        protected void WaitAds()
        {
            if (_waitRoutine != null)
                StopCoroutine(_waitRoutine);

            _waitRoutine = StartCoroutine(GetAdAvailability());
        }

        private IEnumerator GetAdAvailability()
        {
            LockButton();
            yield return new WaitForSeconds(_cooldownTime);
            UnlockButton();
        }
    }
}