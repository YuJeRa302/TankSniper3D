using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Scripts.Services;
using System;
using System.Collections.Generic;

namespace Assets.Source.Game.Scripts.Enemy
{
    public abstract class StandartEnemyStrategy : IUseEnemyStateStrategy, IDisposable
    {
        private Enemy _enemy;
        private BaseEnemyState _currentState;
        private List<BaseEnemyState> _enemyState;

        public abstract int ReloadNumberForSpecialState { get; }

        public void Dispose()
        {
            _enemyState.ForEach(s => s.Dispose());
        }

        public void Construct(Enemy enemy)
        {
            _enemy = enemy;
            _enemyState = enemy.EnemyStates;
            _enemyState.ForEach(s => s.Construct(_enemy));
            _currentState = GetStateByType(TypeEnemyState.Idle);
        }

        public void CurrentStateExecute()
        {
            _currentState?.Execute();
        }

        public void SetNextState(TypeEnemyState typeEnemyState)
        {
            if (TrySetDeathState(typeEnemyState))
            {
                ChangeCurrentState(GetStateByType(typeEnemyState));
                return;
            }

            BaseEnemyState nextState;
            int reloadNumber = GetReloadNumber(TypeEnemyState.Reload);

            if (TrySetSpecialState(reloadNumber))
                nextState = GetStateByType(TypeEnemyState.Idle);
            else
                nextState = GetStateByType(typeEnemyState);

            ChangeCurrentState(nextState);
        }

        private bool TrySetDeathState(TypeEnemyState typeEnemyState)
        {
            if (TypeEnemyState.Death == typeEnemyState)
                return true;

            return false;
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

        private BaseEnemyState GetStateByType(TypeEnemyState typeEnemyState)
        {
            foreach (var enemyState in _enemyState)
            {
                if (enemyState.TypeEnemyState == typeEnemyState)
                    return enemyState;
            }

            return null;
        }

        private void ChangeCurrentState(BaseEnemyState newState)
        {
            if (newState == null)
                return;

            _currentState?.Exit();
            _currentState = newState;
            _currentState?.Enter();
        }
    }
}