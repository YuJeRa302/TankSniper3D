using UnityEngine;

namespace Assets.Source.Scripts.Sound
{
    public class AudioPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource _ambientAudioSource;
        [SerializeField] private AudioSource _sfxAudioSource;
        [Space(20)]
        [SerializeField] private AudioClip _createTankAudioClip;
        [SerializeField] private AudioClip _mergeTankAudioClip;
        [SerializeField] private AudioClip _tankSpawnAudioClip;
        [SerializeField] private AudioClip _ambientAudioClip;

        public AudioSource SfxAudioSource => _sfxAudioSource;

        public void PlaySpawnTankAudio()
        {
            _sfxAudioSource.PlayOneShot(_tankSpawnAudioClip);
        }

        public void PlayCharacterAudio(AudioClip audioClip)
        {
            _sfxAudioSource.PlayOneShot(audioClip);
        }

        public void PlayCreateTankAudio()
        {
            _sfxAudioSource.PlayOneShot(_createTankAudioClip);
        }

        public void PlayMergeTankAudioClip()
        {
            _sfxAudioSource.PlayOneShot(_mergeTankAudioClip);
        }

        public void PlayAmbient()
        {
            _ambientAudioSource.Stop();
        }

        public void StopAmbient()
        {
            _ambientAudioSource.Stop();
        }

        public void MuteSound(bool isMute)
        {
            _ambientAudioSource.mute = isMute;
            _sfxAudioSource.mute = isMute;
        }
    }
}