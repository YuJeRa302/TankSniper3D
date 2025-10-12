using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Scripts.Services;

namespace Assets.Source.Game.Scripts.Enemy
{
    public abstract class BaseEnemyState : IEnemyState
    {
        public abstract TypeEnemyState TypeEnemyState { get; }

        public virtual void Construct(Enemy enemy, IUseEnemyStateStrategy useEnemyStateStrategy)
        {
        }

        public bool TryGetEnemyStateByType(TypeEnemyState typeEnemyState)
        {
            return TypeEnemyState == typeEnemyState;
        }

        public virtual void Enter()
        {
        }

        public virtual void Execute()
        {
        }

        public void SetStateDeath(Enemy enemy) 
        {
            if (enemy.IsDead)
                enemy.UseEnemyStateStrategy.SetNextState(TypeEnemyState.Death);
        }

        public void SetStateAttack(Enemy enemy)
        {
            if (enemy.IsPlayerShot)
                enemy.UseEnemyStateStrategy.SetNextState(TypeEnemyState.Attack);
        }
    }
}