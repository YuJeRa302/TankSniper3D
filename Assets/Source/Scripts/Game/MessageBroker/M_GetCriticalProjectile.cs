using Assets.Source.Game.Scripts.Enemy;
using Assets.Source.Scripts.Projectile;

namespace Assets.Source.Scripts.Game
{
    public struct M_GetCriticalProjectile
    {
        private readonly BaseProjectile _baseProjectile;
        private readonly DamageableArea _damageableArea;

        public M_GetCriticalProjectile(BaseProjectile baseProjectile, DamageableArea damageableArea)
        {
            _baseProjectile = baseProjectile;
            _damageableArea = damageableArea;
        }

        public readonly BaseProjectile BaseProjectile => _baseProjectile;
        public readonly DamageableArea DamageableArea => _damageableArea;
    }
}