using Assets.Source.Scripts.Projectile;
using UnityEngine;

namespace Assets.Source.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New ProjectileData", menuName = "Create ProjectileData", order = 51)]
    public class ProjectileData : ScriptableObject
    {
        [SerializeField] private int _speed;
        [SerializeField] private int _damage;
        [SerializeField] private int _projectileCount;
        [SerializeField] private int _energyProjectileCount;
        [SerializeField] private float _reloadTime;
        [SerializeField] private float _lifeTime;
        [SerializeField] private float _spread;
        [SerializeField] private ParticleSystem _muzzleFlash;
        [SerializeField] private ParticleSystem _hitEffect;
        [SerializeField] private AudioClip _fireSound;
        [SerializeField] private AudioClip _hitSound;
        [SerializeField] private BaseProjectile _baseProjectile;
        [SerializeField] private BaseProjectile _energyProjectile;

        public int Speed => _speed;
        public int Damage => _damage;
        public int ProjectileCount => _projectileCount;
        public int EnergyProjectileCount => _energyProjectileCount;
        public float ReloadTime => _reloadTime;
        public float LifeTime => _lifeTime;
        public float Spread => _spread;
        public BaseProjectile BaseProjectile => _baseProjectile;
        public BaseProjectile EnergyProjectile => _energyProjectile;
        public ParticleSystem MuzzleFlash => _muzzleFlash;
        public ParticleSystem HitEffect => _hitEffect;
        public AudioClip FireSound => _fireSound;
        public AudioClip HitSound => _hitSound;
    }
}