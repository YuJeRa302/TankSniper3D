using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts.Enemy
{
    public class EnemyHealthBar : MonoBehaviour
    {
        [SerializeField] private Slider _sliderHP;

        private EnemyHealth _enemyHealth;
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

        public void Initialize(EnemyHealth enemyHealth)
        {
            _playerCamera = Camera.main;
            SetSliderValue(enemyHealth.CurrentHealth.Value);

            _enemyHealth.CurrentHealth
                .Subscribe(OnChangeHealth)
                .AddTo(this);

            _enemyHealth.OnDeath
                .Subscribe(_ => OnEnemyDeath())
                .AddTo(this);
        }

        private void SetSliderValue(int value)
        {
            _sliderHP.maxValue = value;
            _sliderHP.value = _sliderHP.maxValue;
        }

        private void OnEnemyDeath()
        {
            Destroy(gameObject);
        }

        private void OnChangeHealth(int currentHealth)
        {
            _sliderHP.value = currentHealth;
        }
    }
}