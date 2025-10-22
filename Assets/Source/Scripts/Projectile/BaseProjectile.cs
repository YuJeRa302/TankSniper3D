using Assets.Source.Game.Scripts.Enemy;
using Assets.Source.Scripts.Game;
using Assets.Source.Scripts.ScriptableObjects;
using UnityEngine;

namespace Assets.Source.Scripts.Projectile
{
    public abstract class BaseProjectile : MonoBehaviour
    {
        private readonly float _lifeTimeHitEffect = 1f;

        [SerializeField] private Rigidbody _rigidbody;

        private Transform _target;
        private int _speed;
        private float _lifeTime;

        public abstract ProjectileData ProjectileData { get; }
        public abstract int Damage { get; }

        private void Start()
        {
            if (_rigidbody != null)
                _rigidbody.velocity = transform.forward * _speed;

            Destroy(gameObject, _lifeTime);
        }

        private void OnTriggerEnter(Collider collider)
        {
            Hit(collider);
        }

        public virtual void Initialize(ProjectileData projectileData)
        {
            _speed = projectileData.Speed;
            _lifeTime = projectileData.LifeTime;
        }

        public void SetToTarget(Transform target)
        {
            _target = target;
            Vector3 direction = (_target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.LookAt(target.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, _speed * Time.deltaTime);
            transform.position += transform.forward * _speed * Time.deltaTime;
        }

        protected void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.TryGetComponent(out IndestructibleObject indestructibleObject))
            {
                ContactPoint contact = collision.GetContact(0);
                Vector3 hitPoint = contact.point;
                CreateHitEffect(ProjectileData, hitPoint);
                CreateSoundEffect(ProjectileData, hitPoint);
                Destroy(gameObject);
            }
        }

        protected virtual void Hit(Collider collider)
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
                damageableArea.ApplyDamage(Damage, hitPoint);
            }

            CreateHitEffect(ProjectileData, hitPoint);
            CreateSoundEffect(ProjectileData, hitPoint);
            Destroy(gameObject);
        }

        protected void CreateHitEffect(ProjectileData projectileData, Vector3 hitPoint)
        {
            if (projectileData.HitEffect == null)
                return;

            var effect = Instantiate(projectileData.HitEffect, hitPoint, Quaternion.identity);
            Destroy(effect.gameObject, _lifeTimeHitEffect);
        }

        protected void CreateSoundEffect(ProjectileData projectileData, Vector3 hitPoint)
        {
            if (projectileData.FireSound == null)
                return;

            AudioSource.PlayClipAtPoint(projectileData.HitSound, hitPoint);
        }
    }
}