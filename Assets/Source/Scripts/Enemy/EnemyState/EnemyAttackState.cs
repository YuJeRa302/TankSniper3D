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
            _shootingStrategy = enemyStateStrategy.EnemyShootingStrategy;
            Fill();
        }

        public override void Enter()
        {
            _fireTimer = _fireCooldown;
            _shotCount = 0;
            _enemy.EnemyAnimation.SetAttackAnimation();
            _enemy.EnemySoundPlayer.StopMovingSound();
            _enemy.EnemySoundPlayer.PlayerSoundStanding();
        }

        public override void Execute()
        {
            SetStateDeath(_enemy, _enemyStateStrategy);

            Vector3 dirToPlayer = (_enemy.Player.position - _enemy.transform.position);
            float distance = dirToPlayer.magnitude;
            Quaternion lookRotation = Quaternion.LookRotation(dirToPlayer.normalized);
            RotatePartToPlayer(dirToPlayer);
            ApplyDelayBeforeShooting();
        }

        private void Fill()
        {
            if (_shootingStrategy == null)
                return;

            _shotsBeforeReload = _shootingStrategy.GetProjectileData().ProjectileCount;
            _fireCooldown = _shootingStrategy.GetProjectileData().ReloadTime;
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

        private void RotatePartToPlayer(Vector3 direction)
        {
            Vector3 directionOnPlane = new Vector3(direction.x, 0, direction.z).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(directionOnPlane);

            _enemy.RotationPartToPlayer.rotation = Quaternion.Slerp(
                _enemy.RotationPartToPlayer.rotation,
                lookRotation,
                _enemy.RotateSpeed * Time.deltaTime);
        }
    }
}