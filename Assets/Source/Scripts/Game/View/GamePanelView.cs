using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Scripts.Game
{
    public class GamePanelView : MonoBehaviour
    {
        public static readonly IMessageBroker Message = new MessageBroker();

        [SerializeField] private Button _settingsButton;
        [SerializeField] private Slider _reloadingSlider;

        private CompositeDisposable _disposables = new();
        private Coroutine _reloadCoroutine;

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

            Shooting.Message
                .Receive<M_Reloading>()
                .Subscribe(m => OnReloading(m.ReloadTime))
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

        private void SetSliderValue(float reloadTime)
        {
            _reloadingSlider.value = 0f;
            _reloadingSlider.maxValue = reloadTime;
        }

        private void OnReloading(float reloadTime)
        {
            SetSliderValue(reloadTime);

            if (_reloadCoroutine != null)
                StopCoroutine(_reloadCoroutine);

            _reloadCoroutine = StartCoroutine(PlayReloadingAnimation(reloadTime));
        }

        private IEnumerator PlayReloadingAnimation(float reloadTime)
        {
            _reloadingSlider.gameObject.SetActive(true);
            float elapsed = 0f;

            while (elapsed < reloadTime)
            {
                elapsed += Time.deltaTime;
                _reloadingSlider.value = Mathf.Clamp(elapsed, 0, reloadTime);
                yield return null;
            }

            _reloadingSlider.gameObject.SetActive(false);
            Message.Publish(new M_EndReloading());
        }
    }
}