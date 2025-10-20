namespace Assets.Source.Scripts.Game
{
    public struct M_TankHealthChanged
    {
        private readonly int _currentHealth;

        public M_TankHealthChanged(int currentHealth)
        {
            _currentHealth = currentHealth;
        }

        public readonly int CurrentHealth => _currentHealth;
    }
}