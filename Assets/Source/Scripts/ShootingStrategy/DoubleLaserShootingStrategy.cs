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
    public class DoubleLaserShootingStrategy : BaseShootingStrategy
    {
        private readonly float _doubleShotDelay = 0.2f;
        private readonly float _widthMultiplier = 0.1f;
        private readonly float _attackRange = 35f;
        private readonly float _shotCount = 2;

        private ICoroutineRunner _coroutineRunner;
        private ProjectileData _projectileData;
        private Transform _firePoint;
        private Material _material;
        private AudioPlayer _audioPlayer;

        public override void Construct(ProjectileData projectileData, AudioPlayer audioPlayer, Transform firePoint)
        {
            _projectileData = projectileData;
            _firePoint = firePoint;
            _audioPlayer = audioPlayer;
            _material = (_projectileData.BaseProjectile as LaserBeam).Material;
            _material.color = Color.blue;
            var container = SceneManager.GetActiveScene().GetSceneContainer();
            _coroutineRunner = container.Resolve<ICoroutineRunner>();
        }

        public override void ShootWithEnergy(bool isVibroEnabled)
        {
            Transform target = FindTargetInCrosshair(FindTargetradius);
            var projectile = _projectileData.BaseProjectile;
            projectile.Initialize(_projectileData, _audioPlayer.SfxAudioSource);
            LaserBounceAttack(_firePoint, target);
            CreateVibration(isVibroEnabled);
            CreateFireSound(_projectileData, _audioPlayer);
            CreateMuzzleFlash(_projectileData, _firePoint);
        }

        public override void ShootWithoutEnergy(bool isVibroEnabled)
        {
            var projectile = _projectileData.BaseProjectile;
            projectile.Initialize(_projectileData, _audioPlayer.SfxAudioSource);
            CreateVibration(isVibroEnabled);

            _coroutineRunner.StartCoroutine(DoubleLaserShot());
        }

        private void CreateRaycast()
        {
            if (Physics.Raycast(_firePoint.position, _firePoint.forward, out RaycastHit hit, _attackRange))
            {
                if (hit.collider.TryGetComponent<DamageableArea>(out var damageableArea))
                    damageableArea.ApplyDamage(_projectileData.Damage, hit.point);
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

        private IEnumerator DoubleLaserShot()
        {
            int shotsFired = 0;

            while (shotsFired < _shotCount)
            {
                CreateRaycast();
                CreateLaserTrail(_firePoint, _firePoint.forward);
                CreateFireSound(_projectileData, _audioPlayer);
                CreateMuzzleFlash(_projectileData, _firePoint);
                shotsFired++;

                if (shotsFired < _shotCount)
                    yield return new WaitForSeconds(_doubleShotDelay);
            }
        }
    }
}