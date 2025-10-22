using Assets.Source.Scripts.ScriptableObjects;
using UnityEngine;

namespace Assets.Source.Scripts.Projectile
{
    public class LaserBeam : BaseProjectile
    {
        [SerializeField] private Material _laserMaterial;

        private ProjectileData _projectileData;
        private int _damage;

        public Material Material => _laserMaterial;
        public override ProjectileData ProjectileData => _projectileData;
        public override int Damage => _damage;

        public override void Initialize(ProjectileData projectileData)
        {
            base.Initialize(projectileData);
            _projectileData = projectileData;
            _damage = projectileData.Damage;
        }

        protected override void Hit(Collider collider)
        {
        }
    }
}