using Assets.Source.Scripts.ScriptableObjects;
using UnityEngine;

namespace Assets.Source.Scripts.Projectile
{
    public class Rocket : BaseProjectile
    {
        private ProjectileData _projectileData;
        private AudioSource _audioSource;
        private int _damage;

        public override AudioSource AudioSource => _audioSource;
        public override ProjectileData ProjectileData => _projectileData;
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