using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Game.Scripts.Utility;
using Assets.Source.Scripts.Models;
using Assets.Source.Scripts.Upgrades;
using System.Collections;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Scripts.Game
{
    public class DefeatTab : EndGameTabView
    {
        private readonly float _fillValue = 1.0f;

        [Header("UI Elements")]
        [SerializeField] private Image _timerCircle;
        [SerializeField] private TMP_Text _timerText;
        [SerializeField] private TMP_Text _moneyEarnedText;
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _reloadButton;
        [SerializeField] private Button _radialButton;
        [Header("Timer Settings")]
        [SerializeField] private float _totalTime = 5f;
        [Space(20)]
        [SerializeField] private GameButtonAdsWaiter _gameButtonAdsWaiter;

        private float _currentTime;
        private Coroutine _timerCoroutine;

        public override TypeReward TypeReward => TypeReward.Default;

        public override void Open()
        {
            gameObject.SetActive(true);
            _gameButtonAdsWaiter.Initialize(TypeReward);
            _moneyEarnedText.text = "Награда: " + GameModel.GetEarnedMoney().ToString();
            Message.Publish(new M_OpenPanel());
            StartTimer();
        }

        protected override void AddListeners()
        {
            _reloadButton.onClick.AddListener(OnSceneReloaded);
            _gameButtonAdsWaiter.AdsOpened += OnAdsGetted;
            _radialButton.onClick.AddListener(OnRadialButtonClicked);
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
            _gameButtonAdsWaiter.AdsOpened -= OnAdsGetted;
            _reloadButton.onClick.RemoveListener(OnSceneReloaded);
            _radialButton.onClick.RemoveListener(OnRadialButtonClicked);
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

        private void OnAdsGetted(TypeReward typeReward)
        {
            if (GameModel.IsGameEnded)
                return;

            if (typeReward != TypeReward)
                return;

            base.OnCloseFullscreenAdCallback();
            Message.Publish(new M_RecoverPlayer());
            Close();
        }

        private void OnRadialButtonClicked()
        {
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
    }
}