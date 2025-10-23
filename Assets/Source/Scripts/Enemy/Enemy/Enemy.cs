using Assets.Source.Scripts.Game;
using Assets.Source.Scripts.ScriptableObjects;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Enemy
{
    [RequireComponent(typeof(EnemyStateStrategy))]
    public abstract class Enemy : MonoBehaviour
    {
        [SerializeField] private float _pieceForce = 15f;
        [SerializeField] private float _pieceUpwards = 0.1f;
        [SerializeField] private float _pieceTorque = 2f;
        [SerializeField] private List<GameObject> _destroyPieces;
        [Space(20)]
        [SerializeField] private List<Transform> _waypoints;
        [Space(20)]
        [SerializeField] private Transform _rotationPartToPlayer;
        [Space(20)]
        [SerializeField] private EnemyData _enemyData;
        [SerializeField] private bool _isPlayerShot;
        [Space(20)]
        [SerializeField] private List<DamageableArea> _damageableAreas;
        [Space(20)]
        [SerializeField] private Animator _animator;
        [Space(20)]
        [SerializeField] private AudioSource _audioSource;
        [Space(20)]
        [SerializeField] private EnemyStateStrategy _enemyStateStrategy;

        private Transform _player;
        private EnemyHealth _enemyHealth;
        private EnemyAnimation _enemyAnimation;
        private EnemySoundPlayer _enemySoundPlayer;
        private CompositeDisposable _disposables = new();

        public EnemySoundPlayer EnemySoundPlayer => _enemySoundPlayer;
        public Transform RotationPartToPlayer => _rotationPartToPlayer;
        public Transform Player => _player;
        public EnemyStateStrategy EnemyStateStrategy => _enemyStateStrategy;
        public List<Transform> Waypoints => _waypoints;
        public EnemyAnimation EnemyAnimation => _enemyAnimation;
        public bool IsPlayerShot => _isPlayerShot;
        public bool IsDead => _enemyHealth.IsDead;
        public float MoveSpeed => _enemyData.MoveSpeed;
        public int Health => _enemyData.Health;
        public float RotateSpeed => _enemyData.RotationSpeed;
        public int MoneyReward => _enemyData.MoneyReward;

        private void OnDestroy()
        {
            _disposables?.Dispose();
        }

        public virtual void Initialize(Transform tankTransform)
        {
            _player = tankTransform;
            _enemyAnimation = new EnemyAnimation(_animator);
            _enemyHealth = new EnemyHealth(this);
            _enemySoundPlayer = new EnemySoundPlayer(_enemyData, _audioSource);
            _damageableAreas.ForEach(s => s.Initialize(_enemyHealth));

            Shooting.Message
                .Receive<M_Shoot>()
                .Subscribe(m => OnPlayerFirstShot())
                .AddTo(_disposables);

            DroneScopeView.Message
                .Receive<M_Shoot>()
                .Subscribe(m => OnPlayerFirstShot())
                .AddTo(_disposables);
        }

        public void CreateExplosionEffect()
        {
            if (_enemyData.ExplosionEffect == null)
                return;

            Instantiate(_enemyData.ExplosionEffect, transform.position, transform.rotation);
        }

        public void DestroyParts()
        {
            var maincollider = gameObject.GetComponent<BoxCollider>();

            if (maincollider != null)
                maincollider.enabled = false;

            Vector3 explosionCenter = transform.position;

            foreach (var part in _destroyPieces)
            {
                if (part == null)
                    continue;

                var rigidbody = part.GetComponent<Rigidbody>();
                var collider = part.GetComponent<BoxCollider>();

                if (rigidbody != null)
                {
                    rigidbody.isKinematic = false;
                    collider.enabled = true;
                    Vector3 dir = (part.transform.position - explosionCenter).normalized + Vector3.up * _pieceUpwards;
                    rigidbody.AddForce(dir.normalized * _pieceForce, ForceMode.Impulse);
                    rigidbody.AddTorque(Random.insideUnitSphere * _pieceTorque, ForceMode.Impulse);
                }
            }
        }

        private void OnPlayerFirstShot()
        {
            if (_isPlayerShot)
                return;

            _isPlayerShot = true;
        }
    }
}