using Assets.Source.Scripts.ScriptableObjects;
using Assets.Source.Scripts.Upgrades;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Enemy
{
    public class LaserEnemyShootingStrategy : BaseEnemyShootingStrategy
    {
        private readonly float _attackRate = 1f;
        private readonly float _attackRange = 35f;

        private Enemy _enemy;
        private ProjectileData _projectileData;
        private LineRenderer _lineRenderer;
        private List<Transform> _firePoints = new();

        public override ProjectileData ProjectileData => _projectileData;

        public override void Construct(Enemy enemy, ProjectileData projectileData, List<Transform> firePoints)
        {
            _enemy = enemy;
            _projectileData = projectileData;
            _firePoints = firePoints;
            _lineRenderer = _projectileData.BaseProjectile.GetComponent<LineRenderer>();
        }

        public override void Shoot()
        {
            if (_projectileData.BaseProjectile == null)
                return;

            CreateLaser(_firePoints);
            CreateFireSound(_projectileData, _firePoints);
            CreateMuzzleFlash(_projectileData, _firePoints);
        }

        private void CreateLaser(List<Transform> firePoints)
        {
            foreach (Transform firePoint in firePoints)
            {
                Vector3 direction = (_enemy.Player.position - firePoint.position).normalized;

                var projectile = GameObject.Instantiate(
                    _projectileData.BaseProjectile,
                    firePoint.position,
                    Quaternion.LookRotation(direction));

                projectile.Initialize(_projectileData);
                CreateLaserTrail(firePoint, direction);
                _enemy.StartCoroutine(DealDamage(_projectileData.LifeTime, firePoint, direction));
                GameObject.Destroy(projectile, _projectileData.LifeTime);
            }
        }

        private void CreateLaserTrail(Transform firePoint, Vector3 direction)
        {
            _lineRenderer.SetPosition(0, firePoint.position);
            _lineRenderer.SetPosition(1, firePoint.position + direction * _attackRange);
        }

        private void CreateRaycast(Transform firePoint, Vector3 direction)
        {
            if (Physics.Raycast(firePoint.position, direction, out RaycastHit hit, _attackRange))
            {
                if (hit.collider.TryGetComponent(out TankView tankView))
                {
                    //tankView.ApplyDamage(_projectileData.Damage);
                }
            }
        }

        private IEnumerator DealDamage(float lifeTime, Transform firePoint, Vector3 direction)
        {
            while (lifeTime > 0)
            {
                if (_enemy.IsDead == false)
                {
                    CreateRaycast(firePoint, direction);
                    yield return new WaitForSeconds(_attackRate);
                    lifeTime -= _attackRate;
                }
                else
                {
                    yield return null;
                }
            }
        }
    }
}