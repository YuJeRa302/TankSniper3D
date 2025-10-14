using Assets.Source.Scripts.ScriptableObjects;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Enemy
{
    public class RocketEnemyShootingStrategy : BaseEnemyShootingStrategy
    {
        private Enemy _enemy;
        private ProjectileData _projectileData;

        public override void Construct(Enemy enemy)
        {
            _enemy = enemy;
            _projectileData = _enemy.ProjectileData;
        }

        public override void Shoot()
        {
            if (_projectileData.BaseProjectile == null)
                return;

            Vector3 direction = (_enemy.player.position - _enemy.FirePoint.position).normalized;
            var projectile = GameObject.Instantiate(_projectileData.BaseProjectile, _enemy.FirePoint.position, Quaternion.LookRotation(direction));
            projectile.Initialize(_projectileData);
            CreateFireSound(_enemy);
            CreateMuzzleFlash(_enemy);
        }
    }
}