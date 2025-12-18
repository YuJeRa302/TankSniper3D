using Assets.Source.Game.Scripts.States;
using Assets.Source.Scripts.ScriptableObjects;
using Assets.Source.Scripts.Services;

namespace Assets.Source.Scripts.Models
{
    public class LevelModel
    {
        private readonly PersistentDataService _persistentDataService;
        private readonly BiomsConfig _biomsConfig;

        public LevelModel(PersistentDataService persistentDataService, BiomsConfig biomsConfig)
        {
            _persistentDataService = persistentDataService;
            _biomsConfig = biomsConfig;
            UpdateIndexes();
        }

        public int GetCurrentBiomIndex()
        {
            if (_persistentDataService.PlayerProgress.CurrentBiomId >= _biomsConfig.BiomDatas.Count)
                _persistentDataService.PlayerProgress.CurrentBiomId = 0;

            return _persistentDataService.PlayerProgress.CurrentBiomId;
        }

        public int GetNextBiomIndex()
        {
            int nextBiomIndex = GetCurrentBiomIndex() + 1;

            if (nextBiomIndex >= _biomsConfig.BiomDatas.Count)
                nextBiomIndex = 0;

            return nextBiomIndex;
        }

        public int GetCurrentLevelId()
        {
            return _persistentDataService.PlayerProgress.CurrentLevelId;
        }

        public LevelState GetLevelState(LevelData levelData)
        {
            return _persistentDataService.PlayerProgress.LevelService.GetState(levelData, GetCurrentBiomIndex());
        }

        private void UpdateIndexes()
        {
            var currentLevelIndex = GetCurrentLevelId();

            if (currentLevelIndex >= _biomsConfig.BiomDatas[GetCurrentBiomIndex()].LevelDatas.Count)
            {
                _persistentDataService.PlayerProgress.CurrentLevelId = 0;
                _persistentDataService.PlayerProgress.LevelService.ResetLevelStates(_persistentDataService.PlayerProgress.CurrentBiomId);
                _persistentDataService.PlayerProgress.CurrentBiomId++;
            }
        }
    }
}