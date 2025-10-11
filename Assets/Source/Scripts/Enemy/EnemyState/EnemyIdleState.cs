using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Scripts.Services;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Enemy
{
    public class EnemyIdleState : IEnemyState
    {
        private Enemy _enemy;
        private int _currentWaypointIndex;

        public TypeEnemyState TypeEnemyState => TypeEnemyState.Idle;

        public void Construct(Enemy enemy)
        {
            _enemy = enemy;
        }

        public void Enter()
        {
            _currentWaypointIndex = 0;
            _enemy.EnemyAnimation.SetIdleAnimation();
        }

        public void Execute()
        {
            if (_enemy.IsPlayerShot)
                _enemy.UseEnemyStateStrategy.SetNextState(TypeEnemyState.Attack);

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
        }
    }
}