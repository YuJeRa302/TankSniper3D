using DG.Tweening;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Source.Scripts.Game
{
    public class CrosshairButtonView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private readonly int _loopTweenValue = -1;
        private readonly float _stopDurationValue = 0.3f;
        private readonly float _multiplier = 2.0f;

        [SerializeField] private RectTransform _crosshairIcon;
        [SerializeField] private float _moveDistance = 10f;
        [SerializeField] private float _moveTime = 1f;
        [SerializeField] private float _delayBetweenMoves = 0.2f;

        private CompositeDisposable _disposables = new();
        private Sequence _shakeSequence;
        private Vector2 _startPosition;

        public event Action<PointerEventData> ButtonPressed;
        public event Action<PointerEventData> ButtonReleased;

        private void OnDestroy()
        {
            RemoveListeners();
        }

        private void OnEnable()
        {
            AddListeners();
            StartAimShake();
        }

        private void OnDisable()
        {
            StopAimShake();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            ButtonPressed?.Invoke(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            ButtonReleased?.Invoke(eventData);
        }

        private void AddListeners()
        {
            DefeatTab.Message
                .Receive<M_RecoverPlayer>()
                .Subscribe(m => OnRecoverPlayer())
                .AddTo(_disposables);

            MessageBroker.Default
                .Receive<M_FinishGame>()
                .Subscribe(m => OnTankDeath())
                .AddTo(_disposables);
        }

        private void RemoveListeners()
        {
            _disposables?.Dispose();
        }

        private void OnRecoverPlayer()
        {
            gameObject.SetActive(true);
        }

        private void OnTankDeath()
        {
            gameObject.SetActive(false);
        }

        private void StartAimShake()
        {
            _startPosition = _crosshairIcon.anchoredPosition;
            _shakeSequence = DOTween.Sequence();
            _shakeSequence.SetLoops(_loopTweenValue);

            SetHorizontalMove();
            _shakeSequence.AppendInterval(_delayBetweenMoves);
            SetVerticalMove();
            _shakeSequence.AppendInterval(_delayBetweenMoves);

            _shakeSequence.Play();
        }

        private void SetHorizontalMove()
        {
            _shakeSequence.Append(
                _crosshairIcon.DOAnchorPosX(_startPosition.x + _moveDistance, _moveTime)
                    .SetEase(Ease.InOutSine)
            );

            _shakeSequence.Append(
                _crosshairIcon.DOAnchorPosX(_startPosition.x - _moveDistance, _moveTime * _multiplier)
                    .SetEase(Ease.InOutSine)
            );

            _shakeSequence.Append(
                _crosshairIcon.DOAnchorPosX(_startPosition.x, _moveTime)
                    .SetEase(Ease.InOutSine)
            );
        }

        private void SetVerticalMove()
        {
            _shakeSequence.Append(
                _crosshairIcon.DOAnchorPosY(_startPosition.y + _moveDistance, _moveTime)
                    .SetEase(Ease.InOutSine)
            );

            _shakeSequence.Append(
                _crosshairIcon.DOAnchorPosY(_startPosition.y - _moveDistance, _moveTime * _multiplier)
                    .SetEase(Ease.InOutSine)
            );

            _shakeSequence.Append(
                _crosshairIcon.DOAnchorPosY(_startPosition.y, _moveTime)
                    .SetEase(Ease.InOutSine)
            );
        }

        private void StopAimShake()
        {
            if (_shakeSequence != null && _shakeSequence.IsActive())
                _shakeSequence.Kill();

            _crosshairIcon.DOAnchorPos(_startPosition, _stopDurationValue).SetEase(Ease.OutSine);
        }
    }
}