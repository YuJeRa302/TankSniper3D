using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Scripts.Game
{
    public class SettingsView : MonoBehaviour
    {
        [SerializeField] private Button _vibroButton;
        [SerializeField] private Button _soundButton;
        [SerializeField] private Button _backButton;

        private void Awake()
        {
            _soundButton.onClick.AddListener(OnSoundButtonClicked);
            _vibroButton.onClick.AddListener(OnVibroButtonClicked);
            _backButton.onClick.AddListener(OnBackButtonClicked);
        }

        private void OnDestroy()
        {
            _soundButton.onClick.RemoveListener(OnSoundButtonClicked);
            _vibroButton.onClick.RemoveListener(OnVibroButtonClicked);
            _backButton.onClick.RemoveListener(OnBackButtonClicked);
        }

        private void OnVibroButtonClicked()
        {

        }

        private void OnSoundButtonClicked()
        {

        }

        private void OnBackButtonClicked()
        {

        }
    }
}