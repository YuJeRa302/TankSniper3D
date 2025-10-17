using Assets.Source.Scripts.ScriptableObjects;
using UnityEngine;

namespace Assets.Source.Scripts.Projectile
{
    public abstract class BaseProjectile : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;

        private Transform _target;
        private int _speed;
        private float _lifeTime;

        private void Start()
        {
            if (_rigidbody != null)
                _rigidbody.velocity = transform.forward * _speed;

            Destroy(gameObject, _lifeTime);
        }

        private void OnTriggerEnter(Collider collider)
        {
            Hit2(collider);
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

        public virtual void Hit2(Collider collider) { }

        public virtual void Initialize(ProjectileData projectileData)
        {
            _speed = projectileData.Speed;
            _lifeTime = projectileData.LifeTime;
        }

        protected abstract void Hit(Collision collision);
    }
}