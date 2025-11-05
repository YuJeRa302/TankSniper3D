using Assets.Source.Scripts.ScriptableObjects;
using Assets.Source.Scripts.Services;
using Assets.Source.Scripts.Sound;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Scripts.AnimationShootingStrategy
{
    public abstract class BaseAnimationShootingStrategy : IAnimationShootingStrategy
    {
        public virtual void Construct(
            ProjectileData projectileData,
            AudioPlayer audioPlayer,
            List<Transform> shotPoints)
        {
        }

        public virtual void Shoot()
        {
        }

        public void CreateFireSound(ProjectileData projectileData, AudioPlayer audioPlayer, List<Transform> firePoints)
        {
            if (projectileData.FireSound == null)
                return;

            foreach (Transform firePoint in firePoints)
            {
                audioPlayer.PlayCharacterAudio(projectileData.FireSound);
            }
        }

        public void CreateMuzzleFlash(ProjectileData projectileData, List<Transform> firePoints)
        {
            if (projectileData.MuzzleFlash == null)
                return;

            foreach (Transform firePoint in firePoints)
            {
                var effect = GameObject.Instantiate(
                    projectileData.MuzzleFlash,
                    firePoint.position,
                    firePoint.rotation);

                GameObject.Destroy(effect.gameObject, projectileData.LifeTime);
            }
        }

        public void CreateHitSoundEffect(ProjectileData projectileData, AudioPlayer audioPlayer)
        {
            if (projectileData.HitSound == null)
                return;

            audioPlayer.PlayCharacterAudio(projectileData.HitSound);
        }

        public void CreateHitEffect(ProjectileData projectileData, Vector3 hitPoint)
        {
            if (projectileData.HitEffect == null)
                return;

            GameObject.Instantiate(projectileData.HitEffect, hitPoint, Quaternion.identity);
        }
    }
}