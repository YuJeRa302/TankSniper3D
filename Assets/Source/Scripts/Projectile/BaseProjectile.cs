using Assets.Source.Scripts.ScriptableObjects;
using UnityEngine;

namespace Assets.Source.Scripts.Projectile
{
    public abstract class BaseProjectile : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;

        private int _speed;
        private float _lifeTime;

        private void Start()
        {
            if (_rigidbody != null)
                _rigidbody.velocity = transform.forward * _speed;

            Destroy(gameObject, _lifeTime);
        }

        private void OnCollisionEnter(Collision collision)
        {
            Hit(collision);
        }

        public virtual void Initialize(ProjectileData projectileData)
        {
            _speed = projectileData.Speed;
            _lifeTime = projectileData.LifeTime;
        }

        protected abstract void Hit(Collision collision);
    }
}