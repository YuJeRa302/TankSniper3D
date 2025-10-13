using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Scripts.Services;

namespace Assets.Source.Game.Scripts.Enemy
{
    public abstract class BaseEnemyState : IEnemyState
    {
        public abstract TypeEnemyState TypeEnemyState { get; }

        public virtual void Construct(Enemy enemy, EnemyStateStrategy enemyStateStrategy)
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

        public void SetStateDeath(Enemy enemy, EnemyStateStrategy enemyStateStrategy)
        {
            if (enemy.IsDead)
                enemyStateStrategy.SetNextState(TypeEnemyState.Death);
        }

        public void SetStateAttack(Enemy enemy, EnemyStateStrategy enemyStateStrategy)
        {
            if (enemy.IsPlayerShot)
                enemyStateStrategy.SetNextState(TypeEnemyState.Attack);
        }
    }
}