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
        [SerializeField] private BaseProjectile _baseProjectile;
        [SerializeReference] private IShootingStrategy _shootingStrategy;

        public int Speed => _speed;
        public int Damage => _damage;
        public int ProjectileCount => _projectileCount;
        public float ReloadTime => _reloadTime;
        public float LifeTime => _lifeTime;
        public BaseProjectile BaseProjectile => _baseProjectile;
        public IShootingStrategy ShootingStrategy => _shootingStrategy;
    }
}