using Assets.Source.Game.Scripts.Enemy;

namespace Assets.Source.Scripts.Game
{
    public struct M_CriticalShoot
    {
        private readonly DamageableArea _hitArea;

        public M_CriticalShoot(DamageableArea damageableArea)
        {
            _hitArea = damageableArea;
        }

        public readonly DamageableArea HitArea => _hitArea;
    }
}