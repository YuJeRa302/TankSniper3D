using Assets.Source.Game.Scripts.Enemy;
using Assets.Source.Scripts.ScriptableObjects;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Scripts.Services
{
    public interface IEnemyShootingStrategy
    {
        public void Construct(Enemy enemy, ProjectileData projectileData, List<Transform> firePoints);
        public void Shoot();
        public ProjectileData GetProjectileData();
    }
}