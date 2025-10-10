using Assets.Source.Game.Scripts.Enemy;

namespace Assets.Source.Scripts.Services
{
    public interface IEnemyState
    {
        public void Construct(Enemy enemy);
        public void Enter();
        public void Execute();
        public void Exit();
    }
}