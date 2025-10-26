using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Scripts.Models;
using Assets.Source.Scripts.ScriptableObjects;
using UniRx;
using UnityEngine;
using YG;

namespace Assets.Source.Scripts.Game
{
    public abstract class EndGameTabView : MonoBehaviour
    {
        public static readonly IMessageBroker Message = new MessageBroker();

        protected GameModel GameModel => _gameModel;
        protected CompositeDisposable Disposable => _disposables;
        protected LevelData LevelData => _levelData;

        private LevelData _levelData;
        private GameModel _gameModel;
        private CompositeDisposable _disposables = new();

        public abstract TypeLevel TypeLevel { get; }

        private void OnDestroy()
        {
            RemoveListeners();
        }

        public virtual void Initialize(GameModel gameModel)
        {
            _gameModel = gameModel;
            _levelData = _gameModel.GetLevelData();
            gameObject.SetActive(false);
            AddListeners();
        }

        public virtual void Open()
        {
            gameObject.SetActive(true);
            Message.Publish(new M_OpenPanel());
        }

        public virtual void Close()
        {
            gameObject.SetActive(false);
            MessageBroker.Default.Publish(new M_ClosePanel());
        }

        protected bool TryOpen(LevelData levelData)
        {
            return levelData.TypeLevel != TypeLevel;
        }

        protected virtual void AddListeners()
        {
            YG2.onOpenInterAdv += OnOpenFullscreenAdCallback;
            YG2.onCloseInterAdv += OnCloseFullscreenAdCallback;
            YG2.onErrorInterAdv += OnErrorFullAdCallback;

            MessageBroker.Default
                .Receive<M_FinishGame>()
                .Subscribe(m => Open())
                .AddTo(_disposables);
        }

        protected virtual void RemoveListeners()
        {
            YG2.onOpenInterAdv -= OnOpenFullscreenAdCallback;
            YG2.onCloseInterAdv -= OnCloseFullscreenAdCallback;
            YG2.onErrorInterAdv -= OnErrorFullAdCallback;
            _disposables?.Dispose();
        }

        protected virtual void OpenFullscreenAds()
        {
            YG2.InterstitialAdvShow();
        }

        protected virtual void OnErrorFullAdCallback()
        {
            MessageBroker.Default.Publish(new M_CloseFullscreenAd());
        }

        protected virtual void OnOpenFullscreenAdCallback()
        {
            MessageBroker.Default.Publish(new M_OpenFullscreenAd());
        }

        protected virtual void OnCloseFullscreenAdCallback()
        {
            MessageBroker.Default.Publish(new M_CloseFullscreenAd());
        }
    }
}