using UnityEngine;

namespace Assets.Source.Game.Scripts.Enemy
{
    public class DamageableArea : MonoBehaviour
    {
        [SerializeField] private int _damageMultiplier;
        [SerializeField] private DetachablePart _detachablePart;

        private EnemyHealth _enemyHealth;

        public void Initialize(EnemyHealth enemyHealth)
        {
            _enemyHealth = enemyHealth;
        }

        public void ApplyDamage(int baseDamage, Vector3 hitPoint)
        {
            if (_enemyHealth != null)
            {
                int totalDamage = baseDamage * _damageMultiplier;
                _enemyHealth.TakeDamage(totalDamage);
            }

            if (_detachablePart != null)
                _detachablePart.Detach(hitPoint);
        }
    }
}