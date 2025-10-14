using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Scripts.Services;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Enemy
{
    public class EnemyStateStrategy : MonoBehaviour
    {
        [SerializeReference] private List<IEnemyState> _enemyStates;
        [Space(20)]
        [SerializeField] private bool _isPositionChanged = true;
        [SerializeField] private int _reloadCountForPositionChanged;
        [SerializeField] private int _positionNumber;

        private Enemy _enemy;
        private IEnemyState _currentState;

        public int ReloadCountForPositionChanged => _reloadCountForPositionChanged;
        public int PositionNumber => _positionNumber;

        private void Update()
        {
            _currentState?.Execute();
        }

        public void Initialize(Enemy enemy)
        {
            _enemy = enemy;
            _enemyStates.ForEach(s => s.Construct(_enemy, this));
            _currentState = GetEnemyStateByType(TypeEnemyState.Idle);
            _currentState.Enter();
        }

        public bool GetPositionChangedStatus()
        {
            return _isPositionChanged;
        }

        public void SetNextState(TypeEnemyState typeEnemyState)
        {
            IEnemyState nextState = GetEnemyStateByType(typeEnemyState);
            ChangeCurrentState(nextState);
        }

        public bool TryChangePosition(int reloadCount)
        {
            if (_enemy.Waypoints.Count == 0)
                return false;

            if (reloadCount > 0)
            {
                if (ReloadCountForPositionChanged == reloadCount)
                {
                    _isPositionChanged = false;
                    return true;
                }
            }

            return false;
        }

        public bool TryChangeIdleState(int countPositionChange)
        {
            if (_positionNumber == countPositionChange)
            {
                _isPositionChanged = true;
                return true;
            }

            return false;
        }

        private IEnemyState GetEnemyStateByType(TypeEnemyState typeEnemyState)
        {
            foreach (var state in _enemyStates)
            {
                if (state.TryGetEnemyStateByType(typeEnemyState))
                    return state;
            }

            return null;
        }

        private void ChangeCurrentState(IEnemyState newState)
        {
            if (newState == null)
                return;

            _currentState = newState;
            _currentState?.Enter();
        }
    }
}