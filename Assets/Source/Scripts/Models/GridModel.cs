using Assets.Source.Game.Scripts.States;
using Assets.Source.Game.Scripts.Utility;
using Assets.Source.Scripts.Saves;
using Assets.Source.Scripts.ScriptableObjects;
using Assets.Source.Scripts.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Source.Scripts.Models
{
    public class GridModel
    {
        private readonly BiomsConfig _biomsConfig;
        private readonly CoroutineRunner _coroutineRunner;
        private readonly SaveAndLoader _saveAndLoader;
        private readonly PersistentDataService _persistentDataService;
        private readonly int _addMoneyButtonValue = 5000;
        private readonly int _maxCountBuyTank = 8;
        private readonly float _loadControlValue = 0.9f;
        private readonly int _defaultGridTankCost = 500;

        private AsyncOperation _load;
        private int _currentCountBuyTank = 0;
        private TankState _currentTankState;

        public GridModel(
            PersistentDataService persistentDataService,
            CoroutineRunner coroutineRunner,
            SaveAndLoader saveAndLoader,
            BiomsConfig biomsConfig)
        {
            _persistentDataService = persistentDataService;
            _coroutineRunner = coroutineRunner;
            _saveAndLoader = saveAndLoader;
            _biomsConfig = biomsConfig;
            _currentTankState = _persistentDataService.PlayerProgress.TankService.GetStateByEquip();
        }

        public int CurrentMainTankLevel { get; private set; } = 1;
        public int CurrentGridTankLevel { get; private set; } = 1;

        public void IncreaseMainTankLevel(int currentLevel)
        {
            if (currentLevel > CurrentMainTankLevel)
                CurrentMainTankLevel++;
        }

        public int GetCurrentCountMoney()
        {
            return _persistentDataService.PlayerProgress.Money;
        }

        public int GetMaxTankCountForBuy()
        {
            return _maxCountBuyTank;
        }

        public int GetCountBuyedTanks()
        {
            return _persistentDataService.PlayerProgress.CountBuyedGridTank;
        }

        public int GetCurrentTankCost()
        {
            if (_persistentDataService.PlayerProgress.CurrentGridTankCost == 0)
                _persistentDataService.PlayerProgress.CurrentGridTankCost = _defaultGridTankCost;

            return _persistentDataService.PlayerProgress.CurrentGridTankCost;
        }

        public bool TryBuyGridTank(int coast)
        {
            if (_persistentDataService.TrySpendMoney(coast))
                return true;

            return false;
        }

        public GridTankState GetGridTankStateByIndex(int index)
        {
            return _persistentDataService.PlayerProgress.GridService.GetGridTankStateByIndex(index);
        }

        public List<GridTankState> GetGridTankStates()
        {
            return _persistentDataService.PlayerProgress.GridService.GridTankStates;
        }

        public TankState GetTankStateByEquip()
        {
            return _persistentDataService.PlayerProgress.TankService.GetStateByEquip();
        }

        public TankState GetTankStateByData(TankData tankData)
        {
            return _persistentDataService.PlayerProgress.TankService.GetState(tankData);
        }

        public GridTankState CreateGridTankState(GridTankData gridTankData)
        {
            return _persistentDataService.PlayerProgress.GridService.CreateState(gridTankData);
        }

        public void RemoveGridTankStateByMerge(GridTankState gridTankState)
        {
            _persistentDataService.PlayerProgress.GridService.RemoveGridTankStateByMerge(gridTankState);
        }

        public void AddMoney()
        {
            _persistentDataService.PlayerProgress.Money += _addMoneyButtonValue;
        }

        public void UpdateCurrentCountBuyTank()
        {
            _currentCountBuyTank++;
            _persistentDataService.PlayerProgress.CountBuyedGridTank = _currentCountBuyTank;
        }

        public void ChangeEquippedTank(TankState tankState)
        {
            _currentTankState?.ChangeEquippedState(false);
            _currentTankState = tankState;
            _currentTankState.ChangeEquippedState(true);
            _currentTankState.ChangeOpenState(true);
        }

        public void LoadGameScene()
        {
            _saveAndLoader.SaveDataToPrefs();
            _coroutineRunner.StartCoroutine(LoadLevel(SceneManager.LoadSceneAsync(GetCurrentLevelData().NameScene)));
        }

        public void UpdateGridTankLevel()
        {
            if (_currentCountBuyTank == _maxCountBuyTank)
            {
                CurrentGridTankLevel++;
                _persistentDataService.PlayerProgress.CurrentGridTankCost += _defaultGridTankCost;
                _currentCountBuyTank = 0;
                _persistentDataService.PlayerProgress.CountBuyedGridTank = _currentCountBuyTank;
            }
        }

        private LevelData GetCurrentLevelData()
        {
            return _biomsConfig
                .GetBiomDataById(_persistentDataService.PlayerProgress.CurrentBiomId)
                .GetLevelDataById(_persistentDataService.PlayerProgress.CurrentLevelId);
        }

        private IEnumerator LoadLevel(AsyncOperation asyncOperation)
        {
            if (_load != null)
                yield break;

            _load = asyncOperation;
            _load.allowSceneActivation = false;

            while (_load.progress < _loadControlValue)
            {
                yield return null;
            }

            _load.allowSceneActivation = true;
            _load = null;
        }
    }
}
