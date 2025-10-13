using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Enemy
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(EnemyStateStrategy))]
    public class Enemy : MonoBehaviour
    {
        [SerializeField] public Transform player; // поменять при загрузке передавать его трансформ танка
        [SerializeField] private List<Transform> _waypoints;
        [Space(20)]
        [SerializeField] private int _health;
        [SerializeField] private float _moveSpeed = 3f;
        [SerializeField] private float _rotateSpeed = 5f;
        [SerializeField] private int _shotsBeforeReload = 3;
        [SerializeField] private float _reloadTime = 3;
        [SerializeField] private bool _isPlayerShot;
        [SerializeField] private bool _isDead;
        [Space(20)]
        [SerializeReference] private List<DamageableArea> _damageableAreas;
        [Space(20)]
        [SerializeField] private Animator _animator;
        [Space(20)]
        [SerializeField] private EnemyStateStrategy _enemyStateStrategy;

        private EnemyHealth _enemyHealth;
        private EnemyAnimation _enemyAnimation;

        public List<Transform> Waypoints => _waypoints;
        public EnemyAnimation EnemyAnimation => _enemyAnimation;
        public int ShotsBeforeReload => _shotsBeforeReload;
        public bool IsPlayerShot => _isPlayerShot;
        public float ReloadTime => _reloadTime;
        public bool IsDead => _isDead;
        //public bool IsDead => _enemyHealth.IsDead;
        public float MoveSpeed => _moveSpeed;
        public int MaxHealth => _health;
        public float RotateSpeed => _rotateSpeed;

        private void Awake()
        {
            _enemyAnimation = new EnemyAnimation(_animator);
            _enemyHealth = new EnemyHealth(this);
            _damageableAreas.ForEach(s => s.Initialize(_enemyHealth));
            _enemyStateStrategy.Initialize(this);
            //PlayerShooting.OnPlayerShot += OnPlayerFirstShot;
        }

        private void OnDestroy()
        {
            //PlayerShooting.OnPlayerShot -= OnPlayerFirstShot;
        }

        private void OnPlayerFirstShot()
        {
            _isPlayerShot = true;
        }
    }
}