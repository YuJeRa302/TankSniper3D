using Assets.Source.Game.Scripts.Enemy;
using Assets.Source.Scripts.Game;
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
    public class DoubleLaserShootingStrategy : BaseShootingStrategy
    {
        private readonly float _radiusSearch = 25f;
        private readonly float _rangeMultiplier = 0.75f;
        private readonly float _minValueBounceOffset = -15f;
        private readonly float _maxValueBounceOffset = 15f;
        private readonly float _lifeTimeHitEffect = 0.5f;
        private readonly float _widthMultiplier = 0.2f;
        private readonly float _attackRange = 35f;
        private readonly float _energyLaserYOffset = -0.08f;
        private readonly float _delayBetweenShots = 0.4f;

        private ICoroutineRunner _coroutineRunner;
        private ProjectileData _projectileData;
        private List<Transform> _firePoints;
        private AudioPlayer _audioPlayer;
        private Material _material;

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
            _material = (_projectileData.EnergyProjectile as LaserBeam).Material;
        }

        public override void ShootWithEnergy(bool isVibroEnabled)
        {
            CreateEnergyProjectile();
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

        private void CreateEnergyProjectile()
        {
            Vector3 aimPoint = GetAimPoint();
            Transform mainTarget = FindTargetInCrosshair(FindTargetradius);

            Vector3 startPos = Camera.main.ViewportToWorldPoint(
                new Vector3(0.5f, 0.5f + _energyLaserYOffset, 1f));

            Vector3 direction = (aimPoint - startPos).normalized;

            var projectile = _projectileData.BaseProjectile;

            if (Physics.Raycast(startPos, direction, out RaycastHit firstHit, _attackRange))
            {
                SetHit(firstHit);
                CreateLaserTrail(startPos, firstHit.point);
                LaserBounceAttack(firstHit.point, mainTarget);
            }
            else
            {
                CreateLaserTrail(startPos, startPos + direction * _attackRange);
            }

            CreateFireSound(_projectileData, _audioPlayer, _firePoints);
            CreateMuzzleFlash(_projectileData, _firePoints);
        }

        private void CreateLaserTrail(Vector3 startPoint, Vector3 endPoint)
        {
            List<Vector3> points = new()
            {
                startPoint,
                endPoint
            };

            _coroutineRunner.StartCoroutine(DrawLaserCoroutine(points, _projectileData.LifeTime));
        }

        private Transform FindNearbyTarget(Vector3 fromPosition, float radius, Transform ignoreTarget)
        {
            Collider[] hits = Physics.OverlapSphere(fromPosition, radius);
            Transform closest = null;
            float closestDist = float.MaxValue;

            foreach (var hit in hits)
            {
                if (hit.TryGetComponent(out DamageableArea area))
                {
                    if (ignoreTarget != null && hit.transform == ignoreTarget)
                        continue;

                    float dist = Vector3.Distance(fromPosition, hit.transform.position);

                    if (dist < closestDist)
                    {
                        closest = hit.transform;
                        closestDist = dist;
                    }
                }
            }

            return closest;
        }

        private void LaserBounceAttack(Vector3 startPos, Transform previousTarget)
        {
            Vector3 currentStart = startPos;
            Transform lastTarget = previousTarget;

            for (int bounce = 0; bounce < _projectileData.EnergyProjectileCount; bounce++)
            {
                Transform nextTarget = FindNearbyTarget(currentStart, _radiusSearch, lastTarget);

                Vector3 bounceDirection;
                Vector3 endPoint;

                if (nextTarget != null)
                {
                    bounceDirection = (nextTarget.position - currentStart).normalized;

                    if (Physics.Raycast(currentStart, bounceDirection, out RaycastHit hit, _attackRange))
                    {
                        endPoint = hit.point;
                        SetHit(hit);
                    }
                    else
                    {
                        endPoint = nextTarget.position;
                    }
                }
                else
                {
                    Vector3 randomOffset = new(
                        Random.Range(-_minValueBounceOffset, _maxValueBounceOffset),
                        Random.Range(-_minValueBounceOffset, _maxValueBounceOffset),
                        Random.Range(-_minValueBounceOffset, _maxValueBounceOffset)
                    );

                    bounceDirection = (Quaternion.Euler(randomOffset) * Vector3.forward).normalized;
                    endPoint = currentStart + bounceDirection * (_attackRange * _rangeMultiplier);
                }

                CreateLaserTrail(currentStart, endPoint);
                currentStart = endPoint;
                lastTarget = nextTarget;
            }
        }

        private void SetHit(RaycastHit hit)
        {
            if (hit.collider.TryGetComponent(out DamageableArea damageableArea))
                damageableArea.ApplyDamage(_projectileData.Damage, hit.point);

            if (hit.collider.TryGetComponent(out DestructibleObjectView destructibleObjectView))
                destructibleObjectView.ApplyDamage(hit.point);

            CreateHitEffect(_projectileData, hit.point);
            CreateSoundEffect(_projectileData);
        }

        private IEnumerator DrawLaserCoroutine(List<Vector3> points, float duration)
        {
            GameObject laserObject = new("LaserLine");
            LineRenderer lineRenderer = laserObject.AddComponent<LineRenderer>();

            lineRenderer.useWorldSpace = true;
            lineRenderer.material = _material;
            lineRenderer.widthMultiplier = _widthMultiplier;
            lineRenderer.positionCount = points.Count;
            lineRenderer.SetPositions(points.ToArray());

            yield return new WaitForSeconds(duration);

            GameObject.Destroy(laserObject);
        }

        private void CreateHitEffect(ProjectileData projectileData, Vector3 hitPoint)
        {
            if (projectileData.HitEffect == null)
                return;

            var effect = GameObject.Instantiate(projectileData.HitEffect, hitPoint, Quaternion.identity);
            GameObject.Destroy(effect.gameObject, _lifeTimeHitEffect);
        }

        private void CreateSoundEffect(ProjectileData projectileData)
        {
            if (projectileData == null)
                return;

            if (projectileData.HitSound != null)
                _audioPlayer.PlayCharacterAudio(projectileData.HitSound);
        }
    }
}