using Assets.Source.Game.Scripts.Enemy;
using Assets.Source.Game.Scripts.States;
using Assets.Source.Scripts.ScriptableObjects;
using Assets.Source.Scripts.Services;
using UniRx;
using UnityEngine.SceneManagement;

namespace Assets.Source.Scripts.Models
{
    public class GameModel
    {
        public static readonly IMessageBroker Message = new MessageBroker();

        private readonly int _maxDroneCount = 10;
        private readonly GameData _gameData;
        private readonly UpgradeConfig _upgradeConfig;
        private readonly PersistentDataService _persistentDataService;

        private int _currentDroneCount;
        private int _moneyEarned = 0;

        public GameModel(PersistentDataService persistentDataService, UpgradeConfig upgradeConfig, GameData gameData)
        {
            _persistentDataService = persistentDataService;
            _upgradeConfig = upgradeConfig;
            _gameData = gameData;
            _currentDroneCount = _maxDroneCount;
        }

        public bool TryCreateDrone()
        {
            if (_currentDroneCount > 0)
            {
                _currentDroneCount--;
                return true;
            }

            return false;
        }

        public LevelData GetLevelData()
        {
            return _gameData.BiomsConfig.
                GetBiomDataById(_persistentDataService.PlayerProgress.CurrentBiomId).
                GetLevelDataById(_persistentDataService.PlayerProgress.CurrentLevelId);
        }

        public TankData GetTankData()
        {
            return _upgradeConfig
                .GetTankDataById(_persistentDataService.PlayerProgress.TankService.GetStateByEquip().Id);
        }

        public TankState GetTankState(TankData tankData)
        {
            return _persistentDataService.PlayerProgress.TankService.GetState(tankData);
        }

        public int GetHeroId()
        {
            return _persistentDataService.PlayerProgress.TankService.GetStateByEquip().HeroId;
        }

        public int GetMoney()
        {
            return _persistentDataService.PlayerProgress.Money;
        }

        public int GetEarnedMoney()
        {
            return _moneyEarned;
        }

        public void SetEarnedMoney(int earnedMoney)
        {
            _moneyEarned += earnedMoney;
            Message.Publish(new M_DeathEnemy());
        }

        public void ReloadScene()
        {
            _moneyEarned = 0;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}