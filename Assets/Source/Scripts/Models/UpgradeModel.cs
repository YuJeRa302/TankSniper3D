using Assets.Source.Game.Scripts.States;
using Assets.Source.Scripts.ScriptableObjects;
using Assets.Source.Scripts.Services;
using UnityEngine;

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
            return _persistentDataService.PlayerProgress.HeroService.GetState(heroData);
        }

        public DecorationState GetDecorationState(DecorationData decorationData)
        {
            return _persistentDataService.PlayerProgress.DecorationService.GetState(decorationData);
        }

        public void SelectHero(HeroState heroState)
        {
            heroState.ChangeEquippedState(true);
            _persistentDataService.PlayerProgress.HeroService.SetStateChangeEquipped(heroState);
            _currentTankState.ChangeHero(heroState.Id);
        }

        public void SelectDecoration(DecorationState decorationState)
        {
            decorationState.ChangeEquippedState(true);
            _persistentDataService.PlayerProgress.DecorationService.SetStateChangeEquipped(decorationState);
            _currentTankState.ChangeDecoration(decorationState);
        }

        public void SelectTank(TankState tankState)
        {
            _currentTankState = tankState;
            _currentTankState.ChangeEquippedState(true);
        }
    }
}