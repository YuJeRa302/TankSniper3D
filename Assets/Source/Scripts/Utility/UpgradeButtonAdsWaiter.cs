using Assets.Source.Game.Scripts.Enums;
using System;

namespace Assets.Source.Game.Scripts.Utility
{
    public class UpgradeButtonAdsWaiter : BaseButtonAdsWaiter
    {
        private TypeCard _typeCard;
        private int _cardId;

        public event Action<int, TypeCard> AdsOpened;

        public void Initialize(int cardId, TypeCard typeCard)
        {
            _cardId = cardId;
            _typeCard = typeCard;
        }

        public void SetWaitAds()
        {
            if (gameObject.activeSelf)
                WaitAds();
        }

        protected override void OnButtonClicked()
        {
            AdsOpened?.Invoke(_cardId, _typeCard);
        }
    }
}