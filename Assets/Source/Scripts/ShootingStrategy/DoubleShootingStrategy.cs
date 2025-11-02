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
    public class DoubleShootingStrategy : BaseShootingStrategy
    {
        private float _delayBetweenShots = 0.5f;
        private int _shotCount = 2;
        private ProjectileData _projectileData;
        private Transform _firePoint;
        private AudioPlayer _audioPlayer;
        private ICoroutineRunner _coroutineRunner;

        public override void Construct(ProjectileData projectileData, AudioPlayer audioPlayer, List<Transform> firePoints)
        {
            //_projectileData = projectileData;
            //_firePoint = firePoint;
            //_audioPlayer = audioPlayer;
            //var container = SceneManager.GetActiveScene().GetSceneContainer();
            //_coroutineRunner = container.Resolve<ICoroutineRunner>();
        }

        //public override void ShootWithEnergy(bool isVibroEnabled)
        //{
        //    CreateVibration(isVibroEnabled);
        //    _coroutineRunner.StartCoroutine(DelayedDoubleShot());
        //}

        //public override void ShootWithoutEnergy(bool isVibroEnabled)
        //{
        //    var projectile = GameObject.Instantiate(_projectileData.BaseProjectile, _firePoint.position, _firePoint.rotation);
        //    projectile.Initialize(_projectileData, _audioPlayer.SfxAudioSource);
        //    CreateVibration(isVibroEnabled);
        //    CreateFireSound(_projectileData, _audioPlayer);
        //    CreateMuzzleFlash(_projectileData, _firePoint);
        //}

        //private IEnumerator DelayedDoubleShot()
        //{
        //    int shotsFired = 0;

        //    while (shotsFired < _shotCount)
        //    {
        //        Transform target = FindTargetInCrosshair(FindTargetradius);
        //        var projectile = GameObject.Instantiate(_projectileData.BaseProjectile, _firePoint.position, _firePoint.rotation);
        //        projectile.Initialize(_projectileData, _audioPlayer.SfxAudioSource);

        //        if (target != null)
        //            projectile.SetToTarget(target);

        //        CreateFireSound(_projectileData, _audioPlayer);
        //        CreateMuzzleFlash(_projectileData, _firePoint);

        //        shotsFired++;

        //        if (shotsFired < _shotCount)
        //            yield return new WaitForSeconds(_delayBetweenShots);
        //    }
        //}
    }
}