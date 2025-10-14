using Assets.Source.Scripts.Projectile;
using Assets.Source.Scripts.Services;
using UnityEngine;

namespace Assets.Source.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New ProjectileData", menuName = "Create ProjectileData", order = 51)]
    public class ProjectileData : ScriptableObject
    {
        [SerializeField] private int _speed;
        [SerializeField] private int _damage;
        [SerializeField] private int _projectileCount;
        [SerializeField] private float _reloadTime;
        [SerializeField] private float _lifeTime;
        [SerializeField] private float _spread;
        [SerializeField] private ParticleSystem _muzzleFlash;
        [SerializeField] private ParticleSystem _hitEffect;
        [SerializeField] private AudioClip _fireSound;
        [SerializeField] private AudioClip _hitSound;
        [SerializeField] private BaseProjectile _baseProjectile;
        [SerializeReference] private IShootingStrategy _shootingStrategy; // убрать в танк сам, и забирать уже из его даты а не из даты снаряда

        public int Speed => _speed;
        public int Damage => _damage;
        public int ProjectileCount => _projectileCount;
        public float ReloadTime => _reloadTime;
        public float LifeTime => _lifeTime;
        public float Spread => _spread;
        public BaseProjectile BaseProjectile => _baseProjectile;
        public ParticleSystem MuzzleFlash => _muzzleFlash;
        public ParticleSystem HitEffect => _hitEffect;
        public AudioClip FireSound => _fireSound;
        public AudioClip HitSound => _hitSound;
        public IShootingStrategy ShootingStrategy => _shootingStrategy;
    }
}