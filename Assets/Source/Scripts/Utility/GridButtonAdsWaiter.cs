using Assets.Source.Scripts.Grid;
using System;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Utility
{
    public class GridButtonAdsWaiter : BaseButtonAdsWaiter
    {
        [SerializeField] private AttentionButton _attentionButton;

        public event Action AdsGetted;

        protected override void LockButton()
        {
            base.LockButton();

            if (_attentionButton != null)
                _attentionButton.gameObject.SetActive(false);
        }

        protected override void UnlockButton()
        {
            base.UnlockButton();

            if (_attentionButton != null)
                _attentionButton.gameObject.SetActive(true);
        }

        protected override void OnCloseFullscreenAdCallback()
        {
            AdsGetted?.Invoke();
            WaitAds();
        }
    }
}