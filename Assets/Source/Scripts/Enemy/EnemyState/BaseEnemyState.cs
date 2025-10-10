using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Scripts.Services;
using System;
using UniRx;

namespace Assets.Source.Game.Scripts.Enemy
{
    public abstract class BaseEnemyState : IEnemyState, IDisposable
    {
        private Enemy _enemy;
        private CompositeDisposable _disposables = new();

        public abstract TypeEnemyState TypeEnemyState { get; }

        public virtual void Construct(Enemy enemy)
        {
            _enemy = enemy;

            EnemyHealth.Message
                .Receive<M_DeathEnemy>()
                .Subscribe(m => SetDeathState())
                .AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables?.Dispose();
        }

        public virtual void Enter()
        {
        }

        public virtual void Execute()
        {
        }

        public virtual void Exit()
        {
        }

        private void SetDeathState()
        {
            _enemy.UseEnemyStateStrategy.SetNextState(TypeEnemyState.Death);
        }
    }
}