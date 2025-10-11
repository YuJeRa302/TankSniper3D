using Assets.Source.Game.Scripts.Enemy;
using Assets.Source.Game.Scripts.Enums;
using UnityEngine;

namespace Assets.Source.Scripts.Services
{
    public class EnemyReloadState : IEnemyState
    {
        private Enemy _enemy;
        private float _reloadTime;
        private float _timer = 0f;

        public int ReloadNumberForSpecialState { get; private set; } = 0;
        public TypeEnemyState TypeEnemyState => TypeEnemyState.Reload;

        public void Construct(Enemy enemy)
        {
            _enemy = enemy;
            _reloadTime = enemy.ReloadTime;
        }

        public void Enter()
        {
            _timer = _reloadTime;
            _enemy.EnemyAnimation.SetReloadAnimation();
        }

        public void Execute()
        {
            if (_enemy.IsDead)
                _enemy.UseEnemyStateStrategy.SetNextState(TypeEnemyState.Death);

            _timer -= Time.deltaTime;
            SetReloadState();
        }

        private void SetReloadState()
        {
            if (_timer <= 0f)
            {
                ReloadNumberForSpecialState++;
                _enemy.UseEnemyStateStrategy.SetNextState(TypeEnemyState.Attack);
            }
        }
    }
}