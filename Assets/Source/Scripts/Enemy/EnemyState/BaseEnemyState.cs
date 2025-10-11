using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Scripts.Services;

namespace Assets.Source.Game.Scripts.Enemy
{
    public abstract class BaseEnemyState : IEnemyState
    {
        public abstract TypeEnemyState TypeEnemyState { get; }

        public virtual void Construct(Enemy enemy)
        {
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
    }
}