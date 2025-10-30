using DG.Tweening;
using UnityEngine;

namespace Assets.Source.Scripts.Game
{
    public class CrosshairAnimation : MonoBehaviour
    {
        private readonly float _stopDurationValue = 0.3f;
        private readonly float _multiplier = 2.0f;

        [SerializeField] private RectTransform _crosshairIcon;
        [SerializeField] private float _moveDistance = 10f;
        [SerializeField] private float _moveTime = 1f;
        [SerializeField] private float _delayBetweenMoves = 0.2f;

        private Sequence _shakeSequence;
        private Vector2 _startPosition;

        private void OnEnable()
        {
            StartAimShake();
        }

        private void OnDisable()
        {
            StopAimShake();
        }

        private void StartAimShake()
        {
            _startPosition = _crosshairIcon.anchoredPosition;
            _shakeSequence = DOTween.Sequence();
            _shakeSequence.SetLoops(-1);

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