using Assets.Source.Scripts.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Scripts.Projectile
{
    public class SmallProjectile : BaseProjectile
    {
        public override void Initialize(ProjectileData projectileData)
        {
            base.Initialize(projectileData);
        }

        protected override void Hit(Collision collision)
        {
            //Vector3 hitPoint = collision.contacts[0].point;

            //var destructible = collision.collider.GetComponentInParent<BuildingDestructible>();
            //if (destructible != null)
            //{
            //    destructible.ApplyDamage(damage, hitPoint);
            //}

            //var enemy = collision.collider.GetComponentInParent<EnemyHealth>();
            //if (enemy != null)
            //{
            //    enemy.ApplyDamage(damage);
            //}

            //Destroy(gameObject);
        }
    }
}