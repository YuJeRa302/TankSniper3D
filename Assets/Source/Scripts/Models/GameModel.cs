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

        private readonly string _upgradeSceneName = "UpgradeScene";
        private readonly int _maxDroneCount = 10;
        private readonly GameData _gameData;
        private readonly UpgradeConfig _upgradeConfig;
        private readonly PersistentDataService _persistentDataService;

        private LevelData _levelData;
        private int _currentDroneCount;
        private int _moneyEarned = 0;

        //public GameModel(PersistentDataService persistentDataService, UpgradeConfig upgradeConfig, GameData gameData, LevelData levelData) // для теста
        //{
        //    _persistentDataService = persistentDataService;
        //    _upgradeConfig = upgradeConfig;
        //    _gameData = gameData;
        //    _currentDroneCount = _maxDroneCount;
        //    _levelData = levelData;
        //}

        public GameModel(PersistentDataService persistentDataService, UpgradeConfig upgradeConfig, GameData gameData)
        {
            _persistentDataService = persistentDataService;
            _upgradeConfig = upgradeConfig;
            _gameData = gameData;
            _currentDroneCount = _maxDroneCount;
        }

        public bool IsGameEnded { get; private set; } = false;

        public bool TryCreateDrone()
        {
            if (_currentDroneCount > 0)
            {
                _currentDroneCount--;
                return true;
            }

            return false;
        }

        //public LevelData GetLevelData() // для тестирования
        //{
        //    return _levelData;
        //}

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

        public HeroData GetNewHeroData()
        {
            foreach (var state in _persistentDataService.PlayerProgress.HeroService.HeroStates)
            {
                if (state.IsOpened == false)
                    return _upgradeConfig.GetHeroDataById(state.Id);
            }

            return null;
        }

        public int GetLevel()
        {
            return _persistentDataService.PlayerProgress.CurrentLevel;
        }

        public int GetMoney()
        {
            return _persistentDataService.PlayerProgress.Money;
        }

        public bool GetVibroState()
        {
            return _persistentDataService.PlayerProgress.IsVibro;
        }

        public int GetEarnedMoney()
        {
            return _moneyEarned;
        }

        public void OpenHeroByData(HeroData heroData)
        {
            var state = _persistentDataService.PlayerProgress.HeroService.GetState(heroData);
            state.SetOpenedState();
        }

        public void RecoverDroneCount()
        {
            _currentDroneCount = _maxDroneCount;
        }

        public void UpdateEarnedMoneyByReward(int earnedMoney)
        {
            _moneyEarned = earnedMoney;
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

        public void FinishGame()
        {
            IsGameEnded = true;
            _persistentDataService.PlayerProgress.CurrentLevel++;
            _persistentDataService.PlayerProgress.CurrentLevelId++;
            SceneManager.LoadScene(_upgradeSceneName);
        }
    }
}