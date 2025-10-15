using Assets.Source.Game.Scripts.Enemy;
using Assets.Source.Game.Scripts.Enums;
using UnityEngine;

namespace Assets.Source.Scripts.Services
{
    public class EnemyDeadState : BaseEnemyState
    {
        private Enemy _enemy;

        public override TypeEnemyState TypeEnemyState => TypeEnemyState.Death;

        public override void Construct(Enemy enemy, EnemyStateStrategy enemyStateStrategy)
        {
            _enemy = enemy;
        }

        public override void Enter()
        {
            _enemy.EnemySoundPlayer.StopMovingSound();
            _enemy.EnemySoundPlayer.PlayExplosionSound();
            _enemy.EnemyAnimation.SetDeathAnimation();
            _enemy.CreateExplosionEffect();
            _enemy.DestroyParts();
            GameObject.Destroy(_enemy.gameObject, 3f);
        }
    }
}