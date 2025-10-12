using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Scripts.Services;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Enemy
{
    public class EnemyIdleState : BaseEnemyState
    {
        private Enemy _enemy;
        private IUseEnemyStateStrategy _useEnemyStateStrategy;
        private int _currentWaypointIndex;

        public override TypeEnemyState TypeEnemyState => TypeEnemyState.Idle;

        public override void Construct(Enemy enemy, IUseEnemyStateStrategy useEnemyStateStrategy)
        {
            _enemy = enemy;
            _useEnemyStateStrategy = useEnemyStateStrategy;
        }

        public override void Enter()
        {
            _currentWaypointIndex = 0;
            _enemy.EnemyAnimation.SetIdleAnimation();
        }

        public override void Execute()
        {
            SetStateDeath(_enemy);

            if (_useEnemyStateStrategy.GetPositionChangedStatus() == true)
                SetStateAttack(_enemy);

            if (_enemy.Waypoints.Count == 0)
                return;

            _enemy.EnemyAnimation.SetMoveAnimation();

            Transform target = _enemy.Waypoints[_currentWaypointIndex];
            Vector3 direction = (target.position - _enemy.transform.position).normalized;
            _enemy.transform.position += direction * _enemy.MoveSpeed * Time.deltaTime;

            Quaternion lookRotation = Quaternion.LookRotation(direction);
            _enemy.transform.rotation = Quaternion.Slerp(_enemy.transform.rotation, lookRotation, _enemy.RotateSpeed * Time.deltaTime);

            if (Vector3.Distance(_enemy.transform.position, target.position) < 0.5f)
                _currentWaypointIndex = (_currentWaypointIndex + 1) % _enemy.Waypoints.Count;

            _useEnemyStateStrategy.TryChangeIdleState(_currentWaypointIndex);
        }
    }
}