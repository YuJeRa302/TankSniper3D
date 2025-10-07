using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Scripts.Projectile
{
    public class SimpleProjectile : BaseProjectile
    {
        private int _damage;

        public override void Initialize()
        {

        }

        protected override void Hit(Collision collision)
        {
            //var destructible = collision.collider.GetComponentInParent<BuildingDestructible>();
            //if (destructible != null)
            //{
            //    Vector3 hitPoint = collision.contacts[0].point;
            //    destructible.ApplyDamage(_damage, hitPoint);
            //}

            Destroy(gameObject);
        }
    }
}