using Assets.Source.Game.Scripts.Enums;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Enemy
{
    public class EnemyIdleState : BaseEnemyState
    {
        private readonly float _controlDistanceValue = 0.1f;

        private Enemy _enemy;
        private EnemyStateStrategy _enemyStateStrategy;
        private int _currentWaypointIndex = 0;
        private int _lastReachedWaypointIndex = 0;

        public override TypeEnemyState TypeEnemyState => TypeEnemyState.Idle;

        public override void Construct(Enemy enemy, EnemyStateStrategy enemyStateStrategy)
        {
            _enemy = enemy;
            _enemyStateStrategy = enemyStateStrategy;
        }

        public override void Enter()
        {
            ChangeReachedWaypoint();
            _enemy.EnemyAnimation.SetIdleAnimation();
            _enemy.EnemySoundPlayer.PlayMovingSound();
        }

        public override void Execute()
        {
            SetStateDeath(_enemy, _enemyStateStrategy);

            if (_enemyStateStrategy.GetPositionChangedStatus() == true)
                SetStateAttack(_enemy, _enemyStateStrategy);

            if (_enemy.Waypoints.Count == 0)
                return;

            _enemy.EnemyAnimation.SetMoveAnimation();

            Transform target = _enemy.Waypoints[_currentWaypointIndex];
            Vector3 direction = (target.position - _enemy.transform.position).normalized;
            _enemy.transform.position += direction * _enemy.MoveSpeed * Time.deltaTime;
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            _enemy.transform.rotation = Quaternion.Slerp(
                _enemy.transform.rotation,
                lookRotation,
                _enemy.RotateSpeed * Time.deltaTime);

            _enemy.RotationPartToPlayer.rotation = Quaternion.Slerp(
                _enemy.RotationPartToPlayer.rotation,
                lookRotation,
                _enemy.RotateSpeed * Time.deltaTime);

            if (Vector3.Distance(_enemy.transform.position, target.position) < _controlDistanceValue)
                SetNextWayPoint();

            _enemyStateStrategy.TryChangeIdleState(_lastReachedWaypointIndex);
        }

        private void ChangeReachedWaypoint()
        {
            if (_lastReachedWaypointIndex != _enemyStateStrategy.PositionNumber)
                return;

            _lastReachedWaypointIndex = _currentWaypointIndex;
        }

        private void SetNextWayPoint()
        {
            _lastReachedWaypointIndex = _currentWaypointIndex;
            _currentWaypointIndex = (_currentWaypointIndex + 1) % _enemy.Waypoints.Count;
        }
    }
}