using Assets.Source.Game.Scripts.Enemy;
using Assets.Source.Scripts.ScriptableObjects;
using Assets.Source.Scripts.Services;
using UnityEngine;

namespace Assets.Source.Scripts.ShootingStrategy
{
    public class BaseShootingStrategy : IShootingStrategy
    {
        public float FindTargetradius { get; private set; } = 300f;

        public virtual void Construct(ProjectileData projectileData, Transform shotPoint)
        {
        }

        public virtual void ShootWithEnergy(bool isVibroEnabled)
        {
        }

        public virtual void ShootWithoutEnergy(bool isVibroEnabled)
        {
        }

        public void CreateVibration(bool isVibroEnabled) 
        {
            if (!isVibroEnabled)
                return;

#if UNITY_ANDROID || UNITY_IOS
        Handheld.Vibrate();
#endif
        }

        public void CreateFireSound(ProjectileData projectileData, Transform firePoint)
        {
            if (projectileData.FireSound == null)
                return;

            AudioSource.PlayClipAtPoint(projectileData.FireSound, firePoint.position);
        }

        public void CreateMuzzleFlash(ProjectileData projectileData, Transform firePoint)
        {
            if (projectileData.MuzzleFlash == null)
                return;

            var effect = GameObject.Instantiate(projectileData.MuzzleFlash, firePoint.position, firePoint.rotation);
            GameObject.Destroy(effect.gameObject, projectileData.LifeTime);
        }

        public Transform FindTargetInCrosshair(float radius)
        {
            Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            Collider[] allTargets = Physics.OverlapSphere(Camera.main.transform.position + Camera.main.transform.forward * 100f, 200f);

            Transform closestTarget = null;
            float minDistance = float.MaxValue;

            foreach (var target in allTargets)
            {
                if (!target.TryGetComponent<DamageableArea>(out var enemy))
                    continue;

                Vector3 screenPos = Camera.main.WorldToScreenPoint(target.transform.position);

                if (screenPos.z > 0 && Vector2.Distance(screenCenter, screenPos) <= radius)
                {
                    float dist = Vector3.Distance(Camera.main.transform.position, target.transform.position);

                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        closestTarget = target.transform;
                    }
                }
            }

            return closestTarget;
        }
    }
}