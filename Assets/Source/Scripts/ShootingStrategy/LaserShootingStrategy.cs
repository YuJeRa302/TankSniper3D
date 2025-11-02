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
        private readonly float _widthMultiplier = 0.2f;
        private readonly float _attackRange = 35f;

        private ICoroutineRunner _coroutineRunner;
        private ProjectileData _projectileData;
        private List<Transform> _firePoints;
        private Material _material;
        private AudioPlayer _audioPlayer;

        public override void Construct(
            ProjectileData projectileData,
            AudioPlayer audioPlayer,
            List<Transform> firePoints)
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
        }

        public override void ShootWithoutEnergy(bool isVibroEnabled)
        {
            CreateRaycast(_firePoints);
            CreateVibration(isVibroEnabled);
        }

        private void CreateRaycast(List<Transform> firePoints)
        {
            Vector3 aimPoint = GetAimPoint();

            foreach (Transform firePoint in firePoints)
            {
                Vector3 direction = (aimPoint - firePoint.position).normalized;

                var projectile = _projectileData.BaseProjectile;
                projectile.Initialize(_projectileData, _audioPlayer.SfxAudioSource);

                if (Physics.Raycast(firePoint.position, direction, out RaycastHit hit, _attackRange))
                {
                    if (hit.collider.TryGetComponent(out DamageableArea damageableArea))
                        damageableArea.ApplyDamage(_projectileData.Damage, hit.point);

                    CreateLaserTrail(firePoint, hit.point);
                }
                else
                {
                    CreateLaserTrail(firePoint, firePoint.position + direction * _attackRange);
                }
            }

            CreateFireSound(_projectileData, _audioPlayer, _firePoints);
            CreateMuzzleFlash(_projectileData, _firePoints);
        }

        private void CreateEnergyRaycast(List<Transform> firePoints)
        {
            Vector3 aimPoint = GetAimPoint();
            Transform target = FindTargetInCrosshair(FindTargetradius);

            foreach (Transform firePoint in firePoints)
            {
                Vector3 direction = (target != null
                    ? (target.position - firePoint.position)
                    : (aimPoint - firePoint.position)).normalized;

                var projectile = _projectileData.BaseProjectile;
                projectile.Initialize(_projectileData, _audioPlayer.SfxAudioSource);

                if (target != null)
                    LaserBounceAttack(firePoint, target);
                else
                    CreateLaserTrail(firePoint, firePoint.position + direction * _attackRange);
            }

            CreateFireSound(_projectileData, _audioPlayer, _firePoints);
            CreateMuzzleFlash(_projectileData, _firePoints);
        }

        private void CreateLaserTrail(Transform firePoint, Vector3 endPoint)
        {
            List<Vector3> points = new()
            {
                firePoint.position,
                endPoint
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