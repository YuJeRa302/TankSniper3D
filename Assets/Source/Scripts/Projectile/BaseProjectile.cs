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

        protected abstract void Hit(Collider collider);

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