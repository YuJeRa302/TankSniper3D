using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Game.Scripts.Utility;
using Assets.Source.Scripts.Models;
using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Scripts.Game
{
    public class CompleteLevelTab : EndGameTabView
    {
        private readonly float _transferDuration = 1.5f;
        private readonly float _iconFlyDelay = 0.1f;
        private readonly int _iconCount = 10;

        [SerializeField] private TMP_Text _moneyText;
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private TMP_Text _moneyEarnedText;
        [Space(20)]
        [SerializeField] private Button _continueButton;
        [Space(20)]
        [SerializeField] private Transform _rewardOrigin;
        [SerializeField] private Transform _moneyTarget;
        [Space(20)]
        [SerializeField] private GameObject moneyIconPrefab;

        private int _moneyEarned;
        private int _currentMoney;

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
            _currentMoney = GameModel.GetMoney();
            _moneyEarned = GameModel.GetEarnedMoney();
            _levelText.text = "Уровень " + GameModel.GetLevel().ToString();
            _moneyText.text = _currentMoney.ToString();
            _moneyEarnedText.text = "Награда: " + _moneyEarned.ToString();
        }

        private void OnContinuePressed()
        {
            StartCoroutine(TransferMoney());
        }

        private IEnumerator TransferMoney()
        {
            int startReward = _moneyEarned;
            int targetMoney = _currentMoney + _moneyEarned;

            for (int i = 0; i < _iconCount; i++)
            {
                SpawnFlyingIcon();
                yield return new WaitForSeconds(_iconFlyDelay);
            }

            DOTween.To(() => startReward, x =>
            {
                startReward = x;
                _moneyEarnedText.text = $"Награда: {startReward}";
            }, 0, _transferDuration).SetEase(Ease.OutCubic);

            DOTween.To(() => _currentMoney, x =>
            {
                _currentMoney = x;
                _moneyText.text = $"{_currentMoney}";
            }, targetMoney, _transferDuration).SetEase(Ease.OutCubic);

            yield return new WaitForSeconds(_transferDuration);

            GameModel.FinishGame();
        }

        private void SpawnFlyingIcon()
        {
            GameObject icon = Instantiate(moneyIconPrefab, _rewardOrigin.position, Quaternion.identity, _rewardOrigin.parent);

            Vector3 startPos = _rewardOrigin.position + new Vector3(Random.Range(-40f, 40f), Random.Range(-20f, 20f), 0);
            icon.transform.position = startPos;

            float duration = Random.Range(0.8f, 1.2f);
            icon.transform.DOMove(_moneyTarget.position, duration)
                .SetEase(Ease.InQuad)
                .OnComplete(() =>
                {
                    Destroy(icon);
                });

            icon.transform.DOScale(0.3f, duration).SetEase(Ease.InQuad);
        }
    }
}