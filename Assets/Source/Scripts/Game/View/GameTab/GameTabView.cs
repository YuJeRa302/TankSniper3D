using UniRx;
using UnityEngine;
using YG;

namespace Assets.Source.Scripts.Game
{
    public abstract class GameTabView : MonoBehaviour
    {
        public static readonly IMessageBroker Message = new MessageBroker();

        private void Start()
        {
            AddListeners();
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }

        public virtual void Open()
        {
            gameObject.SetActive(true);
            MessageBroker.Default.Publish(new M_OpenPanel());
        }

        public virtual void Close()
        {
            gameObject.SetActive(false);
            MessageBroker.Default.Publish(new M_ClosePanel());
        }

        protected virtual void AddListeners()
        {
            YG2.onOpenInterAdv += OnOpenFullscreenAdCallback;
            YG2.onCloseInterAdv += OnCloseFullscreenAdCallback;
            YG2.onErrorInterAdv += OnErrorFullAdCallback;
        }

        protected virtual void RemoveListeners()
        {
            YG2.onOpenInterAdv -= OnOpenFullscreenAdCallback;
            YG2.onCloseInterAdv -= OnCloseFullscreenAdCallback;
            YG2.onErrorInterAdv -= OnErrorFullAdCallback;
        }

        protected virtual void OpenFullscreenAds()
        {
            YG2.InterstitialAdvShow();
            MessageBroker.Default.Publish(new M_OpenFullscreenAd());
        }

        protected virtual void OnErrorFullAdCallback()
        {
            MessageBroker.Default.Publish(new M_CloseFullscreenAd());
        }

        protected virtual void OnOpenFullscreenAdCallback()
        {
            OpenFullscreenAds();
        }

        protected virtual void OnCloseFullscreenAdCallback()
        {
            MessageBroker.Default.Publish(new M_CloseFullscreenAd());
        }
    }
}