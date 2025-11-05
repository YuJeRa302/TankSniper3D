using Assets.Source.Scripts.Projectile;
using Assets.Source.Scripts.ScriptableObjects;
using Assets.Source.Scripts.Services;
using Assets.Source.Scripts.Sound;
using DG.Tweening;
using Reflex.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Source.Scripts.AnimationShootingStrategy
{
    public class BounceLaserAnimationShootingStrategy : BaseAnimationShootingStrategy
    {
        private readonly string _colorName = "_Color";
        private readonly Vector3 _minBounds = new(-10f, -1f, -10f);
        private readonly Vector3 _maxBounds = new(10f, 5f, 10f);
        private readonly float _vectorMultiplier = 4f;
        private readonly float _speedLaserMultiplier = 0.35f;
        private readonly float _laserLengthMultiplier = 20f;
        private readonly float _durationMultiplier = 2.2f;
        private readonly float _maxSpreadMultiplier = 0.5f;
        private readonly float _tweenDuration = 0.3f;
        private readonly float _flightDuration = 0.8f;
        private readonly int _laserCount = 1;
        private readonly float _delayBetweenLasers = 0.15f;
        private readonly float _maxSpread = 3.5f;
        private readonly int _bounces = 4;
        private readonly float _endValue = 1f;
        private readonly float _divider = 2f;
        private readonly float _minSpinSpeed = 150f;
        private readonly float _maxSpinSpeed = 220f;

        private float _singleFlightDuration;
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
            _singleFlightDuration = (_flightDuration * _durationMultiplier / _bounces) * _speedLaserMultiplier;
        }

        public override void Shoot()
        {
            _coroutineRunner.StartCoroutine(SpawnLasersRoutine());
        }

        private IEnumerator SpawnLasersRoutine()
        {
            for (int i = 0; i < _laserCount; i++)
            {
                SpawnLaser(_shotPoints[0]);
                yield return new WaitForSeconds(_delayBetweenLasers);
            }
        }

        private void SpawnLaser(Transform firePoint)
        {
            Camera cam = Camera.main;
            Sequence sequence = DOTween.Sequence();
            float spinSpeed = Random.Range(_minSpinSpeed, _maxSpinSpeed);

            Vector3 camCenter = cam.transform.position + cam.transform.forward * _vectorMultiplier;
            Vector3 direction = cam.transform.forward;
            Vector3 straightTarget = camCenter + _maxSpread * _maxSpreadMultiplier * firePoint.forward;

            var laser = GameObject.Instantiate(
                _projectileData.EnergyProjectile,
                firePoint.position,
                Quaternion.identity);

            Vector3 currentScale = laser.transform.localScale;

            laser.transform.localScale = new Vector3(
                currentScale.x,
                currentScale.y,
                currentScale.z * _laserLengthMultiplier);

            CreateFireSound(_projectileData, _audioPlayer, _shotPoints);
            CreateMuzzleFlash(_projectileData, _shotPoints);

            if (laser.TryGetComponent<TrailRenderer>(out var trail))
                trail.Clear();

            CreateStartLaser(sequence, laser, firePoint, straightTarget, spinSpeed);

            for (int i = 1; i < _bounces; i++)
            {
                CreateBounceLaser(sequence, direction, spinSpeed, camCenter, laser);
            }

            DestroyLaser(sequence, trail, laser);
            sequence.Play();
        }

        private void CreateStartLaser(
            Sequence sequence,
            BaseProjectile laser,
            Transform firePoint,
            Vector3 straightTarget,
            float spinSpeed)
        {
            sequence.Append(DOTween.To(
                () => 0f,
                tween =>
                {
                    if (laser == null)
                        return;

                    Vector3 newPos = Vector3.Lerp(firePoint.position, straightTarget, tween);
                    laser.transform.position = newPos;
                    laser.transform.LookAt(straightTarget);
                    laser.transform.Rotate(spinSpeed * Time.deltaTime * Vector3.up, Space.World);
                },
                _endValue,
                _singleFlightDuration)
                .SetEase(Ease.Linear));
        }

        private void CreateBounceLaser(
            Sequence sequence,
            Vector3 direction,
            float spinSpeed,
            Vector3 camCenter,
            BaseProjectile laser)
        {
            Vector3 randomPoint = camCenter + new Vector3(
                Random.Range(-_maxSpread, _maxSpread),
                Random.Range(-_maxSpread / _divider, _maxSpread / _divider),
                Random.Range(-_maxSpread, _maxSpread));

            sequence.Append(DOTween.To(
                () => 0f,
                tween =>
                {
                    if (laser == null)
                        return;

                    Vector3 newPos = Vector3.Lerp(laser.transform.position, randomPoint, tween);
                    newPos.x = Mathf.Clamp(newPos.x, _minBounds.x, _maxBounds.x);
                    newPos.y = Mathf.Clamp(newPos.y, _minBounds.y, _maxBounds.y);
                    newPos.z = Mathf.Clamp(newPos.z, _minBounds.z, _maxBounds.z);

                    laser.transform.position = newPos;
                    laser.transform.LookAt(randomPoint);
                    laser.transform.Rotate(spinSpeed * Time.deltaTime * Vector3.up, Space.World);
                },
                _endValue,
                _singleFlightDuration
            )
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                direction = Vector3.Reflect(direction, Random.onUnitSphere);
            }));
        }

        private void DestroyLaser(Sequence sequence, TrailRenderer trailRenderer, BaseProjectile laser)
        {
            sequence.AppendCallback(() =>
            {
                if (laser == null)
                    return;

                CreateHitSoundEffect(_projectileData, _audioPlayer);

                if (trailRenderer != null)
                    DoFadeTrail(trailRenderer, laser);
                else
                    GameObject.Destroy(laser);
            });
        }

        private void DoFadeTrail(TrailRenderer trail, BaseProjectile laser)
        {
            Material trailMaterial = trail.material;

            if (trailMaterial.HasProperty(_colorName))
            {
                Color startColor = trailMaterial.color;
                Color endColor = new(startColor.r, startColor.g, startColor.b, 0f);

                trailMaterial
                    .DOColor(endColor, _colorName, _tweenDuration)
                    .OnComplete(() => GameObject.Destroy(laser));
            }
            else
            {
                GameObject.Destroy(laser);
            }
        }
    }
}