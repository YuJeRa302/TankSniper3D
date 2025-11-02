using Assets.Source.Scripts.Game;
using UniRx;
using UnityEngine;

namespace Assets.Source.Scripts.Upgrades
{
    public class TankHealth : MonoBehaviour
    {
        public static readonly IMessageBroker Message = new MessageBroker();

        private CompositeDisposable _disposables = new();
        private int _maxHealth;
        private int _currentHealth;

        private void OnDestroy()
        {
            _disposables?.Dispose();
        }

        public void Initialize(int maxHealth)
        {
            _maxHealth = maxHealth;
            _currentHealth = _maxHealth;

            DefeatTab.Message
                .Receive<M_RecoverPlayer>()
                .Subscribe(m => OnRecoveryTankHealth())
                .AddTo(_disposables);
        }

        public void TakeDamage(int damage, Vector3 firePoint)
        {
            if (_currentHealth <= 0)
                return;

            _currentHealth -= damage;
            Message.Publish(new M_TankHealthChanged(_currentHealth));
            Message.Publish(new M_PlayerHitFromDirection(firePoint));

            if (_currentHealth <= 0)
            {
                Message.Publish(new M_DeathTank());
                _currentHealth = 0;
            }
        }

        private void OnRecoveryTankHealth()
        {
            _currentHealth = _maxHealth;
            Message.Publish(new M_TankHealthChanged(_currentHealth));
        }
    }
}