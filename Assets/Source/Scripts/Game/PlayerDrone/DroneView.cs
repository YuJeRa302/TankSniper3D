using Assets.Source.Game.Scripts.Enemy;
using UnityEngine;

namespace Assets.Source.Scripts.Game
{
    public class DroneView : MonoBehaviour
    {
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

        private Collider[] _foundEnemyColliders = new Collider[50];

        public Camera DroneCamera => _droneCamera;
        public float DroneSpeed => _droneSpeed;
        public float RotationSpeed => _rotationSpeed;

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, _findEnemyRange);
        }

        private void Update()
        {
            CheckForCollisions();
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
    }
}