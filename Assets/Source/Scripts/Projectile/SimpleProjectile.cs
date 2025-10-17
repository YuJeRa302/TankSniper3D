using Assets.Source.Game.Scripts.Enemy;
using Assets.Source.Scripts.Game;
using Assets.Source.Scripts.ScriptableObjects;
using UnityEngine;

namespace Assets.Source.Scripts.Projectile
{
    public class SimpleProjectile : BaseProjectile
    {
        private int _damage;

        public override void Initialize(ProjectileData projectileData)
        {
            base.Initialize(projectileData);
            _damage = projectileData.Damage;
        }

        public override void Hit2(Collider collider)
        {
            if (collider.TryGetComponent(out DestructibleObjectView destructibleObjectView))
            {
                Vector3 hitPoint = collider.ClosestPoint(transform.position);
                destructibleObjectView.ApplyDamage(hitPoint);
            }

            if (collider.TryGetComponent(out DamageableArea damageableArea))
            {
                Vector3 hitPoint = collider.ClosestPoint(transform.position);
                damageableArea.ApplyDamage(_damage, hitPoint);
            }

            Destroy(gameObject);
        }

        protected override void Hit(Collision collision)
        {
            Debug.Log("ApplyDamage");

            if (collision.collider.TryGetComponent(out DestructibleObjectView destructibleObjectView))
            {
                Vector3 hitPoint = collision.contacts[0].point;
                destructibleObjectView.ApplyDamage(hitPoint);
            }

            if (collision.collider.TryGetComponent(out DamageableArea damageableArea))
            {
                Vector3 hitPoint = collision.contacts[0].point;
                damageableArea.ApplyDamage(_damage, hitPoint);
            }

            Destroy(gameObject);
        }
    }
}