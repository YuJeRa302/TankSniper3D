using UnityEngine;

namespace Assets.Source.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New EnemyData", menuName = "Create EnemyData", order = 51)]
    public class EnemyData : ScriptableObject
    {
        [SerializeField] private int _health;
        [SerializeField] private float _moveSpeed = 3f;
        [SerializeField] private float _rotateSpeed = 5f;
        [SerializeField] private int _moneyReward;
        [SerializeField] private ParticleSystem _explosionEffect;
        [SerializeField] private AudioClip _movingAudioClip;
        [SerializeField] private AudioClip _standingAudioClip;
        [SerializeField] private AudioClip _reloadingAudioClip;
        [SerializeField] private AudioClip _explosionSound;

        public int Health => _health;
        public float MoveSpeed => _moveSpeed;
        public float RotationSpeed => _rotateSpeed;
        public int MoneyReward => _moneyReward;
        public ParticleSystem ExplosionEffect => _explosionEffect;
        public AudioClip MovingAudioClip => _movingAudioClip;
        public AudioClip ExplosionSound => _explosionSound;
        public AudioClip StandingAudioClip => _standingAudioClip;
        public AudioClip ReloadingAudioClip => _reloadingAudioClip;
    }
}