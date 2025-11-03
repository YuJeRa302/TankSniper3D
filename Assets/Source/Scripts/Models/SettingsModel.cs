using Assets.Source.Scripts.Game;
using Assets.Source.Scripts.Services;
using Assets.Source.Scripts.Sound;
using System;
using UniRx;

namespace Assets.Source.Scripts.Models
{
    public class SettingsModel : IDisposable
    {
        public static readonly IMessageBroker Message = new MessageBroker();

        private readonly AudioPlayer _audioPlayer;
        private readonly PersistentDataService _persistentDataService;
        private readonly GamePauseService _gamePauseService;

        public SettingsModel(PersistentDataService persistentDataService, AudioPlayer audioPlayer)
        {
            _persistentDataService = persistentDataService;
            _audioPlayer = audioPlayer;
            IsMuted = _persistentDataService.PlayerProgress.IsMuted;
            _audioPlayer.MuteSound(IsMuted);
            AddListeners();
        }

        public SettingsModel(PersistentDataService persistentDataService, AudioPlayer audioPlayer, GamePauseService gamePauseService)
        {
            _persistentDataService = persistentDataService;
            _audioPlayer = audioPlayer;
            _gamePauseService = gamePauseService;
            IsMuted = _persistentDataService.PlayerProgress.IsMuted;
            _audioPlayer.MuteSound(IsMuted);
            Message.Publish(new M_SoundStateChanged(IsMuted));
            AddListeners();
        }

        public bool IsMuted { get; private set; }

        public void Dispose()
        {
            RemoveListeners();
        }

        public bool GetVibroState()
        {
            return _persistentDataService.PlayerProgress.IsVibro;
        }

        public void SetVibroState(bool state)
        {
            _persistentDataService.PlayerProgress.IsVibro = state;
        }

        public void SetMute(bool muted)
        {
            if (muted)
                Mute();
            else
                UnMute();
        }

        private void Mute()
        {
            IsMuted = true;
            _persistentDataService.PlayerProgress.IsMuted = IsMuted;

            if (_audioPlayer != null)
                _audioPlayer.MuteSound(IsMuted);
        }

        private void UnMute()
        {
            IsMuted = false;
            _persistentDataService.PlayerProgress.IsMuted = IsMuted;
            _audioPlayer.MuteSound(IsMuted);
        }

        private void OnGamePause(bool state)
        {
            if (_audioPlayer != null)
                _audioPlayer.MuteSound(true);

            Message.Publish(new M_SoundStateChanged(true));
        }

        private void OnGameResume(bool state)
        {
            if (_audioPlayer != null)
                _audioPlayer.MuteSound(_persistentDataService.PlayerProgress.IsMuted);

            Message.Publish(new M_SoundStateChanged(IsMuted));
        }

        private void AddListeners()
        {
            if (_gamePauseService == null)
                return;

            _gamePauseService.GamePaused += OnGamePause;
            _gamePauseService.GameResumed += OnGameResume;
        }

        private void RemoveListeners()
        {
            if (_gamePauseService == null)
                return;

            _gamePauseService.GamePaused -= OnGamePause;
            _gamePauseService.GameResumed -= OnGameResume;
        }
    }
}