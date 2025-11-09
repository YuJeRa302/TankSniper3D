using Assets.Source.Game.Scripts.States;
using Assets.Source.Scripts.ScriptableObjects;
using Assets.Source.Scripts.Services;
using Assets.Source.Scripts.Utility;
using System.Collections.Generic;
using UnityEngine;
using YG;

namespace Assets.Source.Scripts.Saves
{
    public class SaveAndLoader
    {
        private readonly PersistentDataService _persistentDataService;
        private readonly ConfigData _configData;
        private readonly int _defaultIntValue = 0;

        public SaveAndLoader(PersistentDataService persistentDataService, ConfigData configData)
        {
            _persistentDataService = persistentDataService;
            _configData = configData;
        }

        public bool TryGetGameData()
        {
            return YG2.saves.HasSave;
        }

        public void LoadDataFromCloud()
        {
            _persistentDataService.PlayerProgress.AmbientVolume = YG2.saves.AmbientVolume;
            _persistentDataService.PlayerProgress.SfxVolume = YG2.saves.SfxVolumeVolume;
            _persistentDataService.PlayerProgress.Score = YG2.saves.Score;
            _persistentDataService.PlayerProgress.Money = YG2.saves.Money;
            _persistentDataService.PlayerProgress.IsVibro = YG2.saves.IsVibro;
            _persistentDataService.PlayerProgress.IsMuted = YG2.saves.IsMuted;
            _persistentDataService.PlayerProgress.CurrentBiomId = YG2.saves.CurrentBiomId;
            _persistentDataService.PlayerProgress.CurrentLevel = YG2.saves.CurrentLevel;
            _persistentDataService.PlayerProgress.CurrentLevelId = YG2.saves.CurrentLevelId;
            _persistentDataService.PlayerProgress.CurrentPlayerTankId = YG2.saves.CurrentPlayerTankId;
            _persistentDataService.PlayerProgress.CurrentGridTankCost = YG2.saves.CurrentGridTankCost;
            _persistentDataService.PlayerProgress.CountBuyedGridTank = YG2.saves.CountBuyedGridTank;
            _persistentDataService.PlayerProgress.CurrentGridTankLevel = YG2.saves.CurrentGridTankLevel;
            _persistentDataService.PlayerProgress.LevelService.SetStates(YG2.saves.LevelStates);
            _persistentDataService.PlayerProgress.HeroService.SetStates(YG2.saves.HeroStates);
            _persistentDataService.PlayerProgress.TankService.SetStates(YG2.saves.TankStates);
            _persistentDataService.PlayerProgress.DecorationService.SetStates(YG2.saves.DecorationStates);
            _persistentDataService.PlayerProgress.GridService.SetStates(YG2.saves.GridTankStates);
        }

        public void LoadDataFromConfig()
        {
            _persistentDataService.PlayerProgress.AmbientVolume = _configData.AmbientVolume;
            _persistentDataService.PlayerProgress.SfxVolume = _configData.SfxVolumeVolume;
            _persistentDataService.PlayerProgress.Score = _defaultIntValue;
            _persistentDataService.PlayerProgress.Money = _configData.Money;
            _persistentDataService.PlayerProgress.IsMuted = _configData.IsMuted;
            _persistentDataService.PlayerProgress.IsVibro = _configData.IsVibro;
            _persistentDataService.PlayerProgress.CurrentPlayerTankId = _configData.CurrentPlayerTankId;
            _persistentDataService.PlayerProgress.CurrentBiomId = _configData.CurrentBiomId;
            _persistentDataService.PlayerProgress.CurrentLevelId = _configData.CurrentLevelId;
            _persistentDataService.PlayerProgress.CurrentLevel = _configData.CurrentLevel;
            _persistentDataService.PlayerProgress.CurrentGridTankCost = _configData.CurrentGridTankCost;
            _persistentDataService.PlayerProgress.CountBuyedGridTank = _configData.CountBuyedGridTank;
            _persistentDataService.PlayerProgress.CurrentGridTankLevel = _configData.CurrentGridTankLevel;
            _persistentDataService.PlayerProgress.LevelService.SetStates(_configData.LevelStates);
            _persistentDataService.PlayerProgress.HeroService.SetStates(_configData.HeroStates);
            _persistentDataService.PlayerProgress.TankService.SetStates(_configData.TankStates);
            _persistentDataService.PlayerProgress.DecorationService.SetStates(_configData.DecorationStates);
            _persistentDataService.PlayerProgress.GridService.SetStates(_configData.GridTankStates);
        }

        public void LoadDataFromPrefs()
        {
            if (PlayerPrefs.HasKey(GameConstants.PlayerProgressDataKey))
            {
                var jsonString = PlayerPrefs.GetString(GameConstants.PlayerProgressDataKey);
                JsonUtility.FromJsonOverwrite(jsonString, _persistentDataService.PlayerProgress);
                Debug.Log(jsonString);
            }
        }

        public void SaveDataToPrefs()
        {
            var jsonString = JsonUtility.ToJson(_persistentDataService.PlayerProgress);
            Debug.Log(jsonString);
            PlayerPrefs.SetString(GameConstants.PlayerProgressDataKey, jsonString);
            PlayerPrefs.Save();
        }

        public void SaveGameProgerss(int money, int biomId, int levelId, bool isComplete)
        {
            _persistentDataService.PlayerProgress.Money += money;
            _persistentDataService.PlayerProgress.LevelService.AddLevelState(levelId, biomId, isComplete);
            _persistentDataService.PlayerProgress.CurrentLevelId++;
            SaveData();
        }

        public void SaveData()
        {
            var newSaveData = new SavesYG
            {
                Money = _persistentDataService.PlayerProgress.Money,
                AmbientVolume = _persistentDataService.PlayerProgress.AmbientVolume,
                SfxVolumeVolume = _persistentDataService.PlayerProgress.SfxVolume,
                IsMuted = _persistentDataService.PlayerProgress.IsMuted,
                IsVibro = _persistentDataService.PlayerProgress.IsVibro,
                Level = _persistentDataService.PlayerProgress.CurrentLevel,
                Score = _persistentDataService.PlayerProgress.Score,
                CurrentBiomId = _persistentDataService.PlayerProgress.CurrentBiomId,
                CurrentLevelId = _persistentDataService.PlayerProgress.CurrentLevelId,
                CurrentPlayerTankId = _persistentDataService.PlayerProgress.CurrentPlayerTankId,
                CurrentGridTankCost = _persistentDataService.PlayerProgress.CurrentGridTankCost,
                CountBuyedGridTank = _persistentDataService.PlayerProgress.CountBuyedGridTank,
                HasSave = true,

                TankStates = new List<TankState>(
                    _persistentDataService.PlayerProgress.TankService.TankStates),

                DecorationStates = new List<DecorationState>(
                    _persistentDataService.PlayerProgress.DecorationService.DecorationStates),

                HeroStates = new List<HeroState>(
                    _persistentDataService.PlayerProgress.HeroService.HeroStates),

                LevelStates = new List<LevelState>(
                    _persistentDataService.PlayerProgress.LevelService.LevelStates),

                GridTankStates = new List<GridTankState>(
                    _persistentDataService.PlayerProgress.GridService.GridTankStates)
            };

            string oldDataJson = JsonUtility.ToJson(YG2.saves);
            string newDatatJson = JsonUtility.ToJson(newSaveData);

            if (oldDataJson != newDatatJson)
            {
                YG2.saves = newSaveData;
                YG2.SaveProgress();
            }
        }
    }
}