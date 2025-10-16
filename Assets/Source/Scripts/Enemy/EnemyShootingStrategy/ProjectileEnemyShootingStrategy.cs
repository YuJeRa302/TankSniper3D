using Assets.Source.Scripts.ScriptableObjects;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Enemy
{
    public class ProjectileEnemyShootingStrategy : BaseEnemyShootingStrategy
    {
        private Enemy _enemy;
        private ProjectileData _projectileData;
        private List<Transform> _firePoints = new();

        public override ProjectileData ProjectileData => _projectileData;

        public override void Construct(Enemy enemy, ProjectileData projectileData, List<Transform> firePoints)
        {
            _enemy = enemy;
            _projectileData = projectileData;
            _firePoints = firePoints;
        }

        public override void Shoot()
        {
            if (_projectileData.BaseProjectile == null)
                return;

            CreateBullet(_firePoints);
            CreateFireSound(_projectileData, _firePoints);
            CreateMuzzleFlash(_projectileData, _firePoints);
        }

        private Vector3 GetSpreadDirection(Transform firePoint)
        {
            Vector3 direction = (_enemy.Player.position - firePoint.position).normalized;

            float spreadAngle = _projectileData.Spread;
            float randomYaw = Random.Range(-spreadAngle, spreadAngle);
            float randomPitch = Random.Range(-spreadAngle, spreadAngle);

            Quaternion spreadRotation = Quaternion.Euler(randomPitch, randomYaw, 0);
            return spreadRotation * direction;
        }

        private void CreateBullet(List<Transform> firePoints)
        {
            foreach (Transform firePoint in firePoints)
            {
                var projectile = GameObject.Instantiate(
                    _projectileData.BaseProjectile,
                    firePoint.position,
                    Quaternion.LookRotation(GetSpreadDirection(firePoint)));

                projectile.Initialize(_projectileData);
            }
        }
    }
}