using Assets.Source.Game.Scripts.Enemy;
using Assets.Source.Game.Scripts.Enums;

namespace Assets.Source.Scripts.Services
{
    public interface IEnemyState
    {
        public TypeEnemyState TypeEnemyState { get; }
        public void Construct(Enemy enemy);
        public void Enter();
        public void Execute();
    }
}