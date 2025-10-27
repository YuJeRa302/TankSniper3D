using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Scripts.Models;
using Assets.Source.Scripts.Upgrades;
using System.Collections;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace Assets.Source.Scripts.Game
{
    public class DefeatTab : EndGameTabView
    {
        private readonly float _fillValue = 1.0f;
        private readonly float _controlTimerValue = 1.5f;

        [Header("UI Elements")]
        [SerializeField] private Image _timerCircle;
        [SerializeField] private TMP_Text _timerText;
        [SerializeField] private TMP_Text _moneyEarnedText;
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _reloadButton;

        [Header("Timer Settings")]
        [SerializeField] private float _totalTime = 5f;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color warningColor = Color.red;

        private float _currentTime;
        private Coroutine _timerCoroutine;

        public override TypeReward TypeReward => TypeReward.Default;

        public override void Open()
        {
            gameObject.SetActive(true);
            _moneyEarnedText.text = "Награда: " + GameModel.GetEarnedMoney().ToString();
            Message.Publish(new M_OpenPanel());
            StartTimer();
        }

        protected override void AddListeners()
        {
            YG2.onOpenInterAdv += OnOpenFullscreenAdCallback;
            YG2.onCloseInterAdv += OnCloseFullscreenAdCallback;
            YG2.onErrorInterAdv += OnErrorFullAdCallback;
            _reloadButton.onClick.AddListener(OnSceneReloaded);
            _continueButton.onClick.AddListener(OnContinuePressed);

            TankHealth.Message
                .Receive<M_DeathTank>()
                .Subscribe(m => Open())
                .AddTo(Disposable);

            GamePanelView.Message
                .Receive<M_DefeatByDrone>()
                .Subscribe(m => Open())
                .AddTo(Disposable);
        }

        protected override void RemoveListeners()
        {
            YG2.onOpenInterAdv -= OnOpenFullscreenAdCallback;
            YG2.onCloseInterAdv -= OnCloseFullscreenAdCallback;
            YG2.onErrorInterAdv -= OnErrorFullAdCallback;
            _reloadButton.onClick.RemoveListener(OnSceneReloaded);
            _continueButton.onClick.RemoveListener(OnContinuePressed);
            Disposable?.Dispose();
        }

        protected override void OnCloseFullscreenAdCallback()
        {
            if (GameModel.IsGameEnded)
                return;

            base.OnCloseFullscreenAdCallback();
            Message.Publish(new M_RecoverPlayer());
            Close();
        }

        private void StartTimer()
        {
            _currentTime = _totalTime;
            _timerCircle.fillAmount = _fillValue;
            _timerText.text = Mathf.CeilToInt(_currentTime).ToString();

            if (_timerCoroutine != null)
                StopCoroutine(_timerCoroutine);

            _timerCoroutine = StartCoroutine(UpdateTimer());
        }

        private IEnumerator UpdateTimer()
        {
            while (_currentTime > 0f)
            {
                _currentTime -= Time.deltaTime;
                float normalized = Mathf.Clamp01(_currentTime / _totalTime);

                _timerCircle.fillAmount = normalized;
                _timerText.text = Mathf.CeilToInt(_currentTime).ToString();

                if (_currentTime < _controlTimerValue)
                    _timerCircle.color = warningColor;
                else
                    _timerCircle.color = normalColor;

                yield return null;
            }

            OnTimerFinished();
        }

        private void OnTimerFinished()
        {
            _timerCircle.fillAmount = 0f;
            _timerText.text = "0";
        }

        private void OnSceneReloaded()
        {
            GameModel.ReloadScene();
            MessageBroker.Default.Publish(new M_ReloadLevel());
        }

        private void OnContinuePressed()
        {
            OpenFullscreenAds();
        }
    }
}