using UniRx;
using UnityEngine;

namespace Assets.Source.Scripts.Game
{
    public class DroneHealth : MonoBehaviour
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

            if (_currentHealth <= 0)
            {
                Message.Publish(new M_DeathDrone());
                _currentHealth = 0;
            }
        }
    }
}