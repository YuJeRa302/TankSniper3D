using Assets.Source.Scripts.Projectile;
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
            _coroutineRunner.StartCoroutine(CreateProjectile(
                _firePoints,
                _projectileData.EnergyProjectileCount,
                _projectileData.EnergyProjectile));

            CreateVibration(isVibroEnabled);
        }

        public override void ShootWithoutEnergy(bool isVibroEnabled)
        {
            _coroutineRunner.StartCoroutine(CreateProjectile(
                _firePoints,
                _projectileData.ProjectileCount,
                _projectileData.BaseProjectile));

            CreateVibration(isVibroEnabled);
        }

        private IEnumerator CreateProjectile(List<Transform> firePoints, int projectileCount, BaseProjectile baseProjectile)
        {
            Vector3 aimPoint = GetAimPoint();
            int shotIndex = 0;

            for (int i = 0; i < projectileCount; i++)
            {
                var point = firePoints[shotIndex % firePoints.Count];

                shotIndex++;
                Vector3 direction = (aimPoint - point.position).normalized;
                Quaternion rotation = Quaternion.LookRotation(direction);

                var projectile = GameObject.Instantiate(baseProjectile, point.position, rotation);
                projectile.Initialize(_projectileData, _audioPlayer.SfxAudioSource);

                GetCriticalProjectile(projectile);
                CreateFireSound(_projectileData, _audioPlayer, _firePoints);
                CreateMuzzleFlash(_projectileData, _firePoints);

                yield return new WaitForSeconds(_delayBetweenShots);
            }
        }
    }
}