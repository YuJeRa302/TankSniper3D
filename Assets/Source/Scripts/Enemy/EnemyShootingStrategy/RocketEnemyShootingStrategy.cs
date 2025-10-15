using Assets.Source.Scripts.ScriptableObjects;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Enemy
{
    public class RocketEnemyShootingStrategy : BaseEnemyShootingStrategy
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

            CreateRocket(_firePoints);
            CreateFireSound(_projectileData, _firePoints);
            CreateMuzzleFlash(_projectileData, _firePoints);
        }

        private void CreateRocket(List<Transform> firePoints)
        {
            foreach (Transform firePoint in firePoints)
            {
                Vector3 direction = (_enemy.Player.position - firePoint.position).normalized;

                var projectile = GameObject.Instantiate(
                    _projectileData.BaseProjectile,
                    firePoint.position,
                    Quaternion.LookRotation(direction));

                projectile.Initialize(_projectileData);
            }
        }
    }
}