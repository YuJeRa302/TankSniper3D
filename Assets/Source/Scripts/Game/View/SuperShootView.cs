using Assets.Source.Game.Scripts.Enemy;
using Assets.Source.Scripts.Projectile;
using Assets.Source.Scripts.ShootingStrategy;
using System.Collections;
using UniRx;
using UnityEngine;

namespace Assets.Source.Scripts.Game
{
    public class SuperShootView : MonoBehaviour
    {
        [Header("Camera Settings")]
        [SerializeField] private float cameraFollowSpeed = 5f;
        [SerializeField] private Vector3 cameraOffset = new(0, 2f, -5f);
        [SerializeField] private float explosionWaitTime = 0.5f;
        [SerializeField] private float returnDuration = 1.2f;
        [SerializeField] private float orbitRadius = 6f;
        [SerializeField] private float orbitHeight = 2f;
        [SerializeField] private float orbitSpeed = 10f;

        private DamageableArea _target;
        private Camera _mainCamera;
        private CompositeDisposable _disposables = new();
        private Vector3 _targetPosition;
        private bool _isOrbiting = false;
        private bool _hasCriticalTarget = false;

        private void Awake()
        {
            _mainCamera = Camera.main;

            SniperScopeView.Message
                .Receive<M_CriticalShoot>()
                .Subscribe(m => OnCriticalShoot(m.HitArea))
                .AddTo(_disposables);

            BaseShootingStrategy.Message
                .Receive<M_GetCriticalProjectile>()
                .Subscribe(m => StartCoroutine(FollowProjectile(m.BaseProjectile)))
                .AddTo(_disposables);
        }

        private void OnCriticalShoot(DamageableArea damageableArea)
        {
            _target = damageableArea;

            if (_target != null)
                _targetPosition = _target.transform.position;

            _hasCriticalTarget = true;
        }

        private IEnumerator FollowProjectile(BaseProjectile projectile)
        {
            float waitTime = 0f;
            projectile.SetSlowSpeed();

            while (!_hasCriticalTarget && waitTime < 1f)
            {
                waitTime += Time.deltaTime;
                yield return null;
            }

            if (projectile == null || !_hasCriticalTarget)
                yield break;

            MotionChanger.Message.Publish(new M_SlowMotionStarted());

            while (projectile != null)
            {
                // Позиция, где камера должна находиться
                Vector3 desiredPos = projectile.transform.position
                    + projectile.transform.forward * cameraOffset.z
                    + Vector3.up * cameraOffset.y;

                // Чем дальше камера от снаряда, тем быстрее она догоняет
                float distance = Vector3.Distance(_mainCamera.transform.position, desiredPos);
                float dynamicSpeed = Mathf.Clamp(cameraFollowSpeed + distance * 8f, cameraFollowSpeed, 120f);

                // Плавное приближение к нужной позиции
                _mainCamera.transform.position = Vector3.MoveTowards(
                    _mainCamera.transform.position,
                    desiredPos,
                    dynamicSpeed * Time.unscaledDeltaTime
                );

                // Камера смотрит не прямо на снаряд, а немного вперёд его траектории
                Vector3 predictedPos = projectile.transform.position + projectile.transform.forward * 2f;
                _mainCamera.transform.LookAt(predictedPos);

                yield return null;

                //Vector3 desiredPos = projectile.transform.position
                //    + projectile.transform.forward * cameraOffset.z
                //    + Vector3.up * cameraOffset.y;

                //_mainCamera.transform.position = Vector3.MoveTowards(
                //    _mainCamera.transform.position,
                //    desiredPos,
                //    cameraFollowSpeed * Time.unscaledDeltaTime
                //);

                //_mainCamera.transform.LookAt(projectile.transform);

                //yield return null;
            }

            yield return new WaitForSecondsRealtime(explosionWaitTime);
            MotionChanger.Message.Publish(new M_SlowMotionEnded());
            StartCoroutine(OrbitAroundExplosion(_targetPosition));
        }

        //private IEnumerator FollowProjectile(Transform projectile)
        //{
        //    // Ждём, пока мы действительно получили цель
        //    float waitTime = 0f;
        //    while (!_hasCriticalTarget && waitTime < 1f)
        //    {
        //        waitTime += Time.deltaTime;
        //        yield return null;
        //    }

        //    if (projectile == null || !_hasCriticalTarget)
        //        yield break;

        //    MotionChanger.Message.Publish(new M_SlowMotionStarted());

        //    while (projectile != null)
        //    {
        //        Vector3 desiredPos = projectile.position
        //            + projectile.forward * cameraOffset.z
        //            + Vector3.up * cameraOffset.y;

        //        _mainCamera.transform.position = Vector3.Lerp(
        //            _mainCamera.transform.position,
        //            desiredPos,
        //            Time.deltaTime * cameraFollowSpeed
        //        );

        //        _mainCamera.transform.LookAt(projectile);
        //        yield return null;
        //    }

        //    Vector3 explosionPoint = _target != null ? _target.transform.position : _targetPosition;
        //    yield return new WaitForSecondsRealtime(explosionWaitTime);

        //    MotionChanger.Message.Publish(new M_SlowMotionEnded());
        //    StartCoroutine(OrbitAroundExplosion(explosionPoint));
        //}

        private IEnumerator OrbitAroundExplosion(Vector3 center)
        {
            if (_mainCamera == null)
                yield break;

            _isOrbiting = true;

            float angle = 0f;

            while (_isOrbiting)
            {
                angle += orbitSpeed * Time.unscaledDeltaTime;
                float rad = angle * Mathf.Deg2Rad;

                Vector3 offset = new(
                    Mathf.Cos(rad) * orbitRadius,
                    orbitHeight,
                    Mathf.Sin(rad) * orbitRadius
                );

                _mainCamera.transform.position = center + offset;
                _mainCamera.transform.LookAt(center);

                yield return null;
            }
        }

        private void OnDestroy() => _disposables?.Dispose();
    }
}