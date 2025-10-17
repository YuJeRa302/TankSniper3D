using Assets.Source.Game.Scripts.Enemy;
using Assets.Source.Game.Scripts.Enums;
using UnityEngine;

namespace Assets.Source.Scripts.Services
{
    public class EnemyReloadState : BaseEnemyState
    {
        private Enemy _enemy;
        private EnemyStateStrategy _enemyStateStrategy;
        private float _reloadTime;
        private float _timer = 0f;
        private int _reloadCountForChangePosition;

        public override TypeEnemyState TypeEnemyState => TypeEnemyState.Reload;

        public override void Construct(Enemy enemy, EnemyStateStrategy enemyStateStrategy)
        {
            _enemy = enemy;
            _enemyStateStrategy = enemyStateStrategy;
            _reloadTime = enemyStateStrategy.EnemyShootingStrategy.GetProjectileData().ReloadTime;
        }

        public override void Enter()
        {
            _timer = _reloadTime;
            _enemy.EnemyAnimation.SetReloadAnimation();
            _enemy.EnemySoundPlayer.StopMovingSound();
            _enemy.EnemySoundPlayer.PlayerSoundReloading();
        }

        public override void Execute()
        {
            SetStateDeath(_enemy, _enemyStateStrategy);
            _timer -= Time.deltaTime;
            ChangeState();
        }

        private void ChangeState()
        {
            if (_timer <= 0f)
            {
                _reloadCountForChangePosition++;

                if (_enemyStateStrategy.TryChangePosition(_reloadCountForChangePosition))
                    SetIdleState();
                else
                    SetStateAttack(_enemy, _enemyStateStrategy);
            }
        }

        private void SetIdleState()
        {
            _reloadCountForChangePosition = 0;
            _enemyStateStrategy.SetNextState(TypeEnemyState.Idle);
        }
    }
}