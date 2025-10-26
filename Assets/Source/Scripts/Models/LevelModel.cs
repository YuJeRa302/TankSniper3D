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
            return _persistentDataService.PlayerProgress.CurrentBiomId;
        }

        public int GetNextBiomIndex()
        {
            int nextBiomIndex = GetCurrentBiomIndex() + 1;

            if (nextBiomIndex > _biomsConfig.BiomDatas.Count)
                nextBiomIndex = 0;

            return nextBiomIndex;
        }

        public int GetCurrentLevelId()
        {
            return _persistentDataService.PlayerProgress.CurrentLevelId;
        }

        public bool TryGetUnopenedHero()
        {
            if (_persistentDataService.PlayerProgress.HeroService.HeroStates.Count > 0)
            {
                foreach (var heroState in _persistentDataService.PlayerProgress.HeroService.HeroStates)
                {
                    if (heroState.IsOpened != true)
                        return true;
                }

                return false;
            }

            return false;
        }

        public LevelState GetLevelState(LevelData levelData)
        {
            return _persistentDataService.PlayerProgress.LevelService.GetState(levelData, GetCurrentBiomIndex());
        }

        private void UpdateIndexes()
        {
            var currentLevelIndex = GetCurrentLevelId();

            if (currentLevelIndex > _biomsConfig.BiomDatas[GetCurrentBiomIndex()].LevelDatas.Count)
            {
                _persistentDataService.PlayerProgress.CurrentLevelId = 0;
                _persistentDataService.PlayerProgress.CurrentBiomId++;
            }
        }
    }
}