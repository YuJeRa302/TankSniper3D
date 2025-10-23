using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Scripts.Game
{
    public class DroneInterferenceEffect : MonoBehaviour
    {
        [SerializeField] private Image _interferenceImage;
        [SerializeField] private float _maxAlpha = 0.6f;
        [SerializeField] private float _shakeMagnitude = 0.1f;
        [SerializeField] private float _shakeSpeed = 20f;

        private float _currentAlpha = 0f;
        private Vector3 _originalCamPos;

        public void Initialize()
        {
            _originalCamPos = transform.localPosition;
            UpdateInterferenceImage(0);
        }

        public void UpdateEffect(float intensity)
        {
            _currentAlpha = Mathf.Lerp(0f, _maxAlpha, intensity);
            UpdateInterferenceImage(_currentAlpha);

            float shakeAmount = _shakeMagnitude * intensity;
            float shakeX = Mathf.PerlinNoise(Time.time * _shakeSpeed, 0f) * 2f - 1f;
            float shakeY = Mathf.PerlinNoise(0f, Time.time * _shakeSpeed) * 2f - 1f;

            Vector3 shakeOffset = new Vector3(shakeX, shakeY, 0f) * shakeAmount;
            transform.localPosition = _originalCamPos + shakeOffset;
        }

        public void ResetEffect()
        {
            UpdateInterferenceImage(0);
            transform.localPosition = _originalCamPos;
        }

        private void UpdateInterferenceImage(float alphaValue)
        {
            if (_interferenceImage != null)
            {
                Color color = _interferenceImage.color;
                color.a = alphaValue;
                _interferenceImage.color = color;
            }
        }
    }
}