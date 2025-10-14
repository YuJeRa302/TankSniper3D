using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Scripts.Services;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Enemy
{
    public class EnemyAttackState : BaseEnemyState
    {
        private IEnemyShootingStrategy _shootingStrategy;
        private EnemyStateStrategy _enemyStateStrategy;
        private Enemy _enemy;
        private float _fireCooldown = 1f;
        private float _fireTimer = 0f;
        private int _shotsBeforeReload;
        private int _shotCount = 0;

        public override TypeEnemyState TypeEnemyState => TypeEnemyState.Attack;

        public override void Construct(Enemy enemy, EnemyStateStrategy enemyStateStrategy)
        {
            _enemy = enemy;
            _enemyStateStrategy = enemyStateStrategy;
            _shotsBeforeReload = enemy.ShotsBeforeReload;
            _shootingStrategy = enemy.EnemyShootingStrategy;
            _fireCooldown = enemy.ProjectileData.ReloadTime;
        }

        public override void Enter()
        {
            _fireTimer = _fireCooldown;
            _shotCount = 0;
            _enemy.EnemyAnimation.SetAttackAnimation();
        }

        public override void Execute()
        {
            SetStateDeath(_enemy, _enemyStateStrategy);

            Vector3 dirToPlayer = (_enemy.player.position - _enemy.transform.position);
            float distance = dirToPlayer.magnitude;

            Quaternion lookRotation = Quaternion.LookRotation(dirToPlayer.normalized);
            _enemy.transform.rotation = Quaternion.Slerp(_enemy.transform.rotation, lookRotation, _enemy.RotateSpeed * Time.deltaTime);

            ApplyDelayBeforeShooting();
        }

        private void ApplyDelayBeforeShooting()
        {
            _fireTimer -= Time.deltaTime;

            if (_fireTimer <= 0f)
            {
                Shoot();
                _fireTimer = _fireCooldown;
            }
        }

        private void Shoot()
        {
            _shootingStrategy.Shoot();
            _shotCount++;
            SetReloadState();
        }

        private void SetReloadState()
        {
            if (_shotCount >= _shotsBeforeReload)
                _enemyStateStrategy.SetNextState(TypeEnemyState.Reload);
        }
    }
}