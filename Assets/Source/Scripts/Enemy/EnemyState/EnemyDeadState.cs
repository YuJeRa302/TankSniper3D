using Assets.Source.Game.Scripts.Enemy;
using Assets.Source.Game.Scripts.Enums;
using UnityEngine;

namespace Assets.Source.Scripts.Services
{
    public class EnemyDeadState : IEnemyState
    {
        private Enemy _enemy;

        public TypeEnemyState TypeEnemyState => TypeEnemyState.Death;

        public void Construct(Enemy enemy)
        {
            _enemy = enemy;
        }

        public void Enter()
        {
            _enemy.EnemyAnimation.SetDeathAnimation();
            GameObject.Destroy(_enemy.gameObject, 2f);
        }

        public void Execute()
        {
        }
    }
}