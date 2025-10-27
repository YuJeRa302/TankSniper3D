using Assets.Source.Scripts.ScriptableObjects;
using UnityEngine;

namespace Assets.Source.Scripts.Projectile
{
    public class SimpleProjectile : BaseProjectile
    {
        private ProjectileData _projectileData;
        private AudioSource _audioSource;
        private int _damage;

        public override ProjectileData ProjectileData => _projectileData;
        public override AudioSource AudioSource => _audioSource;
        public override int Damage => _damage;

        public override void Initialize(ProjectileData projectileData, AudioSource audioSource)
        {
            base.Initialize(projectileData, audioSource);
            _projectileData = projectileData;
            _damage = projectileData.Damage;
            _audioSource = audioSource;
        }
    }
}