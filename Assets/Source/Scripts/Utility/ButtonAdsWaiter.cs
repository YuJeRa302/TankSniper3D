using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace Assets.Source.Game.Scripts.Utility
{
    public class ButtonAdsWaiter : MonoBehaviour
    {
        [SerializeField] private Button _adButton;
        [SerializeField] private Image _adsImage;
        [SerializeField] private Image _waitImage;

        private Coroutine _waitRoutine;

        public event Action AdsGetted;

        private void Awake()
        {
            YG2.onOpenInterAdv += OnOpenFullscreenAdCallback;
            YG2.onCloseInterAdv += OnCloseFullscreenAdCallback;
            YG2.onErrorInterAdv += OnErrorFullAdCallback;
            _adButton.onClick.AddListener(OnButtonClicked);
        }

        private void OnEnable()
        {
            WaitAds();
        }

        private void OnDestroy()
        {
            YG2.onOpenInterAdv -= OnOpenFullscreenAdCallback;
            YG2.onCloseInterAdv -= OnCloseFullscreenAdCallback;
            YG2.onErrorInterAdv -= OnErrorFullAdCallback;
            _adButton.onClick.RemoveListener(OnButtonClicked);

            if (_waitRoutine != null)
                StopCoroutine(_waitRoutine);
        }

        private void OnButtonClicked()
        {
            WaitAds();
            YG2.InterstitialAdvShow();
        }

        private void WaitAds()
        {
            _adButton.interactable = false;
            _adsImage.gameObject.SetActive(false);
            _waitImage.gameObject.SetActive(true);

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

            _waitImage.gameObject.SetActive(false);
            _adButton.interactable = true;
            _adsImage.gameObject.SetActive(true);
        }

        private void OnOpenFullscreenAdCallback()
        {
            AdsGetted?.Invoke();
        }

        private void OnCloseFullscreenAdCallback()
        {
            WaitAds();
        }

        private void OnErrorFullAdCallback()
        {
            WaitAds();
        }
    }
}