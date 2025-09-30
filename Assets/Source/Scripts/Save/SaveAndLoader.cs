using Assets.Source.Game.Scripts.States;
using Assets.Source.Scripts.ScriptableObjects;
using Assets.Source.Scripts.Services;
using Assets.Source.Scripts.Utility;
using System;
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

        public SaveAndLoader(PersistentDataService persistentDataService)
        {
            _persistentDataService = persistentDataService;
        }

        public bool TryGetGameData()
        {
            return YG2.saves.HasSave;
        }

        public void LoadDataFromConfig()
        {
            _persistentDataService.PlayerProgress.AmbientVolume = _configData.AmbientVolume;
            _persistentDataService.PlayerProgress.SfxVolume = _configData.SfxVolumeVolume;
            _persistentDataService.PlayerProgress.Score = _defaultIntValue;
            _persistentDataService.PlayerProgress.Money = _configData.Money;
            _persistentDataService.PlayerProgress.IsMuted = _configData.IsMuted;
            _persistentDataService.PlayerProgress.CurrentPlayerTankId = _configData.CurrentPlayerTankId;
            _persistentDataService.PlayerProgress.CurrentBiomId = _configData.CurrentBiomId;
            _persistentDataService.PlayerProgress.CurrentLevelId = _configData.CurrentLevelId;
            _persistentDataService.PlayerProgress.CurrentLevel = _configData.CurrentLevel;
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

        //    public void SaveGameProgerss(
        //int score,
        //int coins,
        //int upgradePoints,
        //int levelId,
        //bool isComplete,
        //bool isGameInterrupted)
        //    {
        //        if (isGameInterrupted)
        //            return;

        //        _persistentDataService.PlayerProgress.Score += score;
        //        _persistentDataService.PlayerProgress.Coins += coins;
        //        _persistentDataService.PlayerProgress.UpgradePoints += upgradePoints;
        //        _persistentDataService.PlayerProgress.LevelService.AddLevelState(levelId, isComplete);
        //        SaveData();
        //    }

        //    public void SaveData()
        //    {
        //        var newSaveData = new SavesYG
        //        {
        //            Coins = _persistentDataService.PlayerProgress.Coins,
        //            AmbientVolume = _persistentDataService.PlayerProgress.AmbientVolume,
        //            SfxVolumeVolume = _persistentDataService.PlayerProgress.SfxVolume,
        //            IsMuted = _persistentDataService.PlayerProgress.IsMuted,
        //            UpgradePoints = _persistentDataService.PlayerProgress.UpgradePoints,

        //            UpgradeStates = new List<UpgradeState>(
        //                _persistentDataService.PlayerProgress.UpgradeService.UpgradeStates),

        //            ClassAbilityStates = new List<ClassAbilityState>(
        //                _persistentDataService.PlayerProgress.ClassAbilityService.ClassAbilityStates),

        //            DefaultWeaponState = new WeaponState[
        //            _persistentDataService.PlayerProgress.WeaponService.WeaponStates.Count],

        //            DefaultLevelState = new LevelState[
        //            _persistentDataService.PlayerProgress.LevelService.LevelStates.Count],

        //            Score = _persistentDataService.PlayerProgress.Score,
        //            HasSave = true
        //        };

        //        Array.Copy(
        //            _persistentDataService.PlayerProgress.WeaponService.WeaponStates.ToArray(),
        //            newSaveData.DefaultWeaponState, newSaveData.DefaultWeaponState.Length);

        //        Array.Copy(
        //            _persistentDataService.PlayerProgress.LevelService.LevelStates.ToArray(),
        //            newSaveData.DefaultLevelState, newSaveData.DefaultLevelState.Length);

        //        string oldDataJson = JsonUtility.ToJson(YG2.saves);
        //        string newDatatJson = JsonUtility.ToJson(newSaveData);

        //        if (oldDataJson != newDatatJson)
        //        {
        //            YG2.saves = newSaveData;
        //            YG2.SaveProgress();
        //        }
        //    }
    }
}