using Assets.Source.Game.Scripts.Enums;
using System;

namespace Assets.Source.Game.Scripts.Utility
{
    public class GameButtonAdsWaiter : BaseButtonAdsWaiter
    {
        private TypeReward _typeReward;

        public event Action<TypeReward> AdsOpened;

        public void Initialize(TypeReward typeReward)
        {
            _typeReward = typeReward;
        }

        protected override void OnButtonClicked()
        {
            AdsOpened?.Invoke(_typeReward);
        }
    }
}