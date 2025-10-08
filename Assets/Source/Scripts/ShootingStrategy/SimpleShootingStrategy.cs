using Assets.Source.Scripts.ScriptableObjects;
using Assets.Source.Scripts.Services;
using UnityEngine;

namespace Assets.Source.Scripts.ShootingStrategy
{
    public class SimpleShootingStrategy : IShootingStrategy
    {
        private ProjectileData _projectileData;
        private Transform _shotPoint;

        public void Construct(ProjectileData projectileData, Transform shotPoint)
        {
            _projectileData = projectileData;
            _shotPoint = shotPoint;
        }

        public void ShootWithEnergy()
        {
        }

        public void ShootWithoutEnergy()
        {
            var projectile = GameObject.Instantiate(_projectileData.BaseProjectile, _shotPoint.position, _shotPoint.rotation);
            projectile.Initialize(_projectileData);
        }
    }
}