using Assets.Source.Game.Scripts.States;
using Assets.Source.Scripts.ScriptableObjects;
using Assets.Source.Scripts.Services;
using UnityEngine.SceneManagement;

namespace Assets.Source.Scripts.Models
{
    public class GameModel
    {
        private readonly GameData _gameData;
        private readonly UpgradeConfig _upgradeConfig;
        private readonly PersistentDataService _persistentDataService;

        public GameModel(PersistentDataService persistentDataService, UpgradeConfig upgradeConfig, GameData gameData)
        {
            _persistentDataService = persistentDataService;
            _upgradeConfig = upgradeConfig;
            _gameData = gameData;
        }

        public LevelData GetLevelData()
        {
            return _gameData.BiomsConfig.
                GetBiomDataById(_persistentDataService.PlayerProgress.CurrentBiomId).
                GetLevelDataById(_persistentDataService.PlayerProgress.CurrentLevelId);
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

        public void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}