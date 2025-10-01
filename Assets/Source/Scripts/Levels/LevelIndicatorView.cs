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
        [SerializeField] private Color _selectColor;
        [SerializeField] private Color _defaultColor;
        [SerializeField] private Color _completeColor;

        public void Initialize(LevelData levelData, LevelState levelState, int currentLevelId)
        {
            SetColor(_defaultColor);
            SetState(levelState, currentLevelId);
            SetSpecialIcon(levelData);
        }

        private void SetState(LevelState levelState, int currentLevelId)
        {
            if (levelState.Id == currentLevelId)
                SetColor(_selectColor);

            if (levelState.IsComplete)
                SetColor(_completeColor);
        }

        private void SetColor(Color color)
        {
            _mainImage.color = color;
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