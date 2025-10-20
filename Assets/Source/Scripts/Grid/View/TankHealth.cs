using Assets.Source.Scripts.Game;
using UniRx;
using UnityEngine;

namespace Assets.Source.Scripts.Upgrades
{
    public class TankHealth : MonoBehaviour
    {
        public static readonly IMessageBroker Message = new MessageBroker();

        private int _maxHealth;
        private int _currentHealth;

        public void Initialize(int maxHealth)
        {
            _maxHealth = maxHealth;
            _currentHealth = _maxHealth;
        }

        public void TakeDamage(int damage)
        {
            if (_currentHealth <= 0)
                return;

            _currentHealth -= damage;
            Message.Publish(new M_TankHealthChanged(_currentHealth));

            if (_currentHealth <= 0)
            {
                Message.Publish(new M_DeathTank());
                _currentHealth = 0;
            }
        }
    }
}