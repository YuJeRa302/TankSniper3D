namespace Assets.Source.Game.Scripts.Enemy
{
    public struct M_EnemyHealthChanged
    {
        private readonly int _currentHealth;

        public M_EnemyHealthChanged(int currentHealth)
        {
            _currentHealth = currentHealth;
        }

        public readonly int CurrentHealth => _currentHealth;
    }
}