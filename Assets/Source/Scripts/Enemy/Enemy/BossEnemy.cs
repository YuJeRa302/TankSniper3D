using Assets.Source.Scripts.ScriptableObjects;
using Assets.Source.Scripts.Services;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Enemy
{
    public class BossEnemy : Enemy
    {
        [SerializeField] private EnemyHealthBar _enemyHealthBar;
        [Space(20)]
        [SerializeField] private List<Transform> _firePoints;
        [Space(20)]
        [SerializeField] private ProjectileData _projectileData;
        [Space(20)]
        [SerializeReference] private IEnemyShootingStrategy _enemyShootingStrategy;

        public override void Initialize(Transform tankTransform)
        {
            base.Initialize(tankTransform);
            _enemyShootingStrategy.Construct(this, _projectileData, _firePoints);
            _enemyHealthBar.Initialize(Health);
            EnemyStateStrategy.Initialize(this, _enemyShootingStrategy);
        }
    }
}