using Assets.Source.Game.Scripts.States;
using Assets.Source.Scripts.Models;
using Assets.Source.Scripts.ScriptableObjects;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Scripts.Levels
{
    public class LevelsView : MonoBehaviour
    {
        [SerializeField] private Image _firstBiomImage;
        [SerializeField] private Image _secondBiomImage;
        [SerializeField] private Transform _levelIndicatorContainer;
        [SerializeField] private LevelIndicatorView _levelIndicatorView;

        private List<LevelIndicatorView> _levelIndicatorViews = new();
        private BiomsConfig _biomsConfig;
        private LevelModel _levelModel;

        private void OnDestroy()
        {
            ClearLevelIndicators();
        }

        public void Initialize(LevelModel levelModel, BiomsConfig biomsConfig)
        {
            _levelModel = levelModel;
            _biomsConfig = biomsConfig;
            SetBiomIcons();
            CreateLevelIndicators();
        }

        private void SetBiomIcons()
        {
            _firstBiomImage.sprite = _biomsConfig.GetBiomDataById(_levelModel.GetCurrentBiomIndex()).Icon;
            _secondBiomImage.sprite = _biomsConfig.GetBiomDataById(_levelModel.GetNextBiomIndex()).Icon;
        }

        private void CreateLevelIndicators()
        {
            foreach (LevelData levelData in _biomsConfig.BiomDatas[_levelModel.GetCurrentBiomIndex()].LevelDatas)
            {
                LevelIndicatorView view = Instantiate(_levelIndicatorView, _levelIndicatorContainer);
                _levelIndicatorViews.Add(view);
                LevelState levelState = _levelModel.GetLevelState(levelData);
                view.Initialize(levelData, levelState, _levelModel.GetCurrentLevelId());
            }
        }

        private void ClearLevelIndicators()
        {
            if (_levelIndicatorViews.Count == 0)
                return;

            foreach (LevelIndicatorView view in _levelIndicatorViews)
            {
                Destroy(view.gameObject);
            }

            _levelIndicatorViews.Clear();
        }
    }
}