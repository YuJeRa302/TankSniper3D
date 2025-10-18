using Assets.Source.Game.Scripts.Enemy;
using Assets.Source.Scripts.Projectile;
using Assets.Source.Scripts.ScriptableObjects;
using Assets.Source.Scripts.Services;
using Reflex.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Source.Scripts.ShootingStrategy
{
    public class LaserShootingStrategy : BaseShootingStrategy
    {
        private readonly float _laserDelay = 0.2f;
        private readonly float _widthMultiplier = 0.1f;
        private readonly float _attackRange = 35f;

        private ICoroutineRunner _coroutineRunner;
        private ProjectileData _projectileData;
        private Transform _firePoint;
        private Material _material;

        public override void Construct(ProjectileData projectileData, Transform firePoint)
        {
            _projectileData = projectileData;
            _firePoint = firePoint;
            _material = (_projectileData.BaseProjectile as LaserBeam).Material;
            _material.color = Color.blue;
            var container = SceneManager.GetActiveScene().GetSceneContainer();
            _coroutineRunner = container.Resolve<ICoroutineRunner>();
        }

        public override void ShootWithEnergy()
        {
            Transform target = FindTargetInCrosshair(FindTargetradius);
            var projectile = _projectileData.BaseProjectile;
            projectile.Initialize(_projectileData);
            LaserBounceAttack(_firePoint, target);
            CreateFireSound(_projectileData, _firePoint);
            CreateMuzzleFlash(_projectileData, _firePoint);
        }

        public override void ShootWithoutEnergy()
        {
            var projectile = _projectileData.BaseProjectile;
            projectile.Initialize(_projectileData);
            CreateRaycast();
            CreateLaserTrail(_firePoint, _firePoint.forward);
            CreateFireSound(_projectileData, _firePoint);
            CreateMuzzleFlash(_projectileData, _firePoint);
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
            GameObject laserObject = new("LaserLine");
            LineRenderer lineRenderer = laserObject.AddComponent<LineRenderer>();
            lineRenderer.useWorldSpace = true;
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, firePoint.position);
            lineRenderer.SetPosition(1, firePoint.position + direction * _attackRange);
            lineRenderer.material = _material;
            lineRenderer.widthMultiplier = _widthMultiplier;
            GameObject.Destroy(laserObject, _projectileData.LifeTime);
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
                    laserPoints.Add(currentPosition + currentDirection * 50f);
                    break;
                }
            }

            _coroutineRunner.StartCoroutine(DrawLaser(laserPoints));
        }

        private IEnumerator DrawLaser(List<Vector3> points)
        {
            GameObject laserObject = new("LaserBeam");
            LineRenderer lineRenderer = laserObject.AddComponent<LineRenderer>();

            lineRenderer.useWorldSpace = true;
            lineRenderer.material = _material;
            lineRenderer.widthMultiplier = _widthMultiplier;
            lineRenderer.positionCount = points.Count;
            lineRenderer.SetPositions(points.ToArray());

            yield return new WaitForSeconds(_laserDelay);

            GameObject.Destroy(laserObject);
        }
    }
}