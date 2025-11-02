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
        private readonly float _widthMultiplier = 0.1f;
        private readonly float _attackRange = 35f;

        private ICoroutineRunner _coroutineRunner;
        private ProjectileData _projectileData;
        private List<Transform> _firePoints;
        private Material _material;
        private AudioPlayer _audioPlayer;

        public override void Construct(ProjectileData projectileData, AudioPlayer audioPlayer, List<Transform> firePoints)
        {
            _projectileData = projectileData;
            _firePoints = firePoints;
            _audioPlayer = audioPlayer;
            _material = (_projectileData.BaseProjectile as LaserBeam).Material;
            _material.color = Color.blue;
            var container = SceneManager.GetActiveScene().GetSceneContainer();
            _coroutineRunner = container.Resolve<ICoroutineRunner>();
        }

        public override void ShootWithEnergy(bool isVibroEnabled)
        {
            CreateEnergyRaycast(_firePoints);
            CreateVibration(isVibroEnabled);
            CreateFireSound(_projectileData, _audioPlayer, _firePoints);
            CreateMuzzleFlash(_projectileData, _firePoints);
        }

        public override void ShootWithoutEnergy(bool isVibroEnabled)
        {
            CreateRaycast(_firePoints);
            CreateVibration(isVibroEnabled);
            CreateFireSound(_projectileData, _audioPlayer, _firePoints);
            CreateMuzzleFlash(_projectileData, _firePoints);
        }

        private void CreateRaycast(List<Transform> firePoints)
        {
            foreach (Transform firePoint in firePoints)
            {
                var projectile = _projectileData.BaseProjectile;
                projectile.Initialize(_projectileData, _audioPlayer.SfxAudioSource);

                if (Physics.Raycast(firePoint.position, firePoint.forward, out RaycastHit hit, _attackRange))
                {
                    if (hit.collider.TryGetComponent<DamageableArea>(out var damageableArea))
                        damageableArea.ApplyDamage(_projectileData.Damage, hit.point);
                }

                CreateLaserTrail(firePoint, firePoint.forward);
            }
        }

        private void CreateEnergyRaycast(List<Transform> firePoints)
        {
            foreach (Transform firePoint in firePoints)
            {
                if (Physics.Raycast(firePoint.position, firePoint.forward, out RaycastHit hit, _attackRange))
                {
                    Transform target = FindTargetInCrosshair(FindTargetradius);
                    var projectile = _projectileData.BaseProjectile;
                    projectile.Initialize(_projectileData, _audioPlayer.SfxAudioSource);
                    LaserBounceAttack(firePoint, target);
                }

                CreateLaserTrail(firePoint, firePoint.forward);
            }
        }

        private void CreateLaserTrail(Transform firePoint, Vector3 direction)
        {
            List<Vector3> points = new()
            {
                firePoint.position,
                firePoint.position + direction * _attackRange
            };

            _coroutineRunner.StartCoroutine(DrawLaserCoroutine(points, _projectileData.LifeTime));
        }

        private void LaserBounceAttack(Transform firePoint, Transform nextTarget)
        {
            List<Vector3> laserPoints = new();
            Vector3 currentPosition = firePoint.position;
            Vector3 currentDirection = firePoint.forward;

            laserPoints.Add(currentPosition);

            int bouncesDone = 0;
            int maxBounces = 1;

            while (bouncesDone <= maxBounces)
            {
                if (Physics.Raycast(currentPosition, currentDirection, out RaycastHit hit, _attackRange))
                {
                    laserPoints.Add(hit.point);

                    if (hit.collider.TryGetComponent<DamageableArea>(out var damageableArea))
                        damageableArea.ApplyDamage(_projectileData.Damage, hit.point);

                    if (bouncesDone == maxBounces || nextTarget == null)
                        break;

                    currentPosition = hit.point;
                    currentDirection = (nextTarget.position - currentPosition).normalized;
                    bouncesDone++;
                }
                else
                {
                    laserPoints.Add(currentPosition + currentDirection * _attackRange);
                    break;
                }
            }

            _coroutineRunner.StartCoroutine(DrawLaserCoroutine(laserPoints, _projectileData.LifeTime));
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
    }
}