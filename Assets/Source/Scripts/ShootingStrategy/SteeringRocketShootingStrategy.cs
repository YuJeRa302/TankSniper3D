using Assets.Source.Game.Scripts.Enemy;
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
    public class SteeringRocketShootingStrategy : BaseShootingStrategy
    {
        private readonly float _radius = 150f;
        private readonly float _multiplier = 100f;
        private readonly int _leftPosition = -1;
        private readonly int _rightPosition = 1;
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
            // Смещение по высоте, чтобы ракета летела в центр врага
            float targetHeightOffset = 1.0f; // можно настроить под размер врага

            List<Transform> availableTargets = FindClosestTargets(_projectileData.EnergyProjectileCount);
            bool startRight = (_currentBarrelIndex % _divider == 0);
            int baseSign = startRight ? _rightPosition : _leftPosition;

            for (int i = 0; i < _projectileData.EnergyProjectileCount; i++)
            {
                Transform point = firePoints[i % firePoints.Count];
                Vector3 spawnPos = point.position;
                Vector3 rightDir = point.right;

                int sign = (i == 0) ? baseSign : -baseSign;
                spawnPos += rightDir * (_sideOffset * sign);

                Transform target = (i < availableTargets.Count)
                    ? availableTargets[i]
                    : (availableTargets.Count > 0 ? availableTargets[0] : null);

                Vector3 finalTargetPoint;

                if (target != null)
                {
                    // Добавляем смещение по Y к цели
                    finalTargetPoint = target.position + Vector3.up * targetHeightOffset;
                }
                else
                {
                    finalTargetPoint = GetAimPoint();
                }

                // Вычисляем направление после смещения
                Vector3 direction = (finalTargetPoint - spawnPos).normalized;
                Quaternion rotation = Quaternion.LookRotation(direction);

                // Создаём снаряд
                var projectile = GameObject.Instantiate(_projectileData.BaseProjectile, spawnPos, rotation);
                projectile.Initialize(_projectileData, _audioPlayer.SfxAudioSource);

                CreateFireSound(_projectileData, _audioPlayer, firePoints);
                CreateMuzzleFlash(_projectileData, firePoints);

                yield return new WaitForSeconds(_delayBetweenShots);
            }

            _currentBarrelIndex++;
        }

        //private IEnumerator CreateEnergyProjectile(List<Transform> firePoints)
        //{
        //    List<Transform> availableTargets = FindClosestTargets(_projectileData.EnergyProjectileCount);
        //    bool startRight = (_currentBarrelIndex % _divider == 0);
        //    int baseSign = startRight ? _rightPosition : _leftPosition;

        //    for (int i = 0; i < _projectileData.EnergyProjectileCount; i++)
        //    {
        //        Transform point = firePoints[i % firePoints.Count];
        //        Vector3 spawnPos = point.position;
        //        Vector3 rightDir = point.right;

        //        int sign = (i == 0) ? baseSign : -baseSign;
        //        spawnPos += rightDir * (_sideOffset * sign);

        //        Transform target = (i < availableTargets.Count)
        //            ? availableTargets[i]
        //            : (availableTargets.Count > 0 ? availableTargets[0] : null);

        //        Vector3 finalTargetPoint = target != null ? target.position : GetAimPoint();

        //        Vector3 direction = (finalTargetPoint - spawnPos).normalized;
        //        Quaternion rotation = Quaternion.LookRotation(direction);

        //        var projectile = GameObject.Instantiate(_projectileData.BaseProjectile, spawnPos, rotation);
        //        projectile.Initialize(_projectileData, _audioPlayer.SfxAudioSource);

        //        CreateFireSound(_projectileData, _audioPlayer, firePoints);
        //        CreateMuzzleFlash(_projectileData, firePoints);

        //        yield return new WaitForSeconds(_delayBetweenShots);
        //    }

        //    _currentBarrelIndex++;
        //}

        private List<Transform> FindClosestTargets(int maxTargets)
        {
            List<Transform> targetsList = new();

            Collider[] allTargets = Physics.OverlapSphere(
                Camera.main.transform.position + Camera.main.transform.forward * _multiplier,
                _radius);

            List<(Transform target, float distance)> targets = new();

            foreach (var collider in allTargets)
            {
                if (!collider.TryGetComponent<DamageableArea>(out var enemy))
                    continue;

                float dist = Vector3.Distance(Camera.main.transform.position, collider.transform.position);
                targets.Add((collider.transform, dist));
            }

            targets.Sort((a, b) => a.distance.CompareTo(b.distance));

            for (int i = 0; i < Mathf.Min(maxTargets, targets.Count); i++)
                targetsList.Add(targets[i].target);

            return targetsList;
        }
    }
}