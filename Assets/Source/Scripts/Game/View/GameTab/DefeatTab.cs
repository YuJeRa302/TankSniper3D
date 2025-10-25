using Assets.Source.Scripts.Models;
using Assets.Source.Scripts.Upgrades;
using System.Collections;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Scripts.Game
{
    public class DefeatTab : GameTabView
    {
        [Header("UI Elements")]
        [SerializeField] private Image _timerCircle;
        [SerializeField] private TextMeshProUGUI _timerText;
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _reloadButton;

        [Header("Timer Settings")]
        [SerializeField] private float _totalTime = 5f;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color warningColor = Color.red;

        private float _currentTime;
        private GameModel _gameModel;
        private Coroutine _timerCoroutine;
        private CompositeDisposable _disposables = new();

        public void Initialize(GameModel gameModel)
        {
            _gameModel = gameModel;
            gameObject.SetActive(false);
        }

        public override void Open()
        {
            gameObject.SetActive(true);
            StartTimer();
        }

        protected override void AddListeners()
        {
            base.AddListeners();
            _reloadButton.onClick.AddListener(OnSceneReloaded);
            _continueButton.onClick.AddListener(OnContinuePressed);

            TankHealth.Message
                .Receive<M_DeathTank>()
                .Subscribe(m => Open())
                .AddTo(_disposables);
        }

        protected override void RemoveListeners()
        {
            base.RemoveListeners();
            _reloadButton.onClick.RemoveListener(OnSceneReloaded);
            _continueButton.onClick.RemoveListener(OnContinuePressed);
            _disposables?.Dispose();
        }

        protected override void OnCloseFullscreenAdCallback()
        {
            base.OnCloseFullscreenAdCallback();
            Message.Publish(new M_RecoveryTankHealth());
            Close();
        }

        private void StartTimer()
        {
            _currentTime = _totalTime;
            _timerCircle.fillAmount = 1f;
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

                if (_currentTime < 1.5f)
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
            _gameModel.ReloadScene();
            MessageBroker.Default.Publish(new M_ReloadLevel());
        }

        private void OnContinuePressed()
        {
            OpenFullscreenAds();
        }
    }
}