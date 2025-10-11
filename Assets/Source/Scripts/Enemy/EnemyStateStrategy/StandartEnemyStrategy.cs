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

        public abstract int ReloadNumberForSpecialState { get; }

        public void Construct(Enemy enemy)
        {
            _enemy = enemy;
            _enemyState = _enemy.EnemyStates;
            _enemyState.ForEach(s => s.Construct(_enemy));
            _currentState = GetStateByType(TypeEnemyState.Idle);
            _currentState.Enter();
        }

        public void CurrentStateExecute()
        {
            _currentState?.Execute();
        }

        private void LoadEnemyStates()
        {
            foreach (var enemy in _enemy.EnemyStates)
            {
                _enemyState.Add(enemy as BaseEnemyState);
            }
        }

        public void SetNextState(TypeEnemyState typeEnemyState)
        {
            IEnemyState nextState = null;

            //if (_enemy.IsDead)
            //{
            //    nextState = GetStateByType(TypeEnemyState.Death);
            //    ChangeCurrentState(nextState);
            //    return;
            //}

            //BaseEnemyState nextState;
            //int reloadNumber = GetReloadNumber(TypeEnemyState.Reload);

            //if (TrySetSpecialState(reloadNumber))
            //    nextState = GetStateByType(TypeEnemyState.Idle);
            //else
            //    nextState = GetStateByType(typeEnemyState);

            if (nextState == null)
                nextState = GetStateByType(typeEnemyState);

            ChangeCurrentState(nextState);
        }

        private int GetReloadNumber(TypeEnemyState typeEnemyState)
        {
            foreach (var enemyState in _enemyState)
            {
                if (enemyState.TypeEnemyState == typeEnemyState)
                    return (enemyState as EnemyReloadState).ReloadNumberForSpecialState;
            }

            return 0;
        }

        private bool TrySetSpecialState(int reloadNumber)
        {
            if (ReloadNumberForSpecialState == reloadNumber)
                return true;

            return false;
        }

        private IEnemyState GetStateByType(TypeEnemyState typeEnemyState)
        {
            foreach (var enemyState in _enemyState)
            {
                if (enemyState.TypeEnemyState == typeEnemyState)
                    return enemyState;
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