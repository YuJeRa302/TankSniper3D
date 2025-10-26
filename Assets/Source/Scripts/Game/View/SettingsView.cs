using Assets.Source.Scripts.Models;
using Assets.Source.Scripts.Sound;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Scripts.Game
{
    public class SettingsView : GameTabView
    {
        [SerializeField] private Button _vibroButton;
        [SerializeField] private Button _soundButton;
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _openButton;
        [Space(20)]
        [SerializeField] private Image _soundImage;
        [SerializeField] private Sprite _soundMuteSprite;
        [SerializeField] private Sprite _soundUnmuteSprite;
        [Space(20)]
        [SerializeField] private Image _vibroImage;
        [SerializeField] private Sprite _vibroEnableSprite;
        [SerializeField] private Sprite _vibroDisableSprite;

        private SettingsModel _settingsModel;
        private AudioPlayer _audioPlayer;

        private void OnDestroy()
        {
            RemoveListeners();
        }

        public void Initialize(SettingsModel settingsModel, AudioPlayer audioPlayer)
        {
            _settingsModel = settingsModel;
            _audioPlayer = audioPlayer;
            ChangeImageSprite(_soundImage, _soundUnmuteSprite, _soundMuteSprite, _settingsModel.IsMuted);
            ChangeImageSprite(_vibroImage, _vibroEnableSprite, _vibroDisableSprite, _settingsModel.GetVibroState());
            AddListeners();
            gameObject.SetActive(false);
        }

        private void AddListeners()
        {
            _soundButton.onClick.AddListener(OnSoundButtonClicked);
            _vibroButton.onClick.AddListener(OnVibroButtonClicked);
            _backButton.onClick.AddListener(Close);
            _openButton.onClick.AddListener(Open);
        }

        private void RemoveListeners()
        {
            _soundButton.onClick.RemoveListener(OnSoundButtonClicked);
            _vibroButton.onClick.RemoveListener(OnVibroButtonClicked);
            _backButton.onClick.RemoveListener(Close);
            _openButton.onClick.RemoveListener(Open);
        }

        private void ChangeImageSprite(Image sourceImage, Sprite endableSprite, Sprite disableSprite, bool state)
        {
            if (state)
                sourceImage.sprite = endableSprite;
            else
                sourceImage.sprite = disableSprite;
        }

        private void OnVibroButtonClicked()
        {
            var vibroState = !_settingsModel.GetVibroState();
            _settingsModel.SetVibroState(vibroState);
            ChangeImageSprite(_vibroImage, _vibroEnableSprite, _vibroDisableSprite, vibroState);
        }

        private void OnSoundButtonClicked()
        {
            var mute = !_settingsModel.IsMuted;
            _settingsModel.SetMute(mute);
            _audioPlayer.MuteSound(mute);
            ChangeImageSprite(_soundImage, _soundUnmuteSprite, _soundMuteSprite, mute);
        }
    }
}