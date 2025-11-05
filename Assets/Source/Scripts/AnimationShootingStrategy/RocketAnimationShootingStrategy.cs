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
    public class RocketAnimationShootingStrategy : BaseAnimationShootingStrategy
    {
        private readonly float _tweenDuration = 0.3f;
        private readonly float _vectorMultiplier = 5f;
        private readonly float _forwardFlyTime = 0.5f;
        private readonly float _spiralDuration = 1.8f;
        private readonly float _explosionDelay = 0.2f;
        private readonly float _spiralRadius = 1.2f;
        private readonly float _spiralSpeed = 4f;
        private readonly float _rocketOffset = 0.6f;
        private readonly float _flightHeight = 1.5f;
        private readonly float _delayBetweenRockets = 0.15f;

        private GameObject _rocketPrefab;
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
            _coroutineRunner.StartCoroutine(SpawnRocketsRoutine());
        }

        private IEnumerator SpawnRocketsRoutine()
        {
            SpawnRocket(_shotPoints[0]);

            yield return new WaitForSeconds(_delayBetweenRockets);
            //foreach (var point in _shotPoints)
            //{
            //    if (point != null)
            //    {
            //        SpawnRocket(point);

            //    }
            //}
        }

        private void SpawnRocket(Transform firePoint)
        {
            var rocket1 = GameObject.Instantiate(_rocketPrefab, firePoint.position, Quaternion.identity);
            AnimateRocket(rocket1, true);

            var rocket2 = GameObject.Instantiate(_rocketPrefab, firePoint.position, Quaternion.identity);
            AnimateRocket(rocket2, false);

            CreateFireSound(_projectileData, _audioPlayer, _shotPoints);
            CreateMuzzleFlash(_projectileData, _shotPoints);
        }

        private void AnimateRocket(GameObject rocket, bool clockwise)
        {
            Camera cam = Camera.main;
            Vector3 startPos = rocket.transform.position; // firePoint.position, это начальна€ точка ракеты
            Vector3 forwardDir = cam.transform.forward;  // направление камеры, можно заменить на огневую точку, если нужно

            // –андомное небольшое отклонение направлени€ вылета (до 15 градусов)
            float maxAngleOffset = 15f;

            Vector3 randomDir = Quaternion.Euler(
                Random.Range(-maxAngleOffset, maxAngleOffset),
                Random.Range(-maxAngleOffset, maxAngleOffset),
                0f) * forwardDir;

            float flyDistance = 7f;
            Vector3 straightTarget = startPos + randomDir * flyDistance;

            ParticleSystem particleSystem = rocket.GetComponentInChildren<ParticleSystem>();
            if (particleSystem != null)
            {
                particleSystem.Play();  // «апускаем частицы сразу при старте (вылета)
            }

            Sequence seq = DOTween.Sequence();

            // ћедленный вылет вперед с рандомным направлением (замедлен дл€ видимости дыма)
            float slowedForwardFlyTime = _forwardFlyTime * 2f;

            // ѕерва€ часть - вылет из огневой точки в выбранном направлении
            seq.Append(DOTween.To(
                () => 0f,
                t =>
                {
                    if (rocket == null) return;

                    Vector3 pos = Vector3.Lerp(startPos, straightTarget, t);
                    rocket.transform.position = pos;
                    rocket.transform.LookAt(pos + randomDir);
                },
                1f,
                slowedForwardFlyTime
            ).SetEase(Ease.OutQuad));

            // ¬тора€ часть - спиральное вращение вокруг центра

            Vector3 spiralCenter = cam.transform.position + cam.transform.forward * _vectorMultiplier;

            // ”величиваем радиус вращени€ (рандом от базового радиуса до 2х)
            float randomRadius = Random.Range(_spiralRadius, _spiralRadius * 2f);

            // Ќаправление вращени€: 1 или -1 (по/против часовой стрелки)
            float directionY = clockwise ? 1f : -1f;
            float directionX = Random.value > 0.5f ? 1f : -1f;

            float startAngleY = Random.Range(0f, Mathf.PI * 2f);
            float startAngleX = Random.Range(0f, Mathf.PI * 2f);

            float spiralSpeedY = _spiralSpeed * Random.Range(0.4f, 1f) * directionY;
            float spiralSpeedX = _spiralSpeed * Random.Range(0.4f, 1f) * directionX;

            seq.Append(DOTween.To(
                () => 0f,
                t =>
                {
                    if (rocket == null) return;

                    // —пиральные углы вращени€
                    float angleY = startAngleY + t * Mathf.PI * 2f * spiralSpeedY;
                    float angleX = startAngleX + t * Mathf.PI * 2f * spiralSpeedX * 0.5f;

                    // –адиус уменьшаетс€ к 0.3f к концу
                    float radius = Mathf.Lerp(randomRadius, 0.3f, t);

                    // ѕозици€ спирали вокруг центра (spiralCenter)
                    Vector3 offset = new Vector3(
                        Mathf.Cos(angleY) * radius,
                        Mathf.Sin(angleX) * radius * 0.5f,
                        Mathf.Sin(angleY) * radius);

                    Vector3 newPos = spiralCenter + offset;

                    rocket.transform.position = newPos;

                    // ¬ращение ракеты, чтобы смотреть на центр спирали
                    Vector3 dirToCenter = (spiralCenter - newPos).normalized;
                    Quaternion lookRot = Quaternion.LookRotation(dirToCenter, Vector3.up);
                    rocket.transform.rotation = lookRot;
                },
                1f,
                _spiralDuration
            ).SetEase(Ease.InOutSine));

            seq.AppendInterval(_explosionDelay);

            seq.AppendCallback(() =>
            {
                if (rocket == null) return;

                Vector3 explosionOffset = new Vector3(
                    Random.Range(-0.5f, 0.5f),
                    Random.Range(-0.3f, 0.3f),
                    Random.Range(-0.5f, 0.5f)
                );

                Vector3 explosionPoint = rocket.transform.position + explosionOffset;

                CreateHitSoundEffect(_projectileData, _audioPlayer);
                CreateHitEffect(_projectileData, explosionPoint);

                if (particleSystem != null)
                    particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);

                GameObject.Destroy(rocket, 1f);
            });

            seq.Play();
        }
    }
}