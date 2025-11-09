using Assets.Source.Game.Scripts.Enemy;
using DG.Tweening;
using UnityEngine;

namespace Assets.Source.Scripts.Game
{
    public class DroneView : MonoBehaviour
    {
        private readonly float _droneYpostionValue = 3f;
        private readonly float _divider = 1.5f;
        private readonly int _loopTweenValue = -1;

        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _hitSound;
        [SerializeField] private AudioClip _moveSound;
        [Space(20)]
        [SerializeField] private ParticleSystem _exploudParticle;
        [SerializeField] private float _exploudLifeTime;
        [Space(20)]
        [SerializeField] private Camera _droneCamera;
        [Space(20)]
        [SerializeField] private float _droneSpeed = 10f;
        [SerializeField] private float _droneLifeTime = 5f;
        [SerializeField] private float _rotationSpeed = 5f;
        [SerializeField] private int _damage = 100;
        [SerializeField] private int _health = 250;
        [Space(20)]
        [SerializeField] private DroneHealth _droneHealth;
        [Space(20)]
        [SerializeField] private float _findEnemyRange = 2f;
        [Header("Spawn Animation Settings")]
        [SerializeField] private float _spawnHeight = 6f;
        [SerializeField] private float _flyUpDuration = 1.5f;
        [SerializeField] private float _hoverAmplitude = 0.15f;
        [SerializeField] private float _hoverFrequency = 0.5f;
        [SerializeField] private float _tiltAngle = 3f;

        private Transform _droneSpawnPoint;
        private Collider[] _foundEnemyColliders = new Collider[50];
        private Tween _hoverTween;
        private Camera _freeLookCamera;
        private float _baseY;
        private bool _isHovering;
        private bool _isFlying = false;

        public Camera DroneCamera => _droneCamera;
        public float DroneSpeed => _droneSpeed;
        public float RotationSpeed => _rotationSpeed;

        private void OnDestroy()
        {
            _hoverTween?.Kill();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, _findEnemyRange);
        }

        private void Update()
        {
            CheckForCollisions();

            if (_isFlying == false)
                RotateToCameraCenter();
        }

        public void Initialize(Transform spawnPoint)
        {
            _droneSpawnPoint = spawnPoint;
            _freeLookCamera = Camera.main;
            AnimateDroneSpawn();
        }

        public void DisableHover()
        {
            _hoverTween?.Kill();
            _isHovering = true;
            _isFlying = true;
        }

        private void CheckForCollisions()
        {
            int count = Physics.OverlapSphereNonAlloc(
                transform.position,
                _findEnemyRange,
                _foundEnemyColliders
            );

            for (int i = 0; i < count; i++)
            {
                Collider collider = _foundEnemyColliders[i];

                if (collider == null)
                    continue;

                Vector3 hitPoint = collider.ClosestPoint(transform.position);

                if (collider.TryGetComponent(out DamageableArea damageableArea))
                {
                    OnEnemyHit(damageableArea, hitPoint);
                    return;
                }

                if (collider.TryGetComponent(out DestructibleObjectView destructible))
                {
                    destructible.ApplyDamage(hitPoint);
                    OnDestroyDrone(hitPoint);
                    return;
                }

                if (collider.TryGetComponent(out IndestructibleObject indestructible))
                {
                    OnDestroyDrone(hitPoint);
                    return;
                }
            }
        }

        private void OnEnemyHit(DamageableArea damageableArea, Vector3 hitPoint)
        {
            damageableArea.ApplyDamage(_damage, hitPoint);
            OnDestroyDrone(hitPoint);
        }

        private void OnDestroyDrone(Vector3 hitPoint)
        {
            CreateHitEffect(hitPoint);
            CreateSoundEffect(hitPoint);
            UpdateDroneEntities();
            Destroy(gameObject);
        }

        private void UpdateDroneEntities()
        {
            _droneHealth.TakeDamage();
            _droneCamera.enabled = false;
        }

        private void CreateHitEffect(Vector3 hitPoint)
        {
            if (gameObject == null)
                return;

            if (_exploudParticle == null)
                return;

            var effect = Instantiate(_exploudParticle, hitPoint, Quaternion.identity);
            Destroy(effect.gameObject, _exploudLifeTime);
        }

        private void CreateSoundEffect(Vector3 hitPoint)
        {
            if (_hitSound == null)
                return;

            AudioSource.PlayClipAtPoint(_hitSound, hitPoint);
        }

        private void AnimateDroneSpawn()
        {
            _hoverTween?.Kill();

            Vector3 startPos = _droneSpawnPoint.position;
            startPos.y = _spawnHeight;
            transform.position = startPos;

            float targetY = _droneSpawnPoint.position.y + _droneYpostionValue;

            transform.DOMoveY(targetY, _flyUpDuration)
                .SetEase(Ease.OutSine)
                .OnComplete(() =>
                {
                    _baseY = targetY;
                    StartHoverEffect();
                });
        }

        private void StartHoverEffect()
        {
            if (_isHovering)
                return;

            _isHovering = true;
            _hoverTween?.Kill();
            _baseY = transform.position.y;

            _hoverTween = transform
                .DOMoveY(_baseY + _hoverAmplitude, _divider / _hoverFrequency)
                .SetEase(Ease.InOutSine)
                .SetLoops(_loopTweenValue, LoopType.Yoyo);
        }

        private void RotateToCameraCenter()
        {
            if (_freeLookCamera == null)
                return;

            Ray ray = _freeLookCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            Vector3 targetDir = ray.direction;

            Quaternion targetRotation = Quaternion.LookRotation(targetDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);
        }
    }
}