using UnityEngine;

namespace Assets.Source.Scripts.Projectile
{
    public class LaserBeam : BaseProjectile
    {
        [SerializeField] private Material _laserMaterial;

        public Material Material => _laserMaterial;

        protected override void Hit(Collider collider)
        {
            throw new System.NotImplementedException();
        }
    }
}