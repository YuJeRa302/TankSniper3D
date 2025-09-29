using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Game.Scripts.States;
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
        private readonly int _firstIndex = 0;

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
        [SerializeField] private Button _buyButton;
        [SerializeField] private Button _selectButton;

        private HeroState _heroState;
        private HeroData _heroData;
        private CompositeDisposable _disposables = new();

        public event Action<HeroCardView> Selected;
        public event Action<int, TypeCard> BuyButtonClicked;

        public HeroData HeroData => _heroData;
        public HeroState HeroState => _heroState;

        private void OnDestroy()
        {
            RemoveListeners();
        }

        public void Initialize(HeroData heroData, HeroState heroState)
        {
            _heroData = heroData;
            _heroState = heroState;
            AddListeners();
            Fill();
            Lock();

            if (_heroData.Id == _firstIndex)
                Unlock();

            UnlockByPlayerProgress();
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

        private void BuyButtonClick()
        {
            BuyButtonClicked?.Invoke(_heroData.Id, _heroData.TypeCard);
        }

        private void Lock()
        {
            if (_heroData.Id == _firstIndex)
                return;

            _selectButton.interactable = false;
            _scrollStarView.gameObject.SetActive(false);
            _mainImage.gameObject.SetActive(false);
            _selectButtonImage.color = _lockedColor;
            _lockStarImage.gameObject.SetActive(true);
        }

        private void Unlock()
        {
            _selectButton.interactable = true;
            _scrollStarView.gameObject.SetActive(true);
            _mainImage.gameObject.SetActive(true);
            _selectButtonImage.color = _defaultColor;
            _lockStarImage.gameObject.SetActive(false);
        }

        private void UnlockByPlayerProgress()
        {
            if (_heroData.Id == _firstIndex)
                return;

            if (_heroState.IsBuyed == true)
            {
                Unlock();
                EquippByPlayerProgress();
            }
        }

        private void AddListeners()
        {
            _selectButton.onClick.AddListener(SelectButtonClick);
            _buyButton.onClick.AddListener(BuyButtonClick);

            UpgradeView.Message
                .Receive<M_UpgradeUnlocked>()
                .Subscribe(m => OnUpgradeUnlocked(m.Id, m.TypeCard))
                .AddTo(_disposables);

            UpgradeView.Message
                .Receive<M_DeselectCards>()
                .Subscribe(m => OnDeselect(m.Id))
                .AddTo(_disposables);
        }

        private void RemoveListeners()
        {
            _selectButton.onClick.RemoveListener(SelectButtonClick);
            _buyButton.onClick.RemoveListener(BuyButtonClick);
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

        private void EquippByPlayerProgress()
        {
            if (_heroState.IsEquipped == true)
                Select();
        }

        private void Select()
        {
            Selected?.Invoke(this);
            _chooseMark.gameObject.SetActive(true);
        }

        private void OnUpgradeUnlocked(int id, TypeCard typeCard)
        {
            if (_heroData.TypeCard == typeCard)
            {
                if (_heroData.Id == id)
                    Unlock();
            }
        }

        private void OnDeselect(int id)
        {
            if (_heroData.Id == id)
                return;

            _chooseMark.gameObject.SetActive(false);
            _heroState.ChangeEquippedState(false);
        }
    }
}