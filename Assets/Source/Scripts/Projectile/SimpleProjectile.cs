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
        }

        protected override void Hit(Collision collision)
        {
            if (collision.collider.TryGetComponent(out DestructibleObjectView destructibleObjectView))
            {
                Vector3 hitPoint = collision.contacts[0].point;
                destructibleObjectView.ApplyDamage(hitPoint);
            }

            Destroy(gameObject);
        }
    }
}