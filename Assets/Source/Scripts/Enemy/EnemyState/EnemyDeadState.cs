using Assets.Source.Game.Scripts.Enemy;
using Assets.Source.Game.Scripts.Enums;
using UnityEngine;

namespace Assets.Source.Scripts.Services
{
    public class EnemyDeadState : BaseEnemyState
    {
        private Enemy _enemy;

        public override TypeEnemyState TypeEnemyState => TypeEnemyState.Death;

        public override void Construct(Enemy enemy)
        {
            base.Construct(enemy);
            _enemy = enemy;
        }

        public override void Enter()
        {
            Debug.Log("Enemy dead");
            _enemy.EnemyAnimation.SetDeathAnimation();
            GameObject.Destroy(_enemy.gameObject, 2f);
        }
    }
}