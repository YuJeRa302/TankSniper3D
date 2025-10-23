using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Source.Scripts.Game
{
    public class RewardMeterView : MonoBehaviour, IPointerDownHandler
    {
        private readonly float _speedControlValue = 0.01f;
        private readonly float _angleControlValue = 64f;

        [Header("UI Elements")]
        [SerializeField] private RectTransform _arrowTransform;
        [SerializeField] private TextMeshProUGUI rewardText;
        [Header("Settings")]
        [SerializeField] private float _rotationSpeed = 200f;
        [SerializeField] private float _maxAngle = 90f;
        [SerializeField] private float _deceleration = 2.5f;

        private bool _isRotating = true;
        private bool _isSlowingDown = false;
        private float _currentAngle;
        private float _currentSpeed;
        private bool _rotatingRight = true;
        private int _moneyReward = 0;

        private void Update()
        {
            if (_isRotating)
                AnimateArrow();
            else if (_isSlowingDown)
                SlowDownArrow();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_isRotating || _isSlowingDown)
                return;

            _isRotating = false;
            _isSlowingDown = true;
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
                FinalizeReward();
        }

        private void FinalizeReward()
        {
            _isSlowingDown = false;
            float multiplier = GetMultiplier(_currentAngle);
            int finalReward = Mathf.RoundToInt(_moneyReward * multiplier);
        }

        private float GetMultiplier(float angle)
        {
            if (angle < -_angleControlValue)
                return 2f;

            if (angle < 0 && angle > -_angleControlValue)
                return 3f;

            if (angle > 0 && angle < _angleControlValue)
                return 4f;

            if (angle > _angleControlValue)
                return 5f;

            return 3f;
        }
    }
}