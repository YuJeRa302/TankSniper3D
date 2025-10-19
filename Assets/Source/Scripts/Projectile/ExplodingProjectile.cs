using Assets.Source.Game.Scripts.Enemy;
using Assets.Source.Scripts.Game;
using Assets.Source.Scripts.ScriptableObjects;
using UnityEngine;

namespace Assets.Source.Scripts.Projectile
{
    public class ExplodingProjectile : BaseProjectile
    {
        private readonly float _degrees = 360f;

        [SerializeField] private ProjectileData _smallProjectileData;

        private ProjectileData _projectileData;
        private int _damage;

        public override void Initialize(ProjectileData projectileData)
        {
            base.Initialize(projectileData);
            _projectileData = projectileData;
            _damage = projectileData.Damage;
        }

        protected override void Hit(Collider collider)
        {
            Vector3 hitPoint = transform.position;

            if (collider.TryGetComponent(out DestructibleObjectView destructibleObjectView))
            {
                hitPoint = collider.ClosestPoint(transform.position);
                destructibleObjectView.ApplyDamage(hitPoint);
            }

            if (collider.TryGetComponent(out DamageableArea damageableArea))
            {
                hitPoint = collider.ClosestPoint(transform.position);
                damageableArea.ApplyDamage(_damage, hitPoint);
            }

            CreateHitEffect(_projectileData, hitPoint);
            CreateSoundEffect(_projectileData, hitPoint);
            SpawnSmallProjectiles(hitPoint);
            Destroy(gameObject);
        }

        private void SpawnSmallProjectiles(Vector3 spawnPoint)
        {
            for (int i = 0; i < _smallProjectileData.ProjectileCount; i++)
            {
                float angle = i * _degrees / _smallProjectileData.ProjectileCount;
                Vector3 dir = Quaternion.Euler(0, angle, 0) * Vector3.forward;

                var projectile = Instantiate(_smallProjectileData.BaseProjectile, spawnPoint, Quaternion.LookRotation(dir));
                projectile.Initialize(_smallProjectileData);
                Destroy(projectile, _smallProjectileData.LifeTime);
            }
        }
    }
}