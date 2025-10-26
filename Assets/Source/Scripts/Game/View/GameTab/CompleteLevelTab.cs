using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Scripts.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Scripts.Game
{
    public class CompleteLevelTab : EndGameTabView
    {

        [SerializeField] private TMP_Text _moneyText;
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private TMP_Text _moneyEarnedText;
        [Space(20)]
        [SerializeField] private Button _continueButton;

        public override TypeReward TypeReward => TypeReward.Default;

        public override void Open()
        {
            if (TryExecute(LevelData))
                return;

            base.Open();
            Fill();
        }

        protected override void AddListeners()
        {
            base.AddListeners();
            _continueButton.onClick.AddListener(OnContinuePressed);
        }

        protected override void RemoveListeners()
        {
            base.RemoveListeners();
            _continueButton.onClick.RemoveListener(OnContinuePressed);
        }

        private void Fill()
        {
            _levelText.text = "Уровень " + GameModel.GetLevel().ToString();
            _moneyText.text = GameModel.GetMoney().ToString();
            _moneyEarnedText.text = "Награда: " + GameModel.GetEarnedMoney().ToString();
        }

        private void OnContinuePressed()
        {
            GameModel.FinishGame();
        }
    }
}