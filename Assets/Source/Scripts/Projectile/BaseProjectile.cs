using Assets.Source.Game.Scripts.Enemy;
using Assets.Source.Scripts.Game;
using Assets.Source.Scripts.ScriptableObjects;
using Assets.Source.Scripts.Upgrades;
using UnityEngine;

namespace Assets.Source.Scripts.Projectile
{
    public abstract class BaseProjectile : MonoBehaviour
    {
        private readonly int _criticalShootSpeed = 30;
        private readonly float _lifeTimeHitEffect = 1f;

        [SerializeField] private Rigidbody _rigidbody;

        private int _speed;
        private float _lifeTime;

        public abstract ProjectileData ProjectileData { get; }
        public abstract AudioSource AudioSource { get; }
        public abstract int Damage { get; }

        private void OnTriggerEnter(Collider collider)
        {
            Hit(collider);
        }

        public virtual void Initialize(ProjectileData projectileData, AudioSource audioSource)
        {
            _speed = projectileData.Speed;
            _lifeTime = projectileData.LifeTime;
            SetVelocityValue();
            Destroy(gameObject, _lifeTime);
        }

        public void SetSlowSpeed()
        {
            _speed = _criticalShootSpeed;
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            Vector3 hitPoint = transform.position;

            if (collision.collider.TryGetComponent(out IndestructibleObject indestructibleObject))
            {
                ContactPoint contact = collision.GetContact(0);
                hitPoint = contact.point;
            }

            if (collision.collider.TryGetComponent(out DamageableArea damageableArea))
            {
                ContactPoint contact = collision.GetContact(0);
                hitPoint = contact.point;
                damageableArea.ApplyDamage(Damage, hitPoint);
            }

            if (collision.collider.TryGetComponent(out DestructibleObjectView destructibleObjectView))
            {
                ContactPoint contact = collision.GetContact(0);
                hitPoint = contact.point;
                destructibleObjectView.ApplyDamage(hitPoint);
            }

            if (collision.collider.TryGetComponent(out DestructibleBuildingView destructibleBuildingView))
            {
                ContactPoint contact = collision.GetContact(0);
                hitPoint = contact.point;
                destructibleBuildingView.ApplyDamage(hitPoint);
            }

            CreateHitEffect(ProjectileData, hitPoint);
            CreateSoundEffect(ProjectileData, hitPoint);
            Destroy(gameObject);
        }

        protected virtual void Hit(Collider collider)
        {
            Vector3 hitPoint = transform.position;

            if (collider.TryGetComponent(out TankHealth tankHealth))
            {
                hitPoint = collider.ClosestPoint(transform.position);
                tankHealth.TakeDamage(Damage, hitPoint);
            }

            CreateHitEffect(ProjectileData, hitPoint);
            CreateSoundEffect(ProjectileData, hitPoint);
            Destroy(gameObject);
        }

        protected virtual void CreateSoundEffect(ProjectileData projectileData, Vector3 hitPoint)
        {
            if (projectileData == null)
                return;

            if (projectileData.HitSound != null)
                AudioSource.PlayOneShot(projectileData.HitSound);
        }

        protected void CreateHitEffect(ProjectileData projectileData, Vector3 hitPoint)
        {
            if (projectileData == null)
                return;

            if (projectileData.HitEffect != null)
            {
                var effect = Instantiate(projectileData.HitEffect, hitPoint, Quaternion.identity);
                Destroy(effect.gameObject, _lifeTimeHitEffect);
            }
        }

        private void SetVelocityValue()
        {
            if (_rigidbody != null)
                _rigidbody.velocity = transform.forward * _speed;
        }
    }
}