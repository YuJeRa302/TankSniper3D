using Assets.Source.Game.Scripts.Enemy;
using Assets.Source.Scripts.Game;
using Assets.Source.Scripts.ScriptableObjects;
using UnityEngine;

namespace Assets.Source.Scripts.Projectile
{
    public class Rocket : BaseProjectile
    {
        private ProjectileData _projectileData;
        private int _damage;

        public override void Initialize(ProjectileData projectileData)
        {
            base.Initialize(projectileData);
            _projectileData = projectileData;
            _damage = projectileData.Damage;
        }

        protected override void Hit(Collider collider)
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

            CreateHitEffect(_projectileData, hitPoint);
            Destroy(gameObject);
        }
    }
}