using Assets.Source.Game.Scripts.States;
using Assets.Source.Scripts.ScriptableObjects;
using Assets.Source.Scripts.Services;
using UnityEngine;

namespace Assets.Source.Scripts.Models
{
    public class GridModel
    {
        private readonly PersistentDataService _persistentDataService;
        private readonly int _levelDifferenceMultiplier = 2;

        private TankState _currentTankState;

        public GridModel(PersistentDataService persistentDataService)
        {
            _persistentDataService = persistentDataService;
            _currentTankState = _persistentDataService.PlayerProgress.TankService.GetStateByEquip();
        }

        public int CurrentMainTankLevel { get; private set; } = 1;
        public int CurrentGridTankLevel { get; private set; } = 1;

        public void IncreaseMainTankLevel(int currentLevel)
        {
            if (currentLevel > CurrentMainTankLevel)
                CurrentMainTankLevel++;

            IncreaseGridTankLevel(CurrentMainTankLevel);
        }

        public TankState GetTankState(TankData tankData)
        {
            return _persistentDataService.PlayerProgress.TankService.GetState(tankData);
        }

        public void ChangeEquippedTank(TankState tankState)
        {
            _currentTankState?.ChangeEquippedState(false);
            _currentTankState = tankState;
            _currentTankState.ChangeEquippedState(true);
        }

        private void IncreaseGridTankLevel(int currentLevel)
        {
            Debug.Log("currentLevel" + currentLevel);
            Debug.Log("CurrentMainTankLevel" + CurrentMainTankLevel);

            if (CurrentMainTankLevel > CurrentGridTankLevel * _levelDifferenceMultiplier)
                CurrentGridTankLevel++;
        }
    }
}
