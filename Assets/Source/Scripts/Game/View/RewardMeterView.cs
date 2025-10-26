using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Source.Scripts.Game
{
    public class RewardMeterView : MonoBehaviour, IPointerDownHandler
    {
        private readonly float _speedControlValue = 0.01f;
        private readonly float _angleControlValue = 64f;

        [SerializeField] private RectTransform _arrowTransform;
        [Space(20)]
        [SerializeField] private float _rotationSpeed = 200f;
        [SerializeField] private float _maxAngle = 90f;
        [SerializeField] private float _deceleration = 2.5f;
        [Space(20)]
        [SerializeField] private int[] _multipliers;

        private Coroutine _rotationCoroutine;
        private bool _isActiveCoroutine;
        private bool _isRotating = true;
        private float _currentAngle;
        private float _currentSpeed;
        private bool _rotatingRight = true;

        public ReactiveProperty<float> Multiplier { get; } = new ReactiveProperty<float>(0f);

        public void Initialize()
        {
            _isActiveCoroutine = true;

            if (_rotationCoroutine != null)
                StopCoroutine(_rotationCoroutine);

            _rotationCoroutine = StartCoroutine(RotateArrowRoutine());
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_isRotating)
                return;

            _isRotating = false;
        }

        private IEnumerator RotateArrowRoutine()
        {
            while (_isActiveCoroutine)
            {
                if (_isRotating)
                    AnimateArrow();
                else
                    SlowDownArrow();

                yield return null;
            }
        }

        private void AnimateArrow()
        {
            float delta = _rotationSpeed * Time.deltaTime;

            if (_rotatingRight)
                _currentAngle += delta;
            else
                _currentAngle -= delta;

            if (_currentAngle >= _maxAngle)
            {
                _rotatingRight = false;
                _currentAngle = _maxAngle;
            }
            else if (_currentAngle <= -_maxAngle)
            {
                _rotatingRight = true;
                _currentAngle = -_maxAngle;
            }

            _arrowTransform.localRotation = Quaternion.Euler(0, 0, -_currentAngle);
            _currentSpeed = delta;
        }

        private void SlowDownArrow()
        {
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, 0f, _deceleration * Time.deltaTime);

            if (_rotatingRight)
                _currentAngle += _currentSpeed;
            else
                _currentAngle -= _currentSpeed;

            _arrowTransform.localRotation = Quaternion.Euler(0, 0, -_currentAngle);

            if (_currentSpeed <= _speedControlValue)
                GetRewardMultiplier();
        }

        private void GetRewardMultiplier()
        {
            _isActiveCoroutine = false;
            Multiplier.Value = SetMultiplier(_currentAngle);
        }

        private float SetMultiplier(float angle)
        {
            if (angle < -_angleControlValue)
                return _multipliers[0];
            if (angle < 0 && angle > -_angleControlValue)
                return _multipliers[1];
            if (angle > 0 && angle < _angleControlValue)
                return _multipliers[2];
            if (angle > _angleControlValue)
                return _multipliers[3];

            return _multipliers[1];
        }
    }
}