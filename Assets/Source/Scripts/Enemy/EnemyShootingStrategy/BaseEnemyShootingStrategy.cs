using Assets.Source.Scripts.Services;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Enemy
{
    public class BaseEnemyShootingStrategy : IEnemyShootingStrategy
    {
        public virtual void Construct(Enemy enemy)
        {
        }
        public virtual void Shoot()
        {
        }

        public void CreateFireSound(Enemy enemy)
        {
            if (enemy.ProjectileData.FireSound != null)
                AudioSource.PlayClipAtPoint(enemy.ProjectileData.FireSound, enemy.FirePoint.position);
        }

        public void CreateMuzzleFlash(Enemy enemy)
        {
            if (enemy.ProjectileData.MuzzleFlash == null)
                return;

            GameObject.Instantiate(enemy.ProjectileData.MuzzleFlash, enemy.FirePoint.position, enemy.FirePoint.rotation);
        }
    }
}