using Assets.Source.Game.Scripts.States;
using Assets.Source.Game.Scripts.Utility;
using Assets.Source.Scripts.ScriptableObjects;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Scripts.Upgrades
{
    public class HeroCardView : MonoBehaviour
    {
        [SerializeField] private Color _lockedColor = new Color32(66, 72, 132, 255);
        [SerializeField] private Color _defaultColor = new Color32(255, 255, 255, 255);
        [SerializeField] private Image _icon;
        [SerializeField] private Image _selectButtonImage;
        [SerializeField] private Image _lockStarImage;
        [SerializeField] private Image _mainImage;
        [SerializeField] private GameObject _chooseMark;
        [Space(20)]
        [SerializeField] private ScrollRect _scrollStarView;
        [Space(20)]
        [SerializeField] private List<Image> _starImages;
        [SerializeField] private Sprite _star;
        [Space(20)]
        [SerializeField] private Button _selectButton;
        [Space(20)]
        [SerializeField] private UpgradeButtonAdsWaiter _buttonAdsWaiter;

        private HeroState _heroState;
        private HeroData _heroData;
        private CompositeDisposable _disposables = new();

        public event Action<HeroCardView> Selected;

        public UpgradeButtonAdsWaiter UpgradeButtonAdsWaiter => _buttonAdsWaiter;
        public HeroData HeroData => _heroData;
        public HeroState HeroState => _heroState;

        private void OnDestroy()
        {
            RemoveListeners();
        }

        public void Initialize(HeroData heroData, HeroState heroState, TankState tankState)
        {
            _heroData = heroData;
            _heroState = heroState;
            _buttonAdsWaiter.Initialize(_heroData.Id, _heroData.TypeCard);
            AddListeners();
            Fill();
            Lock();
            Unlock();
            Purchased();
            UnlockByPlayerProgress(tankState);
        }

        private void Fill()
        {
            _icon.sprite = _heroData.Sprite;
            ChangeStarCount();
        }

        private void SelectButtonClick()
        {
            Select();
        }

        private void Lock()
        {
            _selectButton.interactable = false;
            _scrollStarView.gameObject.SetActive(false);
            _mainImage.gameObject.SetActive(false);
            _selectButtonImage.color = _lockedColor;
            _lockStarImage.gameObject.SetActive(true);
        }

        private void Unlock()
        {
            if (_heroState.IsOpened != true)
                return;

            _selectButtonImage.color = _defaultColor;
            _lockStarImage.gameObject.SetActive(false);
            _mainImage.gameObject.SetActive(true);
            _buttonAdsWaiter.gameObject.SetActive(true);
        }

        private void Purchased()
        {
            if (_heroState.IsBuyed != true)
                return;

            if (_heroState.IsOpened != true)
                return;

            _selectButton.interactable = true;
            _scrollStarView.gameObject.SetActive(true);
            _mainImage.gameObject.SetActive(true);
            _selectButtonImage.color = _defaultColor;
            _lockStarImage.gameObject.SetActive(false);
            _buttonAdsWaiter.gameObject.SetActive(false);
        }

        private void AddListeners()
        {
            _selectButton.onClick.AddListener(SelectButtonClick);

            UpgradeView.Message
                .Receive<M_UpgradeUnlocked>()
                .Subscribe(m => OnPurchasedByAds(m.Id))
                .AddTo(_disposables);

            UpgradeView.Message
                .Receive<M_DeselectCards>()
                .Subscribe(m => OnDeselect(m.Id))
                .AddTo(_disposables);
        }

        private void RemoveListeners()
        {
            _selectButton.onClick.RemoveListener(SelectButtonClick);
            _disposables?.Dispose();
        }

        private void ChangeStarCount()
        {
            int index = 0;

            while (index < _heroData.StarCount)
            {
                _starImages[index].sprite = _star;
                index++;
            }
        }

        private void UnlockByPlayerProgress(TankState state)
        {
            if (_heroState.Id == state.HeroId)
                Select();
        }

        private void Select()
        {
            Selected?.Invoke(this);
            _chooseMark.gameObject.SetActive(true);
        }

        private void OnPurchasedByAds(int id)
        {
            if (_heroData.Id == id)
                Purchased();
        }

        private void OnDeselect(int id)
        {
            if (_heroData.Id == id)
                return;

            _chooseMark.gameObject.SetActive(false);
        }
    }
}