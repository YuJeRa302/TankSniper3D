using Assets.Source.Game.Scripts.Enemy;
using Assets.Source.Scripts.Models;
using Assets.Source.Scripts.Upgrades;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Scripts.Game
{
    public class GameParametersView : MonoBehaviour
    {
        [SerializeField] private Slider _sliderHP;
        [SerializeField] private TMP_Text _enemyCount;

        private int _currentEnemyCount;
        private int _maxEnemyCount;
        private CompositeDisposable _disposables = new();

        private void OnDestroy()
        {
            _disposables?.Dispose();
        }

        public void Initialize(int health, int enemyCount)
        {
            _currentEnemyCount = 0;
            _maxEnemyCount = enemyCount;
            SetSliderValue(health);
            SetEnemyCount(_currentEnemyCount);

            TankHealth.Message
                .Receive<M_TankHealthChanged>()
                .Subscribe(m => OnChangeHealth(m.CurrentHealth))
                .AddTo(_disposables);

            GameModel.Message
                .Receive<M_DeathEnemy>()
                .Subscribe(m => OnChangeEnemyCount())
                .AddTo(_disposables);

            gameObject.SetActive(false);
        }

        private void SetEnemyCount(int currentEnemyCount)
        {
            _enemyCount.text = currentEnemyCount.ToString() + "/" + _maxEnemyCount;
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

        private void OnChangeEnemyCount()
        {
            _currentEnemyCount = Mathf.Clamp(_currentEnemyCount + 1, 0, _maxEnemyCount);
            SetEnemyCount(_currentEnemyCount);

            if (_currentEnemyCount == _maxEnemyCount)
                MessageBroker.Default.Publish(new M_FinishGame());
        }
    }
}