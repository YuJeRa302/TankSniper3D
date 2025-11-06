using Assets.Source.Scripts.Game;
using Assets.Source.Scripts.Models;
using Assets.Source.Scripts.ScriptableObjects;
using Assets.Source.Scripts.Upgrades;
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
        private GameModel _gameModel;
        private EnemyHealth _enemyHealth;
        private EnemyAnimation _enemyAnimation;
        private EnemySoundPlayer _enemySoundPlayer;
        private CompositeDisposable _disposables = new();

        public AudioSource AudioSource => _audioSource;
        public EnemyHealth EnemyHealth => _enemyHealth;
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

        private void OnDestroy()
        {
            _disposables?.Dispose();
        }

        public virtual void Initialize(Transform tankTransform, GameModel gameModel)
        {
            _player = tankTransform;
            _enemyAnimation = new EnemyAnimation(_animator);
            _enemyHealth = new EnemyHealth(this);
            _enemySoundPlayer = new EnemySoundPlayer(_enemyData, _audioSource);
            _damageableAreas.ForEach(s => s.Initialize(_enemyHealth));
            _gameModel = gameModel;

            Shooting.Message
                .Receive<M_SuperShoot>()
                .Subscribe(m => OnPlayerFirstShot())
                .AddTo(_disposables);

            TankHealth.Message
                .Receive<M_DeathTank>()
                .Subscribe(m => OnPlayerDeath())
                .AddTo(_disposables);

            GamePanelView.Message
                .Receive<M_DefeatByDrone>()
                .Subscribe(m => OnPlayerDeath())
                .AddTo(_disposables);

            DefeatTab.Message
                .Receive<M_RecoverPlayer>()
                .Subscribe(m => OnRecoveryTankHealth())
                .AddTo(_disposables);

            DroneScopeView.Message
                .Receive<M_Shoot>()
                .Subscribe(m => OnPlayerFirstShot())
                .AddTo(_disposables);

            SettingsModel.Message
                .Receive<M_SoundStateChanged>()
                .Subscribe(m => ChangeStateSound(m.IsMuted))
                .AddTo(_disposables);

            _enemyHealth.OnDeath
                .Subscribe(_ => OnEnemyDeath())
                .AddTo(this);
        }

        public void CreateExplosionEffect()
        {
            if (_enemyData.ExplosionEffect == null)
                return;

            Instantiate(_enemyData.ExplosionEffect, transform.position, transform.rotation);
        }

        public void DestroyParts()
        {
            foreach (var damageableArea in _damageableAreas)
                damageableArea.DestroyCollider();

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

        protected virtual void OnEnemyDeath()
        {
            _gameModel.SetEarnedMoney(_enemyData.MoneyReward);
        }

        private void ChangeStateSound(bool state)
        {
            if (_audioSource != null)
                _audioSource.mute = state;
        }

        private void OnPlayerFirstShot()
        {
            if (_isPlayerShot)
                return;

            _isPlayerShot = true;
        }

        private void OnRecoveryTankHealth()
        {
            _isPlayerShot = true;
        }

        private void OnPlayerDeath()
        {
            _isPlayerShot = false;
        }
    }
}