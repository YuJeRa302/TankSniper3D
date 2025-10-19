using Assets.Source.Game.Scripts.States;
using Assets.Source.Scripts.ScriptableObjects;
using Assets.Source.Scripts.Services;

namespace Assets.Source.Scripts.Models
{
    public class GameModel
    {
        private readonly UpgradeConfig _upgradeConfig;
        private readonly PersistentDataService _persistentDataService;

        public GameModel(PersistentDataService persistentDataService, UpgradeConfig upgradeConfig)
        {
            _persistentDataService = persistentDataService;
            _upgradeConfig = upgradeConfig;
        }

        public TankData GetTankData()
        {
            return _upgradeConfig.GetTankDataById(_persistentDataService.PlayerProgress.CurrentPlayerTankId);
        }

        public TankState GetTankState(TankData tankData)
        {
            return _persistentDataService.PlayerProgress.TankService.GetState(tankData);
        }

        public int GetMoney()
        {
            return _persistentDataService.PlayerProgress.Money;
        }
    }
}