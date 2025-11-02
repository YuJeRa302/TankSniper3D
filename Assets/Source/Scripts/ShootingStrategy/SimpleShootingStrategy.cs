using Assets.Source.Scripts.ScriptableObjects;
using Assets.Source.Scripts.Services;
using Assets.Source.Scripts.Sound;
using Reflex.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Source.Scripts.ShootingStrategy
{
    public class SimpleShootingStrategy : BaseShootingStrategy
    {
        private readonly float _delayBetweenShots = 0.4f;

        private ICoroutineRunner _coroutineRunner;
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
            var container = SceneManager.GetActiveScene().GetSceneContainer();
            _coroutineRunner = container.Resolve<ICoroutineRunner>();
        }

        public override void ShootWithEnergy(bool isVibroEnabled)
        {
            _coroutineRunner.StartCoroutine(CreateEnergyProjectile(_firePoints));
            CreateVibration(isVibroEnabled);

        }

        public override void ShootWithoutEnergy(bool isVibroEnabled)
        {
            _coroutineRunner.StartCoroutine(CreateProjectile(_firePoints));
            CreateVibration(isVibroEnabled);
        }

        private IEnumerator CreateProjectile(List<Transform> firePoints)
        {
            Vector3 aimPoint = GetAimPoint();

            foreach (Transform firePoint in firePoints)
            {
                Vector3 direction = (aimPoint - firePoint.position).normalized;
                Quaternion rotation = Quaternion.LookRotation(direction);

                var projectile = GameObject.Instantiate(_projectileData.BaseProjectile, firePoint.position, rotation);
                projectile.Initialize(_projectileData, _audioPlayer.SfxAudioSource);

                CreateFireSound(_projectileData, _audioPlayer, _firePoints);
                CreateMuzzleFlash(_projectileData, _firePoints);

                yield return new WaitForSeconds(_delayBetweenShots);
            }
        }

        private IEnumerator CreateEnergyProjectile(List<Transform> firePoints)
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

                CreateFireSound(_projectileData, _audioPlayer, _firePoints);
                CreateMuzzleFlash(_projectileData, _firePoints);

                yield return new WaitForSeconds(_delayBetweenShots);
            }
        }
    }
}