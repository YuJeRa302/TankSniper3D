using Assets.Source.Scripts.ScriptableObjects;
using UnityEngine;

namespace Assets.Source.Scripts.Projectile
{
    public class Bullet : BaseProjectile
    {
        private ProjectileData _projectileData;
        private int _damage;

        public override ProjectileData ProjectileData => _projectileData;
        public override int Damage => _damage;

        public override void Initialize(ProjectileData projectileData)
        {
            base.Initialize(projectileData);
            _projectileData = projectileData;
            _damage = _projectileData.Damage;
        }

        protected override void Hit(Collider collider)
        {
            //if (collision.collider.TryGetComponent(out DestructibleObjectView destructibleObjectView))
            //{
            //    Vector3 hitPoint = collision.contacts[0].point;
            //    destructibleObjectView.ApplyDamage(hitPoint);
            //}

            //Destroy(gameObject);
        }
    }
}