using Assets.Source.Game.Scripts.Enemy;
using Assets.Source.Game.Scripts.Enums;
using UnityEngine;

namespace Assets.Source.Scripts.Services
{
    public class EnemyReloadState : BaseEnemyState
    {
        private Enemy _enemy;
        private float _reloadTime = 2f;
        private float _timer = 0f;

        public int ReloadNumberForSpecialState { get; private set; } = 0;
        public override TypeEnemyState TypeEnemyState => TypeEnemyState.Reload;

        public override void Construct(Enemy enemy)
        {
            base.Construct(enemy);
            _enemy = enemy;
        }

        public override void Enter()
        {
            _timer = _reloadTime;
            _enemy.EnemyAnimation.SetReloadAnimation();
        }

        public override void Execute()
        {
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