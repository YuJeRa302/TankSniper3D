using Assets.Source.Scripts.ScriptableObjects;
using Assets.Source.Scripts.Sound;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Scripts.ShootingStrategy
{
    public class SimpleShootingStrategy : BaseShootingStrategy
    {
        private ProjectileData _projectileData;
        private List<Transform> _firePoints;
        private AudioPlayer _audioPlayer;

        public override void Construct(
            ProjectileData projectileData,
            AudioPlayer audioPlayer,
            List<Transform> firePoints)
        {
            _projectileData = projectileData;
            _firePoints = firePoints;
            _audioPlayer = audioPlayer;
        }

        public override void ShootWithEnergy(bool isVibroEnabled)
        {
            CreateEnergyProjectile(_firePoints);
            CreateVibration(isVibroEnabled);
            CreateFireSound(_projectileData, _audioPlayer, _firePoints);
            CreateMuzzleFlash(_projectileData, _firePoints);
        }

        public override void ShootWithoutEnergy(bool isVibroEnabled)
        {
            CreateProjectile(_firePoints);
            CreateVibration(isVibroEnabled);
            CreateFireSound(_projectileData, _audioPlayer, _firePoints);
            CreateMuzzleFlash(_projectileData, _firePoints);
        }

        private void CreateProjectile(List<Transform> firePoints)
        {
            Vector3 aimPoint = GetAimPoint();

            foreach (Transform firePoint in firePoints)
            {
                Vector3 direction = (aimPoint - firePoint.position).normalized;
                Quaternion rotation = Quaternion.LookRotation(direction);

                var projectile = GameObject.Instantiate(
                    _projectileData.BaseProjectile,
                    firePoint.position,
                    rotation
                );

                projectile.Initialize(_projectileData, _audioPlayer.SfxAudioSource);
            }
        }

        private void CreateEnergyProjectile(List<Transform> firePoints)
        {
            Vector3 aimPoint = GetAimPoint();
            Transform target = FindTargetInCrosshair(FindTargetradius);

            foreach (Transform firePoint in firePoints)
            {
                Vector3 finalTargetPoint = target != null ? target.position : aimPoint;
                Vector3 direction = (finalTargetPoint - firePoint.position).normalized;
                Quaternion rotation = Quaternion.LookRotation(direction);

                var projectile = GameObject.Instantiate(_projectileData.BaseProjectile, firePoint.position, rotation);
                projectile.Initialize(_projectileData, _audioPlayer.SfxAudioSource);
            }
        }

        private Vector3 GetAimPoint(float maxDistance = 1000f)
        {
            var camera = Camera.main;
            Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
                return hit.point;

            return ray.origin + ray.direction * maxDistance;
        }
    }
}