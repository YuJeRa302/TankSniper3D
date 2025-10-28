using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace Assets.Source.Game.Scripts.Utility
{
    public abstract class BaseButtonAdsWaiter : MonoBehaviour
    {
        [SerializeField] private Button _adButton;
        [SerializeField] private Image _adsImage;
        [SerializeField] private Image _waitImage;

        protected Coroutine _waitRoutine;

        private void OnEnable()
        {
            YG2.onCloseInterAdv += OnCloseFullscreenAdCallback;
            YG2.onErrorInterAdv += OnErrorFullAdCallback;
            _adButton.onClick.AddListener(OnButtonClicked);
            WaitAds();
        }

        private void OnDisable()
        {
            YG2.onCloseInterAdv -= OnCloseFullscreenAdCallback;
            YG2.onErrorInterAdv -= OnErrorFullAdCallback;
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
            _waitImage.gameObject.SetActive(false);
            _adButton.interactable = true;
            _adsImage.gameObject.SetActive(true);
        }

        protected virtual void OnCloseFullscreenAdCallback()
        {
            if (gameObject.activeSelf)
                StartCoroutine(WaitingCloseAds());
        }

        protected virtual void OnButtonClicked()
        {
            StartCoroutine(WaitingCloseAds());
            YG2.InterstitialAdvShow();
        }

        protected IEnumerator WaitingCloseAds()
        {
            yield return new WaitUntil(() => !YG2.nowInterAdv);
            WaitAds();
        }

        private void WaitAds()
        {
            LockButton();

            if (_waitRoutine != null)
                StopCoroutine(_waitRoutine);

            _waitRoutine = StartCoroutine(GetAdAvailability());
        }

        private IEnumerator GetAdAvailability()
        {
            while (!YG2.isTimerAdvCompleted)
            {
                yield return null;
            }

            UnlockButton();
        }

        private void OnErrorFullAdCallback()
        {
            if (gameObject.activeSelf)
                StartCoroutine(WaitingCloseAds());
        }
    }
}