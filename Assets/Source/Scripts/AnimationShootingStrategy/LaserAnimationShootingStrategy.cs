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
    public class LaserAnimationShootingStrategy : BaseAnimationShootingStrategy
    {
        private readonly string _colorName = "_Color";
        private readonly Vector3 _minBounds = new(-10f, -1f, -10f);
        private readonly Vector3 _maxBounds = new(10f, 5f, 10f);
        private readonly float _tweenDuration = 0.3f;
        private readonly float _vectorMultiplier = 5f;
        private readonly float _delayBetweenLasers = 0.15f;
        private readonly float _flightDuration = 0.8f;
        private readonly float _maxSpread = 1.5f;
        private readonly float _angleMultiplier = 2f;
        private readonly float _endRadius = 0.5f;
        private readonly float _endValue = 1f;
        private readonly float _durationMultiplier = 2.2f;
        private readonly float _minSpinSpeed = 120f;
        private readonly float _maxSpinSpeed = 200f;
        private readonly float _minHeight = 1f;
        private readonly float _maxHeight = 2f;
        private readonly float _minRadius = 2.5f;
        private readonly float _maxRadius = 3.5f;

        private float _duration;
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
            _duration = _flightDuration * _durationMultiplier;
        }

        public override void Shoot()
        {
            _coroutineRunner.StartCoroutine(SpawnLasersRoutine());
        }

        private IEnumerator SpawnLasersRoutine()
        {
            int totalProjectiles = _projectileData.EnergyProjectileCount;
            int shotPointsCount = _shotPoints.Count;
            int shotIndex = 0;

            for (int i = 0; i < totalProjectiles; i++)
            {
                var point = _shotPoints[shotIndex % shotPointsCount];

                if (point != null)
                    SpawnLaser(point);

                shotIndex++;

                yield return new WaitForSeconds(_delayBetweenLasers);
            }
        }

        private void SpawnLaser(Transform firePoint)
        {
            Sequence sequence = DOTween.Sequence();

            var laser = GameObject.Instantiate(
                _projectileData.EnergyProjectile,
                firePoint.position,
                Quaternion.identity);

            CreateFireSound(_projectileData, _audioPlayer, _shotPoints);
            CreateMuzzleFlash(_projectileData, _shotPoints);

            if (laser.TryGetComponent<TrailRenderer>(out var trail))
                trail.Clear();

            CreateLaser(sequence, trail, laser);
            sequence.Play();
        }

        private void CreateLaser(Sequence sequence, TrailRenderer trailRenderer, BaseProjectile laser)
        {
            Vector3 center = Camera.main.transform.position + Camera.main.transform.forward * _vectorMultiplier;

            Vector3 explosionOffset = new(
                Random.Range(-_maxSpread, _maxSpread),
                0f,
                Random.Range(-_maxSpread, _maxSpread));

            Vector3 explosionPoint = new(center.x + explosionOffset.x, 0f, center.z + explosionOffset.z);

            float startRadius = Random.Range(_minRadius, _maxRadius);
            float rotations = Random.Range(_minRadius, _maxRadius);
            float height = Random.Range(_minHeight, _maxHeight);
            float spinSpeed = Random.Range(_minSpinSpeed, _maxSpinSpeed);

            sequence.Append(DOTween.To(
                () => 0f,
                tween =>
                {
                    if (laser == null)
                        return;

                    float angle = tween * Mathf.PI * _angleMultiplier * rotations;
                    float radius = Mathf.Lerp(startRadius, _endRadius, tween);

                    Vector3 offset = new(
                        Mathf.Cos(angle) * radius,
                        Mathf.Sin(angle * _maxSpread) * height,
                        Mathf.Sin(angle) * radius);

                    Vector3 newPos = center + offset;

                    newPos.x = Mathf.Clamp(newPos.x, _minBounds.x, _maxBounds.x);
                    newPos.y = Mathf.Clamp(newPos.y, _minBounds.y, _maxBounds.y);
                    newPos.z = Mathf.Clamp(newPos.z, _minBounds.z, _maxBounds.z);

                    laser.transform.position = newPos;
                    laser.transform.LookAt(center);
                    laser.transform.Rotate(spinSpeed * Time.deltaTime * Vector3.up, Space.World);
                },
                _endValue,
                _duration
                )
                .SetEase(Ease.InOutSine)
                .OnComplete(() =>
                {
                    if (laser == null)
                        return;

                    CreateHitSoundEffect(_projectileData, _audioPlayer);
                    CreateHitEffect(_projectileData, explosionPoint);

                    if (trailRenderer != null)
                        DoFadeTrail(trailRenderer, laser);
                    else
                        GameObject.Destroy(laser.gameObject);
                }));
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