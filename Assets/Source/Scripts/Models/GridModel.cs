using Assets.Source.Game.Scripts.States;
using Assets.Source.Scripts.ScriptableObjects;
using Assets.Source.Scripts.Services;

namespace Assets.Source.Scripts.Models
{
    public class GridModel
    {
        private readonly PersistentDataService _persistentDataService;
        private readonly int _maxCountBuyTank = 8;

        private int _currentCountBuyTank = 0;
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
        }

        public bool TryBuyGridTank(int coast)
        {
            if (_persistentDataService.TrySpendMoney(coast))
            {
                _currentCountBuyTank++;
                UpdateGridTankLevel();
                return true;
            }

            return false;
        }

        public TankState GetTankStateByEquip()
        {
            return _persistentDataService.PlayerProgress.TankService.GetStateByEquip();
        }

        public TankState GetTankStateByData(TankData tankData)
        {
            return _persistentDataService.PlayerProgress.TankService.GetState(tankData);
        }

        public GridTankState GetGridTankState(GridTankData gridTankData)
        {
            return _persistentDataService.PlayerProgress.GridService.GetState(gridTankData);
        }

        public void ChangeEquippedTank(TankState tankState)
        {
            _currentTankState?.ChangeEquippedState(false);
            _currentTankState = tankState;
            _currentTankState.ChangeEquippedState(true);
            _currentTankState.ChangeOpenState(true);
        }

        private void UpdateGridTankLevel()
        {
            if (_currentCountBuyTank == _maxCountBuyTank)
            {
                CurrentGridTankLevel++;
                _currentCountBuyTank = 0;
            }
        }
    }
}
