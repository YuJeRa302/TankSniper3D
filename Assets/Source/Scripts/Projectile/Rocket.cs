using Assets.Source.Scripts.ScriptableObjects;

namespace Assets.Source.Scripts.Projectile
{
    public class Rocket : BaseProjectile
    {
        private ProjectileData _projectileData;
        private int _damage;

        public override ProjectileData ProjectileData => _projectileData;
        public override int Damage => _damage;

        public override void Initialize(ProjectileData projectileData)
        {
            base.Initialize(projectileData);
            _projectileData = projectileData;
            _damage = projectileData.Damage;
        }
    }
}