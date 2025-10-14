using Assets.Source.Game.Scripts.Enemy;
using UnityEngine;

namespace Assets.Source.Scripts.Services
{
    public interface IEnemyShootingStrategy
    {
        public void Construct(Enemy enemy);
        public void Shoot();
    }
}