using Assets.Source.Scripts.ScriptableObjects;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Enemy
{
    public class BulletEnemyShootingStrategy : BaseEnemyShootingStrategy
    {
        private Enemy _enemy;
        private ProjectileData _projectileData;
        private Vector3 _spreadDirection;

        public override void Construct(Enemy enemy)
        {
            _enemy = enemy;
            _projectileData = _enemy.ProjectileData;
        }

        public override void Shoot()
        {
            if (_projectileData.BaseProjectile == null)
                return;

            CreateSpread();
            var projectile = GameObject.Instantiate(_projectileData.BaseProjectile, _enemy.FirePoint.position, Quaternion.LookRotation(_spreadDirection));
            projectile.Initialize(_projectileData);
            CreateFireSound(_enemy);
            CreateMuzzleFlash(_enemy);
        }

        private void CreateSpread()
        {
            Vector3 direction = (_enemy.player.position - _enemy.FirePoint.position).normalized;

            float spreadAngle = _projectileData.Spread;
            float randomYaw = Random.Range(-spreadAngle, spreadAngle);
            float randomPitch = Random.Range(-spreadAngle, spreadAngle);

            Quaternion spreadRotation = Quaternion.Euler(randomPitch, randomYaw, 0);
            _spreadDirection = spreadRotation * direction;
        }
    }
}