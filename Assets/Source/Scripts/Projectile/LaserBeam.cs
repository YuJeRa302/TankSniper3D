using Assets.Source.Scripts.ScriptableObjects;
using UnityEngine;

namespace Assets.Source.Scripts.Projectile
{
    public class LaserBeam : BaseProjectile
    {
        public float maxDistance = 100f;
        public float damage = 50f;  // ����������� ���� �� �������
        public LineRenderer lineRenderer;
        public Transform firePoint;
        public float laserDisplayTime = 0.2f; // ����� ������ ������ ����� ��������

        private float laserTimer = 0f;

        public override void Initialize(ProjectileData projectileData)
        {
            base.Initialize(projectileData);
        }

        void Update() // �������� �� ��������
        {
            // ��������� ����� ������ laserDisplayTime
            if (laserTimer > 0)
            {
                laserTimer -= Time.deltaTime;
                if (laserTimer <= 0)
                {
                    lineRenderer.enabled = false;
                }
            }
        }


        protected override void Hit(Collision collision)
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, firePoint.position);

            RaycastHit hit;
            if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, maxDistance))
            {
                lineRenderer.SetPosition(1, hit.point);

                // ���������, ���� �� � ������� ���������, ���������� �� ����
                //var building = hit.collider.GetComponentInParent<BuildingDestructible>();
                //if (building != null)
                //{
                //    building.ApplyDamage(damage, hit.point);
                //    return;
                //}

                //var enemy = hit.collider.GetComponentInParent<EnemyHealth>();
                //if (enemy != null)
                //{
                //    enemy.ApplyDamage(damage);
                //    return;
                //}

                // ����� �������� ������ ���� ��������, ���� �����
            }
            else
            {
                lineRenderer.SetPosition(1, firePoint.position + firePoint.forward * maxDistance);
            }

            laserTimer = laserDisplayTime;
        }
    }
}