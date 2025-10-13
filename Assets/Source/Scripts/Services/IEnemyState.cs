using Assets.Source.Game.Scripts.Enemy;
using Assets.Source.Game.Scripts.Enums;

namespace Assets.Source.Scripts.Services
{
    public interface IEnemyState
    {
        public void Construct(Enemy enemy, EnemyStateStrategy enemyStateStrategy);
        public bool TryGetEnemyStateByType(TypeEnemyState typeEnemyState);
        public void Enter();
        public void Execute();
    }
}