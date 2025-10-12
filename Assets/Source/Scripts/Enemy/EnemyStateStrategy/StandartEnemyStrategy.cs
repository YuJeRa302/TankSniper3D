using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Scripts.Services;
using System.Collections.Generic;

namespace Assets.Source.Game.Scripts.Enemy
{
    public abstract class StandartEnemyStrategy : IUseEnemyStateStrategy
    {
        private Enemy _enemy;
        private IEnemyState _currentState;
        private List<IEnemyState> _enemyState = new();
        private bool _isPositionChanged = true;

        public abstract int ReloadCountForPositionChanged { get; }
        public abstract int CountPositionChange { get; }

        public void Construct(Enemy enemy)
        {
            _enemy = enemy;
            _enemyState = _enemy.EnemyStates;
            _enemyState.ForEach(s => s.Construct(_enemy, this));
            _currentState = GetEnemyStateByType(TypeEnemyState.Idle);
            _currentState.Enter();
        }

        public void CurrentStateExecute()
        {
            _currentState?.Execute();
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
            if (CountPositionChange == countPositionChange)
            {
                _isPositionChanged = true;
                return true;
            }

            return false;
        }

        private IEnemyState GetEnemyStateByType(TypeEnemyState typeEnemyState)
        {
            foreach (var state in _enemyState)
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