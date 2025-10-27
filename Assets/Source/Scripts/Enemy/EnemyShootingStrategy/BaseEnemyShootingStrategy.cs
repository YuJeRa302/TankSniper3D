using Assets.Source.Scripts.ScriptableObjects;
using Assets.Source.Scripts.Services;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Enemy
{
    public abstract class BaseEnemyShootingStrategy : IEnemyShootingStrategy
    {
        public abstract ProjectileData ProjectileData { get; }

        public virtual void Construct(Enemy enemy, ProjectileData projectileData, List<Transform> firePoints)
        {
        }

        public virtual void Shoot()
        {
        }

        public ProjectileData GetProjectileData()
        {
            return ProjectileData;
        }

        public void CreateFireSound(ProjectileData projectileData, List<Transform> firePoints, AudioSource audioSource)
        {
            if (projectileData.FireSound == null)
                return;

            foreach (Transform firePoint in firePoints)
            {
                audioSource.PlayOneShot(projectileData.FireSound);
            }
        }

        public void CreateMuzzleFlash(ProjectileData projectileData, List<Transform> firePoints)
        {
            if (projectileData.MuzzleFlash == null)
                return;

            foreach (Transform firePoint in firePoints)
            {
                GameObject.Instantiate(projectileData.MuzzleFlash, firePoint.position, firePoint.rotation);
            }
        }
    }
}