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
    public class SimpleRocketShootingStrategy : BaseShootingStrategy
    {
        private readonly int _leftPosition = -1;
        private readonly int _rightPosition = 1;
        private readonly int _oppositePosition = 2;
        private readonly int _divider = 2;
        private readonly float _sideOffset = 0.5f;
        private readonly float _delayBetweenShots = 0.4f;

        private int _currentBarrelIndex = 0;
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

                CreateFireSound(_projectileData, _audioPlayer, _firePoints);
                CreateMuzzleFlash(_projectileData, _firePoints);

                yield return new WaitForSeconds(_delayBetweenShots);
            }
        }

        private IEnumerator CreateEnergyProjectile(List<Transform> firePoints)
        {
            Vector3 aimPoint = GetAimPoint();
            bool startRight = (_currentBarrelIndex % _divider == 0);
            int baseSign = startRight ? _rightPosition : _leftPosition;

            for (int i = 0; i < _projectileData.EnergyProjectileCount; i++)
            {
                Transform point = firePoints[i % firePoints.Count];
                Vector3 spawnPos = point.position;
                Vector3 rightDir = point.right;

                int positionIndex = i % _projectileData.EnergyProjectileCount;
                int sign = (positionIndex == 0) ? baseSign : (positionIndex == _oppositePosition ? -baseSign : 0);
                spawnPos += rightDir * (_sideOffset * sign);

                Vector3 direction = (aimPoint - spawnPos).normalized;
                Quaternion rotation = Quaternion.LookRotation(direction);

                var projectile = GameObject.Instantiate(_projectileData.EnergyProjectile, spawnPos, rotation);
                projectile.Initialize(_projectileData, _audioPlayer.SfxAudioSource);

                CreateFireSound(_projectileData, _audioPlayer, firePoints);
                CreateMuzzleFlash(_projectileData, firePoints);

                yield return new WaitForSeconds(_delayBetweenShots);
            }

            _currentBarrelIndex++;
        }
    }
}
