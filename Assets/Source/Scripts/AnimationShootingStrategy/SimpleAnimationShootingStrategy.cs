using Assets.Source.Scripts.Projectile;
using Assets.Source.Scripts.ScriptableObjects;
using Assets.Source.Scripts.Services;
using Assets.Source.Scripts.Sound;
using Reflex.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Source.Scripts.AnimationShootingStrategy
{
    public class SimpleAnimationShootingStrategy : BaseAnimationShootingStrategy
    {
        private readonly float _delayBetweenRockets = 0.15f;

        private AudioPlayer _audioPlayer;
        private ProjectileData _projectileData;
        private List<Transform> _shotPoints = new();
        private ICoroutineRunner _coroutineRunner;

        public override void Construct(
            ProjectileData projectileData,
            AudioPlayer audioPlayer,
            List<Transform> shotPoints)
        {
            _projectileData = projectileData;
            _audioPlayer = audioPlayer;
            _shotPoints = shotPoints;
            var container = SceneManager.GetActiveScene().GetSceneContainer();
            _coroutineRunner = container.Resolve<ICoroutineRunner>();
        }

        public override void Shoot()
        {
            _coroutineRunner.StartCoroutine(SpawnProjectile());
        }

        private void CreateProjectile(Transform firePoint)
        {
            var projectile = GameObject.Instantiate(
                _projectileData.EnergyProjectile,
                firePoint.position,
                firePoint.rotation);

            _coroutineRunner.StartCoroutine(MoveProjectileForward(projectile, firePoint.forward));
            CreateFireSound(_projectileData, _audioPlayer, _shotPoints);
            CreateMuzzleFlash(_projectileData, _shotPoints);
        }

        private IEnumerator MoveProjectileForward(BaseProjectile projectile, Vector3 direction)
        {
            float elapsed = 0f;

            while (projectile != null && elapsed < _projectileData.LifeTime)
            {
                float step = _projectileData.Speed * Time.deltaTime;
                projectile.transform.position += direction.normalized * step;
                projectile.transform.rotation = Quaternion.LookRotation(direction);
                elapsed += Time.deltaTime;
                yield return null;
            }

            if (projectile != null)
                GameObject.Destroy(projectile.gameObject);
        }

        private IEnumerator SpawnProjectile()
        {
            int shotIndex = 0;

            for (int i = 0; i < _projectileData.EnergyProjectileCount; i++)
            {
                var point = _shotPoints[shotIndex % _shotPoints.Count];

                if (point != null)
                    CreateProjectile(point);

                shotIndex++;

                yield return new WaitForSeconds(_delayBetweenRockets);
            }
        }
    }
}