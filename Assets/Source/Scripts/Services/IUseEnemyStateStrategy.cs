using Assets.Source.Game.Scripts.Enemy;
using Assets.Source.Game.Scripts.Enums;

namespace Assets.Source.Scripts.Services
{
    public interface IUseEnemyStateStrategy
    {
        public void Construct(Enemy enemy);
        public void CurrentStateExecute();
        public void SetNextState(TypeEnemyState typeEnemyState);
    }
}