using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Scripts.Game
{
    public class ReloadingView : MonoBehaviour
    {
        public static readonly IMessageBroker Message = new MessageBroker();

        [SerializeField] private Slider _reloadingSlider;

        private Coroutine _reloadCoroutine;
        private CompositeDisposable _disposables = new();

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            Shooting.Message
                .Receive<M_Reloading>()
                .Subscribe(m => OnReloading(m.ReloadTime))
                .AddTo(_disposables);
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
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