using UnityEngine;

namespace Assets.Source.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New EnemyData", menuName = "Create EnemyData", order = 51)]
    public class EnemyData : ScriptableObject
    {
        [SerializeField] private int _health;
        [SerializeField] private float _moveSpeed = 3f;
        [SerializeField] private float _rotateSpeed = 5f;
        [SerializeField] private ParticleSystem _explosionEffect;
        [SerializeField] private AudioClip _movingAudioClip;
        [SerializeField] private AudioClip _explosionSound;

        public int Health => _health;
        public float MoveSpeed => _moveSpeed;
        public float RotationSpeed => _rotateSpeed;
        public ParticleSystem ExplosionEffect => _explosionEffect;
        public AudioClip MovingAudioClip => _movingAudioClip;
        public AudioClip ExplosionSound => _explosionSound;
    }
}