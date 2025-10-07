using UnityEngine;

namespace Assets.Source.Scripts.Projectile
{
    public class ExplodingProjectile : BaseProjectile
    {
        public override void Initialize()
        {

        }

        protected override void Hit(Collision collision)
        {
            Vector3 hitPoint = collision.contacts[0].point;

            Explode(hitPoint);

            SpawnSmallProjectiles(hitPoint);

            Destroy(gameObject);
        }

        private void Explode(Vector3 center)
        {
            //// Взрыв наносит урон всем в радиусе
            //Collider[] hits = Physics.OverlapSphere(center, explosionRadius);
            //foreach (var hit in hits)
            //{
            //    var destructible = hit.GetComponentInParent<BuildingDestructible>();
            //    if (destructible != null)
            //    {
            //        destructible.ApplyDamage(damage, center);
            //    }

            //    var enemy = hit.GetComponentInParent<EnemyHealth>();
            //    if (enemy != null)
            //    {
            //        enemy.ApplyDamage(damage);
            //    }
            //}

            //// Тут можно добавить эффект взрыва, звук и т.д.
        }

        private void SpawnSmallProjectiles(Vector3 spawnPoint)
        {
            //for (int i = 0; i < smallProjectileCount; i++)
            //{
            //    float angle = i * 360f / smallProjectileCount;
            //    Vector3 dir = Quaternion.Euler(0, angle, 0) * Vector3.forward;

            //    GameObject proj = Instantiate(smallProjectilePrefab, spawnPoint, Quaternion.LookRotation(dir));
            //    var projRb = proj.GetComponent<Rigidbody>();
            //    if (projRb != null)
            //        projRb.velocity = dir * smallProjectileSpeed;

            //    // Передаем урон маленьким снарядам, если у них есть скрипт с логикой урона
            //    var smallProjScript = proj.GetComponent<SmallProjectile>();
            //    if (smallProjScript != null)
            //    {
            //        smallProjScript.damage = smallProjectileDamage;
            //    }
            //}
        }
    }
}