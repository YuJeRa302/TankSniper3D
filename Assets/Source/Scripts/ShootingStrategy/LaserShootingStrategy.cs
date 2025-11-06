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
    public class LaserShootingStrategy : BaseShootingStrategy
    {
        private readonly float _minPosValue = 0;
        private readonly float _maxPosValue = 1;
        private readonly float _targetHeightOffset = 1.0f;
        private readonly float _radius = 150f;
        private readonly float _multiplier = 100f;
        private readonly int _leftPosition = -1;
        private readonly int _rightPosition = 1;
        private readonly int _divider = 2;
        private readonly float _delayBetweenShots = 0.4f;
        private readonly float _sideOffset = 0.5f;

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
            bool startRight = (_currentBarrelIndex % _divider == 0);
            int baseSign = startRight ? _rightPosition : _leftPosition;

            for (int i = 0; i < _projectileData.EnergyProjectileCount; i++)
            {
                List<Transform> visibleTargets = FindVisibleTargetsInCamera(_projectileData.EnergyProjectileCount);

                Transform point = firePoints[i % firePoints.Count];
                Vector3 spawnPos = point.position;
                Vector3 rightDir = point.right;

                int sign = (i == 0) ? baseSign : -baseSign;
                spawnPos += rightDir * (_sideOffset * sign);

                Transform target = (i < visibleTargets.Count)
                    ? visibleTargets[i]
                    : (visibleTargets.Count > 0 ? visibleTargets[0] : null);

                Vector3 finalTargetPoint = target != null
                    ? target.position + Vector3.up * _targetHeightOffset
                    : GetAimPoint();

                Vector3 direction = (finalTargetPoint - spawnPos).normalized;
                Quaternion rotation = Quaternion.LookRotation(direction);

                var projectile = GameObject.Instantiate(_projectileData.BaseProjectile, spawnPos, rotation);
                projectile.Initialize(_projectileData, _audioPlayer.SfxAudioSource);

                CreateFireSound(_projectileData, _audioPlayer, firePoints);
                CreateMuzzleFlash(_projectileData, firePoints);

                yield return new WaitForSeconds(_delayBetweenShots);
            }

            _currentBarrelIndex++;
        }

        private List<Transform> FindVisibleTargetsInCamera(int maxTargets)
        {
            List<Transform> visibleTargets = new();

            Collider[] allTargets = Physics.OverlapSphere(
                Camera.main.transform.position + Camera.main.transform.forward * _multiplier,
                _radius);

            List<(Transform target, float screenCenterDist, float worldDist)> candidates = new();

            foreach (var collider in allTargets)
            {
                if (!collider.TryGetComponent<DamageableArea>(out var enemy))
                    continue;

                Transform enemyTransform = collider.transform;
                Vector3 viewportPos = Camera.main.WorldToViewportPoint(enemyTransform.position);

                if (!GetStateTarget(viewportPos))
                    continue;

                float screenCenterDist = Vector2.Distance(
                    new(0.5f, 0.5f),
                    new(viewportPos.x, viewportPos.y)
                );

                float worldDist = Vector3.Distance(Camera.main.transform.position, enemyTransform.position);
                candidates.Add((enemyTransform, screenCenterDist, worldDist));
            }

            candidates.Sort((a, b) =>
            {
                int screenCompare = a.screenCenterDist.CompareTo(b.screenCenterDist);
                return screenCompare != 0 ? screenCompare : a.worldDist.CompareTo(b.worldDist);
            });

            for (int i = 0; i < Mathf.Min(maxTargets, candidates.Count); i++)
                visibleTargets.Add(candidates[i].target);

            return visibleTargets;
        }

        private bool GetStateTarget(Vector3 viewportPos)
        {
            return viewportPos.z > _minPosValue
                && viewportPos.x > _minPosValue
                && viewportPos.x < _maxPosValue
                && viewportPos.y > _minPosValue
                && viewportPos.y < _maxPosValue;
        }
    }
}