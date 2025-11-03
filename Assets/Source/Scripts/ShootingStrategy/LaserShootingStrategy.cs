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
    public class LaserShootingStrategy : BaseShootingStrategy
    {
        private readonly float _lifeTimeHitEffect = 0.5f;
        private readonly float _laserDelay = 0.4f;
        private readonly float _widthMultiplier = 0.2f;
        private readonly float _attackRange = 35f;
        private readonly float _energyLaserYOffset = -0.08f;
        private readonly float _laserYOffset = -0.1f;

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
            CreateEnergyRaycast();
            CreateVibration(isVibroEnabled);
        }

        public override void ShootWithoutEnergy(bool isVibroEnabled)
        {
            if (_firePoints.Count > 1)
                _coroutineRunner.StartCoroutine(CreateDoubleLaser(_firePoints));
            else
                CreateLaser();

            CreateVibration(isVibroEnabled);
        }

        private void CreateLaser()
        {
            Vector3 startPos = new(
                _firePoints[0].position.x,
                _firePoints[0].position.y + _laserYOffset,
                _firePoints[0].position.z);

            Vector3 aimPoint = GetAimPoint();
            Vector3 direction = (aimPoint - startPos).normalized;

            var projectile = _projectileData.BaseProjectile;
            projectile.Initialize(_projectileData, _audioPlayer.SfxAudioSource);

            if (Physics.Raycast(startPos, direction, out RaycastHit hit, _attackRange))
                SetHit(hit);

            CreateLaserTrail(startPos, startPos + direction * _attackRange);
            CreateFireSound(_projectileData, _audioPlayer, _firePoints);
            CreateMuzzleFlash(_projectileData, _firePoints);
        }

        private IEnumerator CreateDoubleLaser(List<Transform> firePoints)
        {
            Camera camera = Camera.main;

            Vector3 aimPoint = camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 15));

            foreach (Transform firePoint in firePoints)
            {
                var projectile = _projectileData.BaseProjectile;
                projectile.Initialize(_projectileData, _audioPlayer.SfxAudioSource);

                Vector3 direction = (aimPoint - firePoint.position).normalized;

                if (Physics.Raycast(firePoint.position, direction, out RaycastHit hit, _attackRange))
                {
                    SetHit(hit);
                    CreateLaserTrail(firePoint.position, hit.point);
                }
                else
                {
                    CreateLaserTrail(firePoint.position, firePoint.position + direction * _attackRange);
                }

                yield return new WaitForSeconds(_laserDelay);
            }

            CreateFireSound(_projectileData, _audioPlayer, _firePoints);
            CreateMuzzleFlash(_projectileData, _firePoints);
        }

        private void CreateEnergyRaycast()
        {
            Vector3 aimPoint = GetAimPoint();
            Transform mainTarget = FindTargetInCrosshair(FindTargetradius);

            Vector3 startPos = Camera.main.ViewportToWorldPoint(
                new Vector3(0.5f, 0.5f + _energyLaserYOffset, 1f));

            Vector3 direction = (aimPoint - startPos).normalized;

            var projectile = _projectileData.BaseProjectile;
            projectile.Initialize(_projectileData, _audioPlayer.SfxAudioSource);

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

        private void CreateHitEffect(ProjectileData projectileData, Vector3 hitPoint)
        {
            if (projectileData.HitEffect == null)
                return;

            var effect = GameObject.Instantiate(projectileData.HitEffect, hitPoint, Quaternion.identity);
            GameObject.Destroy(effect.gameObject, _lifeTimeHitEffect);
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
            Transform nextTarget = FindNearbyTarget(startPos, 25f, previousTarget);

            Vector3 bounceDirection;
            Vector3 endPoint;

            if (nextTarget != null)
            {
                bounceDirection = (nextTarget.position - startPos).normalized;

                if (Physics.Raycast(startPos, bounceDirection, out RaycastHit hit, _attackRange))
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
                bounceDirection = (Quaternion.Euler(-30f, 20f, 0f) * Vector3.forward).normalized;
                endPoint = startPos + bounceDirection * (_attackRange * 0.75f);
            }

            CreateLaserTrail(startPos, endPoint);
        }

        private void SetHit(RaycastHit hit)
        {
            if (hit.collider.TryGetComponent(out DamageableArea damageableArea))
                damageableArea.ApplyDamage(_projectileData.Damage, hit.point);

            if (hit.collider.TryGetComponent(out DestructibleObjectView destructibleObjectView))
                destructibleObjectView.ApplyDamage(hit.point);

            CreateHitEffect(_projectileData, hit.point);
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