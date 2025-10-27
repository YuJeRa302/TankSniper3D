using Assets.Source.Scripts.ScriptableObjects;
using Assets.Source.Scripts.Sound;
using UnityEngine;

namespace Assets.Source.Scripts.ShootingStrategy
{
    public class SimpleShootingStrategy : BaseShootingStrategy
    {
        private ProjectileData _projectileData;
        private Transform _firePoint;
        private AudioPlayer _audioPlayer;

        public override void Construct(ProjectileData projectileData, AudioPlayer audioPlayer, Transform firePoint)
        {
            _projectileData = projectileData;
            _firePoint = firePoint;
            _audioPlayer = audioPlayer;
        }

        public override void ShootWithEnergy(bool isVibroEnabled)
        {
            Transform target = FindTargetInCrosshair(FindTargetradius);
            var projectile = GameObject.Instantiate(_projectileData.BaseProjectile, _firePoint.position, _firePoint.rotation);
            projectile.Initialize(_projectileData, _audioPlayer.SfxAudioSource);

            if (target != null)
                projectile.SetToTarget(target);

            CreateVibration(isVibroEnabled);
            CreateFireSound(_projectileData, _audioPlayer);
            CreateMuzzleFlash(_projectileData, _firePoint);
        }

        public override void ShootWithoutEnergy(bool isVibroEnabled)
        {
            var projectile = GameObject.Instantiate(_projectileData.BaseProjectile, _firePoint.position, _firePoint.rotation);
            projectile.Initialize(_projectileData, _audioPlayer.SfxAudioSource);
            CreateVibration(isVibroEnabled);
            CreateFireSound(_projectileData, _audioPlayer);
            CreateMuzzleFlash(_projectileData, _firePoint);
        }
    }
}