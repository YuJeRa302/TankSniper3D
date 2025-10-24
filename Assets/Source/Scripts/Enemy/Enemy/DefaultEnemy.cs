using Assets.Source.Scripts.Models;
using Assets.Source.Scripts.ScriptableObjects;
using Assets.Source.Scripts.Services;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Enemy
{
    public class DefaultEnemy : Enemy
    {
        [SerializeField] private List<Transform> _firePoints;
        [Space(20)]
        [SerializeField] private ProjectileData _projectileData;
        [Space(20)]
        [SerializeReference] private IEnemyShootingStrategy _enemyShootingStrategy;

        public override void Initialize(Transform tankTransform, GameModel gameModel)
        {
            base.Initialize(tankTransform, gameModel);
            _enemyShootingStrategy.Construct(this, _projectileData, _firePoints);
            EnemyStateStrategy.Initialize(this, _enemyShootingStrategy);
        }
    }
}