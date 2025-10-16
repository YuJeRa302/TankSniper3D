using UniRx;

namespace Assets.Source.Game.Scripts.Enemy
{
    public class EnemyHealth
    {
        public static readonly IMessageBroker Message = new MessageBroker();

        private readonly int _maxHealth;

        private bool _isDead = false;
        private int _currentHealth;

        public EnemyHealth(Enemy enemy)
        {
            _maxHealth = enemy.Health;
            _currentHealth = _maxHealth;
        }

        public bool IsDead => _isDead;

        public void TakeDamage(int damage)
        {
            if (_currentHealth <= 0)
                return;

            _currentHealth -= damage;
            Message.Publish(new M_EnemyHealthChanged(_currentHealth));

            if (_currentHealth <= 0)
            {
                Message.Publish(new M_DeathEnemy());
                _currentHealth = 0;
                _isDead = true;
            }
        }
    }
}