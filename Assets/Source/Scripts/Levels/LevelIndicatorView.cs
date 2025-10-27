using Assets.Source.Game.Scripts.States;
using Assets.Source.Scripts.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Scripts.Levels
{
    public class LevelIndicatorView : MonoBehaviour
    {
        [SerializeField] private Image _mainImage;
        [SerializeField] private Image _specialImage;
        [SerializeField] private Sprite _selectSprite;
        [SerializeField] private Sprite _defaultSprite;
        [SerializeField] private Sprite _completeSprite;

        public void Initialize(LevelData levelData, LevelState levelState, int currentLevelId)
        {
            SetSprite(_defaultSprite);
            SetState(levelState, currentLevelId);
            SetSpecialIcon(levelData);
        }

        private void SetState(LevelState levelState, int currentLevelId)
        {
            if (levelState.Id == currentLevelId)
                SetSprite(_selectSprite);

            if (levelState.IsComplete)
                SetSprite(_completeSprite);
        }

        private void SetSprite(Sprite sprite)
        {
            _mainImage.sprite = sprite;
        }

        private void SetSpecialIcon(LevelData levelData)
        {
            if (levelData.SpecialSprite == null)
                return;

            _specialImage.gameObject.SetActive(true);
            _specialImage.sprite = levelData.SpecialSprite;
        }
    }
}