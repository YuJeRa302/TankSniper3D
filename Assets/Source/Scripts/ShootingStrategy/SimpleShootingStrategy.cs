using Assets.Source.Scripts.ScriptableObjects;
using UnityEngine;

namespace Assets.Source.Scripts.ShootingStrategy
{
    public class SimpleShootingStrategy : BaseShootingStrategy
    {
        private ProjectileData _projectileData;
        private Transform _firePoint;

        public override void Construct(ProjectileData projectileData, Transform firePoint)
        {
            _projectileData = projectileData;
            _firePoint = firePoint;
        }

        public override void ShootWithEnergy(bool isVibroEnabled)
        {
            Transform target = FindTargetInCrosshair(FindTargetradius);
            var projectile = GameObject.Instantiate(_projectileData.BaseProjectile, _firePoint.position, _firePoint.rotation);
            projectile.Initialize(_projectileData);

            if (target != null)
                projectile.SetToTarget(target);

            CreateVibration(isVibroEnabled);
            CreateFireSound(_projectileData, _firePoint);
            CreateMuzzleFlash(_projectileData, _firePoint);
        }

        public override void ShootWithoutEnergy(bool isVibroEnabled)
        {
            var projectile = GameObject.Instantiate(_projectileData.BaseProjectile, _firePoint.position, _firePoint.rotation);
            projectile.Initialize(_projectileData);
            CreateVibration(isVibroEnabled);
            CreateFireSound(_projectileData, _firePoint);
            CreateMuzzleFlash(_projectileData, _firePoint);
        }
    }
}