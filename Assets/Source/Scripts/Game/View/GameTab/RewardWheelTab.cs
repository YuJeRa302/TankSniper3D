using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Scripts.Models;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Scripts.Game
{
    public class RewardWheelTab : EndGameTabView
    {
        [SerializeField] private TMP_Text _moneyText;
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private TMP_Text _moneyEarnedText;
        [SerializeField] private TMP_Text _getRewardText;
        [SerializeField] private TMP_Text _continueText;
        [Space(20)]
        [SerializeField] private Button _rewardButton;
        [SerializeField] private Button _continueButton;
        [Space(20)]
        [SerializeField] private RewardMeterView _rewardMeterView;

        private int _finalReward;

        public override TypeReward TypeReward => TypeReward.MoneyWheel;

        public override void Open()
        {
            if (TryExecute(LevelData))
                return;

            base.Open();
            _rewardMeterView.Initialize();
            Fill();
        }

        protected override void AddListeners()
        {
            base.AddListeners();
            _rewardButton.onClick.AddListener(OnRewardOpened);
            _continueButton.onClick.AddListener(OnContinuePressed);

            _rewardMeterView.Multiplier
                .Subscribe(OnGetMultiplier)
                .AddTo(Disposable);
        }

        protected override void RemoveListeners()
        {
            base.RemoveListeners();
            _rewardButton.onClick.RemoveListener(OnRewardOpened);
            _continueButton.onClick.RemoveListener(OnContinuePressed);
        }

        protected override void OnCloseFullscreenAdCallback()
        {
            if (TryExecute(LevelData))
                return;

            base.OnCloseFullscreenAdCallback();
            GameModel.UpdateEarnedMoneyByReward(_finalReward);
            GameModel.FinishGame();
        }

        private void Fill()
        {
            _levelText.text = "Уровень " + GameModel.GetLevel().ToString();
            _moneyText.text = GameModel.GetMoney().ToString();
            _moneyEarnedText.text = "Награда: " + GameModel.GetEarnedMoney().ToString();
            _rewardButton.gameObject.SetActive(false);
            _continueButton.gameObject.SetActive(false);
        }

        private void OnGetMultiplier(float multiplier)
        {
            if (GameModel == null)
                return;

            _finalReward = Mathf.RoundToInt(GameModel.GetMoney() * multiplier);
            _rewardButton.gameObject.SetActive(true);
            _continueButton.gameObject.SetActive(true);
            _continueText.text = GameModel.GetMoney() + " Достаточно";
            _getRewardText.text = "+ " + _finalReward;
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