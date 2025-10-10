using Assets.Source.Scripts.ScriptableObjects;
using Assets.Source.Scripts.Services;
using System.Collections;
using UnityEngine;

namespace Assets.Source.Scripts.ShootingStrategy
{
    public class DoubleShootingStrategy : IShootingStrategy
    {
        private float _delayBetweenShots = 0.5f;
        private int _shotCount = 2;
        private ProjectileData _projectileData;
        private Transform _shotPoint;

        public void Construct(ProjectileData projectileData, Transform shotPoint)
        {
            _projectileData = projectileData;
            _shotPoint = shotPoint;
        }

        public void ShootWithEnergy()
        {
            //StartCoroutine(ShootSequentially());
        }

        public void ShootWithoutEnergy()
        {
            var projectile = GameObject.Instantiate(_projectileData.BaseProjectile, _shotPoint.position, _shotPoint.rotation);
            projectile.Initialize(_projectileData);
        }

        private IEnumerator ShootSequentially()
        {
            for (int i = 0; i < _shotCount; i++)
            {
                var projectile = GameObject.Instantiate(_projectileData.BaseProjectile, _shotPoint.position, _shotPoint.rotation);
                projectile.Initialize(_projectileData);
                yield return new WaitForSeconds(_delayBetweenShots);
            }
        }
    }
}