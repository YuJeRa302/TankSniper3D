using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Game.Scripts.States;
using Assets.Source.Scripts.ScriptableObjects;
using Assets.Source.Scripts.Services;

namespace Assets.Source.Scripts.Models
{
    public class UpgradeModel
    {
        private readonly PersistentDataService _persistentDataService;

        private TankState _currentTankState;

        public UpgradeModel(PersistentDataService persistentDataService)
        {
            _persistentDataService = persistentDataService;
        }

        public TankState TankState => _currentTankState;

        public int GetMoney()
        {
            return _persistentDataService.PlayerProgress.Money;
        }

        public TankState GetTankStateByEquip()
        {
            return _persistentDataService.PlayerProgress.TankService.GetStateByEquip();
        }

        public TankState GetTankState(TankData tankData)
        {
            return _persistentDataService.PlayerProgress.TankService.GetState(tankData);
        }

        public HeroState GetHeroState(HeroData heroData)
        {
            return _persistentDataService.PlayerProgress.HeroService.GetStateByData(heroData);
        }

        public DecorationState GetDecorationState(DecorationData decorationData)
        {
            return _persistentDataService.PlayerProgress.DecorationService.GetStateByData(decorationData);
        }

        public void UnlockByReward(int id, TypeCard typeCard)
        {
            if (typeCard == TypeCard.Hero)
                UnlockHero(id);
            else
                UnlockDecoration(id, typeCard);
        }

        public void SelectHero(HeroState heroState)
        {
            _currentTankState.ChangeHero(heroState.Id);
        }

        public void SelectDecoration(DecorationState decorationState)
        {
            _currentTankState.ChangeDecoration(decorationState);
        }

        public void SelectTank(TankState tankState)
        {
            _currentTankState = tankState;
            _currentTankState.ChangeEquippedState(true);
        }

        private void UnlockDecoration(int id, TypeCard typeCard)
        {
            var state = _persistentDataService.PlayerProgress.DecorationService.GetState(id, typeCard);
            state.ChangeBuyState(true);
        }

        private void UnlockHero(int id)
        {
            var state = _persistentDataService.PlayerProgress.HeroService.GetState(id);
            state.ChangeBuyState(true);
        }
    }
}