using Assets.Source.Scripts.ScriptableObjects;
using Assets.Source.Scripts.Upgrades;
using UnityEngine;

namespace Assets.Source.Scripts.Projectile
{
    public class EnemySimpleProjectile : BaseProjectile
    {
        private Vector3 _firePoint;
        private ProjectileData _projectileData;
        private int _damage;
        private AudioSource _audioSource;

        public override ProjectileData ProjectileData => _projectileData;
        public override AudioSource AudioSource => _audioSource;
        public override int Damage => _damage;

        public override void Initialize(ProjectileData projectileData, AudioSource audioSource)
        {
            base.Initialize(projectileData, audioSource);
            _projectileData = projectileData;
            _damage = _projectileData.Damage;
            _audioSource = audioSource;
            _firePoint = transform.position;
        }

        protected override void Hit(Collider collider)
        {
            Vector3 hitPoint = transform.position;

            if (collider.TryGetComponent(out TankHealth tankHealth))
            {
                hitPoint = collider.ClosestPoint(transform.position);
                tankHealth.TakeDamage(Damage, _firePoint);
            }

            CreateHitEffect(ProjectileData, hitPoint);
            CreateSoundEffect(ProjectileData, hitPoint);
            Destroy(gameObject);
        }
    }
}