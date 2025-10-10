using UnityEngine;

namespace Assets.Source.Game.Scripts.Enemy
{
    public class DamageableArea : MonoBehaviour
    {
        [SerializeField] private int _damageMultiplier;

        private EnemyHealth _enemyHealth;

        public void Initialize(EnemyHealth enemyHealth)
        {
            _enemyHealth = enemyHealth;
        }

        public void ApplyDamage(int baseDamage)
        {
            if (_enemyHealth != null)
            {
                int totalDamage = baseDamage * _damageMultiplier;
                _enemyHealth.TakeDamage(totalDamage);
            }
        }
    }
}