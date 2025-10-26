using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Scripts.Models;
using Assets.Source.Scripts.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Scripts.Game
{
    public class HeroRewardTab : EndGameTabView
    {
        [SerializeField] private TMP_Text _moneyText;
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private TMP_Text _moneyEarnedText;
        [Space(20)]
        [SerializeField] private Button _rewardButton;
        [SerializeField] private Button _continueButton;
        [Space(20)]
        [SerializeField] private Image _heroImage;

        private HeroData _heroData;

        public override TypeLevel TypeLevel => TypeLevel.Hero;

        public override void Initialize(GameModel gameModel)
        {
            base.Initialize(gameModel);
            _heroData = GameModel.GetNewHeroData();
        }

        public override void Open()
        {
            if (TryOpen(LevelData))
                return;

            base.Open();
            Fill();
        }

        protected override void AddListeners()
        {
            base.AddListeners();
            _rewardButton.onClick.AddListener(OnRewardOpened);
            _continueButton.onClick.AddListener(OnContinuePressed);
        }

        protected override void RemoveListeners()
        {
            base.RemoveListeners();
            _rewardButton.onClick.RemoveListener(OnRewardOpened);
            _continueButton.onClick.RemoveListener(OnContinuePressed);
        }

        protected override void OnCloseFullscreenAdCallback()
        {
            GameModel.OpenHeroByData(_heroData);
            GameModel.FinishGame();
        }

        private void Fill()
        {
            _levelText.text = "Уровень " + GameModel.GetLevel().ToString();
            _moneyText.text = GameModel.GetMoney().ToString();
            _moneyEarnedText.text = "Награда: " + GameModel.GetEarnedMoney().ToString();
            _heroImage.sprite = _heroData.Sprite;
        }

        private void OnRewardOpened()
        {
            OpenFullscreenAds();
        }

        private void OnContinuePressed()
        {
            GameModel.FinishGame();
        }
    }
}