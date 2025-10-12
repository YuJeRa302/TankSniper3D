using Assets.Source.Game.Scripts.Enemy;
using Assets.Source.Game.Scripts.Enums;
using UnityEngine;

namespace Assets.Source.Scripts.Services
{
    public class EnemyReloadState : BaseEnemyState
    {
        private Enemy _enemy;
        private IUseEnemyStateStrategy _useEnemyStateStrategy;
        private float _reloadTime;
        private float _timer = 0f;
        private int _reloadCountForChangePosition;

        public override TypeEnemyState TypeEnemyState => TypeEnemyState.Reload;

        public override void Construct(Enemy enemy, IUseEnemyStateStrategy useEnemyStateStrategy)
        {
            _enemy = enemy;
            _useEnemyStateStrategy = useEnemyStateStrategy;
            _reloadTime = enemy.ReloadTime;
        }

        public override void Enter()
        {
            _timer = _reloadTime;
            _enemy.EnemyAnimation.SetReloadAnimation();
        }

        public override void Execute()
        {
            SetStateDeath(_enemy);
            _timer -= Time.deltaTime;
            ChangeState();
        }

        private void ChangeState()
        {
            if (_timer <= 0f)
            {
                _reloadCountForChangePosition++;

                if (_useEnemyStateStrategy.TryChangePosition(_reloadCountForChangePosition))
                    SetIdleState();
                else
                    SetStateAttack(_enemy);
            }
        }

        private void SetIdleState()
        {
            _reloadCountForChangePosition = 0;
            _enemy.UseEnemyStateStrategy.SetNextState(TypeEnemyState.Idle);
        }
    }
}