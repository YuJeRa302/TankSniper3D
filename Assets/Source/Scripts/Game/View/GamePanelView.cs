using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Scripts.Game
{
    public class GamePanelView : MonoBehaviour
    {
        public static readonly IMessageBroker Message = new MessageBroker();

        [SerializeField] private Button _settingsButton;

        private CompositeDisposable _disposables = new();

        private void Awake()
        {
            AddListeners();
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }

        private void AddListeners()
        {
            _settingsButton.onClick.AddListener(OnSettingsButton);

            SniperScopeView.Message
                .Receive<M_Aiming>()
                .Subscribe(m => OnSniperScopeUsed(m.IsAiming))
                .AddTo(_disposables);
        }

        private void RemoveListeners()
        {
            _settingsButton.onClick.RemoveListener(OnSettingsButton);
            _disposables?.Dispose();
        }

        private void OnSettingsButton()
        {

        }

        private void OnSniperScopeUsed(bool state)
        {
            gameObject.SetActive(!state);
        }
    }
}