using Assets.Source.Game.Scripts.Enums;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Enemy
{
    public class EnemyAttackState : BaseEnemyState
    {
        private Enemy _enemy;
        private float _fireCooldown = 1f;
        private float _fireTimer = 0f;
        private int _shotsBeforeReload;
        private int _shotCount = 0;

        public override TypeEnemyState TypeEnemyState => TypeEnemyState.Attack;

        public override void Construct(Enemy enemy)
        {
            base.Construct(enemy);
            _enemy = enemy;
            _shotsBeforeReload = enemy.ShotsBeforeReload;
        }

        public override void Enter()
        {
            _fireTimer = 0f;
            _shotCount = 0;
            _enemy.EnemyAnimation.SetAttackAnimation();
        }

        public override void Execute()
        {
            if (_enemy.player == null)
            {
                _enemy.UseEnemyStateStrategy.SetNextState(TypeEnemyState.Idle);
                return;
            }

            Vector3 dirToPlayer = (_enemy.player.position - _enemy.transform.position);
            float distance = dirToPlayer.magnitude;

            Quaternion lookRotation = Quaternion.LookRotation(dirToPlayer.normalized);
            _enemy.transform.rotation = Quaternion.Slerp(_enemy.transform.rotation, lookRotation, _enemy.RotateSpeed * Time.deltaTime);

            _fireTimer -= Time.deltaTime;

            if (_fireTimer <= 0f)
            {
                Shoot();
                _fireTimer = _fireCooldown;
            }
        }

        private void Shoot()
        {
            SetReloadState();
            _shotCount++;
        }

        private void SetReloadState()
        {
            if (_shotCount >= _shotsBeforeReload)
                _enemy.UseEnemyStateStrategy.SetNextState(TypeEnemyState.Reload);
        }
    }
}