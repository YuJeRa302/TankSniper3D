using Assets.Source.Game.Scripts.Enemy;
using Assets.Source.Scripts.ScriptableObjects;
using Assets.Source.Scripts.Services;
using Assets.Source.Scripts.Sound;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Scripts.ShootingStrategy
{
    public class BaseShootingStrategy : IShootingStrategy
    {
        private readonly float _cameraOffset = 0.5f;
        private readonly float _maxDistance = 100f;
        private readonly float _radius = 150f;
        private readonly float _multiplier = 100f;
        private readonly int _sizeDivider = 2;

        public float FindTargetradius { get; private set; } = 300f;

        public virtual void Construct(
            ProjectileData projectileData,
            AudioPlayer audioPlayer,
            List<Transform> shotPoints)
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

        public Transform FindTargetInCrosshair(float radius)
        {
            Vector3 screenCenter = new(Screen.width / _sizeDivider, Screen.height / _sizeDivider, 0);

            Collider[] allTargets = Physics.OverlapSphere(
                Camera.main.transform.position + Camera.main.transform.forward * _multiplier,
                _radius);

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

        public Vector3 GetAimPoint()
        {
            Camera camera = Camera.main;
            Ray ray = camera.ViewportPointToRay(new Vector3(_cameraOffset, _cameraOffset, 0f));

            if (Physics.Raycast(ray, out RaycastHit hit, _maxDistance))
                return hit.point;

            return ray.origin + ray.direction * _maxDistance;
        }
    }
}