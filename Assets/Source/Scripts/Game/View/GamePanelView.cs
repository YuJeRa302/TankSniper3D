using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Scripts.Game
{
    public class GamePanelView : MonoBehaviour
    {
        public static readonly IMessageBroker Message = new MessageBroker();

        [SerializeField] private Button _settingsButton;

        private void Awake()
        {
            _settingsButton.onClick.AddListener(OnSettingsButton);
        }

        private void OnDestroy()
        {
            _settingsButton.onClick.RemoveListener(OnSettingsButton);
        }

        private void OnSettingsButton()
        {

        }
    }
}