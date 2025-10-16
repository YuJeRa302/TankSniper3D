using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts.Enemy
{
    public class EnemyHealthBar : MonoBehaviour
    {
        [SerializeField] private Slider _sliderHP;

        private Camera _playerCamera;
        private CompositeDisposable _disposables = new();

        private void OnDestroy()
        {
            _disposables?.Dispose();
        }

        private void LateUpdate()
        {
            if (_playerCamera != null)
                transform.LookAt(_playerCamera.transform);
        }

        public void Initialize(int health)
        {
            _playerCamera = Camera.main;
            SetSliderValue(health);

            EnemyHealth.Message
                .Receive<M_EnemyHealthChanged>()
                .Subscribe(m => OnChangeHealth(m.CurrentHealth))
                .AddTo(_disposables);
        }

        private void SetSliderValue(int value)
        {
            _sliderHP.maxValue = value;
            _sliderHP.value = _sliderHP.maxValue;
        }

        private void OnChangeHealth(int currentHealth)
        {
            _sliderHP.value = currentHealth;
        }
    }
}