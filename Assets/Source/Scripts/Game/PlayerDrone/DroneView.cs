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

        public Camera DroneCamera => _droneCamera;
        public float DroneSpeed => _droneSpeed;
        public float RotationSpeed => _rotationSpeed;

        private void OnDestroy()
        {
            CreateHitEffect(transform.position);
            CreateSoundEffect(transform.position);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.TryGetComponent(out IndestructibleObject indestructibleObject))
            {
                ContactPoint contact = collision.GetContact(0);
                Vector3 hitPoint = contact.point;
                CreateHitEffect(hitPoint);
                CreateSoundEffect(hitPoint);
                UpdateDroneEntities();
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter(Collider collider)
        {
            Vector3 hitPoint = transform.position;

            if (collider.TryGetComponent(out DestructibleObjectView destructibleObjectView))
            {
                hitPoint = collider.ClosestPoint(transform.position);
                destructibleObjectView.ApplyDamage(hitPoint);
            }

            if (collider.TryGetComponent(out DamageableArea damageableArea))
            {
                hitPoint = collider.ClosestPoint(transform.position);
                damageableArea.ApplyDamage(_damage, hitPoint);
            }

            CreateHitEffect(hitPoint);
            CreateSoundEffect(hitPoint);
            UpdateDroneEntities();
            Destroy(gameObject);
        }

        public void Initialize()
        {
            _droneHealth.Initialize(_health);
            Destroy(gameObject, _droneLifeTime);
        }

        private void UpdateDroneEntities()
        {
            _droneHealth.TakeDamage(_health);
            _droneCamera.enabled = false;
        }

        private void CreateHitEffect(Vector3 hitPoint)
        {
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