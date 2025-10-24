using System;
using UniRx;

namespace Assets.Source.Game.Scripts.Enemy
{
    public class EnemyHealth
    {
        private readonly Enemy _enemy;
        private readonly ReactiveProperty<bool> _isDead = new(false);

        public ReactiveProperty<int> CurrentHealth { get; }
        public bool IsDead => _isDead.Value;
        public IObservable<Unit> OnDeath => _isDead.Where(dead => dead).AsUnitObservable();

        public EnemyHealth(Enemy enemy)
        {
            _enemy = enemy;
            CurrentHealth = new ReactiveProperty<int>(_enemy.Health);
        }

        public void TakeDamage(int damage)
        {
            if (_isDead.Value)
                return;

            CurrentHealth.Value -= damage;

            if (CurrentHealth.Value <= 0)
            {
                CurrentHealth.Value = 0;
                _isDead.Value = true;
            }
        }
    }
}