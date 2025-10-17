using Assets.Source.Scripts.ScriptableObjects;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Enemy
{
    public class EnemySoundPlayer
    {
        private readonly EnemyData _enemyData;
        private readonly AudioSource _audioSource;

        public EnemySoundPlayer(EnemyData enemyData, AudioSource audioSource)
        {
            _audioSource = audioSource;
            _enemyData = enemyData;
        }

        public void PlayerSoundReloading()
        {
            if (_enemyData.ReloadingAudioClip == null)
                return;

            _audioSource.PlayOneShot(_enemyData.ReloadingAudioClip);
        }

        public void PlayerSoundStanding()
        {
            if (_enemyData.StandingAudioClip == null)
                return;

            _audioSource.PlayOneShot(_enemyData.StandingAudioClip);
        }

        public void PlayExplosionSound()
        {
            if (_enemyData.ExplosionSound == null)
                return;

            _audioSource.PlayOneShot(_enemyData.ExplosionSound);
        }

        public void PlayMovingSound()
        {
            if (_enemyData.MovingAudioClip == null)
                return;

            _audioSource.clip = _enemyData.MovingAudioClip;
            _audioSource.Play();
        }

        public void StopMovingSound()
        {
            if (_audioSource == null)
                return;

            _audioSource.clip = null;
            _audioSource.Stop();
        }
    }
}