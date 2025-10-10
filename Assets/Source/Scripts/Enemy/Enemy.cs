using Assets.Source.Scripts.Services;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Enemy
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] public Transform player; // поменять при загрузке передавать его трансформ танка
        [SerializeField] private List<Transform> _waypoints;
        [Space(20)]
        [SerializeField] private int _health;
        [SerializeField] private float _moveSpeed = 3f;
        [SerializeField] private float _rotateSpeed = 5f;
        [SerializeField] private float _attackRange = 15f;
        [SerializeField] private int _shotsBeforeReload = 3;
        [Space(20)]
        [SerializeReference] private List<DamageableArea> _damageableAreas;
        [Space(20)]
        [SerializeReference] private List<BaseEnemyState> _enemyStates;
        [Space(20)]
        [SerializeReference] private IUseEnemyStateStrategy _useEnemyStateStrategy;
        [Space(20)]
        [SerializeField] private Animator _animator;

        private EnemyHealth _enemyHealth;
        private EnemyAnimation _enemyAnimation;
        private bool _isPlayerShot = false;

        public IUseEnemyStateStrategy UseEnemyStateStrategy => _useEnemyStateStrategy;
        public List<BaseEnemyState> EnemyStates => _enemyStates;
        public List<Transform> Waypoints => _waypoints;
        public EnemyAnimation EnemyAnimation => _enemyAnimation;
        public int ShotsBeforeReload => _shotsBeforeReload;
        public bool IsPlayerShot => _isPlayerShot;
        public bool IsDead => _enemyHealth.IsDead;
        public float MoveSpeed => _moveSpeed;
        public int MaxHealth => _health;
        public float RotateSpeed => _rotateSpeed;
        public float AttackRange => _attackRange;

        private void Awake()
        {
            _enemyAnimation = new EnemyAnimation(_animator);
            _enemyHealth = new EnemyHealth(this);
            _damageableAreas.ForEach(s => s.Initialize(_enemyHealth));
            _useEnemyStateStrategy.Construct(this);

            //PlayerShooting.OnPlayerShot += OnPlayerFirstShot;
        }

        private void Update()
        {
            _useEnemyStateStrategy.CurrentStateExecute();
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