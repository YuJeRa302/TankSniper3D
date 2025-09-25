using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Game.Scripts.States;
using Assets.Source.Scripts.ScriptableObjects;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Scripts.Upgrades
{
    public class DecorationCardView : MonoBehaviour
    {
        private readonly int _firstIndex = 0;

        [SerializeField] private Image _icon;
        [SerializeField] private GameObject _chooseMark;
        [SerializeField] private GameObject _lockPanel;
        [SerializeField] private Button _buyButton;
        [SerializeField] private Button _selectButton;

        private CompositeDisposable _disposables = new();
        private DecorationData _decorationData;
        private DecorationState _decorationState;

        public event Action<DecorationCardView> DecorationSelected;
        public event Action<int, TypeCard> BuyButtonClicked;

        public DecorationData DecorationData => _decorationData;
        public DecorationState DecorationState => _decorationState;

        private void OnDestroy()
        {
            RemoveListeners();
        }

        public void Initialize(DecorationData decorationData, DecorationState decorationState)
        {
            _decorationData = decorationData;
            _decorationState = decorationState;
            _icon.sprite = decorationData.Sprite;
            AddListeners();
            Lock();

            if (_decorationData.Id == _firstIndex)
                Unlock();

            UnlockByPlayerProgress();
        }

        private void SelectButtonClick()
        {
            Select();
        }

        private void BuyButtonClick()
        {
            BuyButtonClicked?.Invoke(_decorationData.Id, _decorationData.TypeCard);
        }

        private void Lock()
        {
            if (_decorationData.Id == _firstIndex)
                return;

            _lockPanel.gameObject.SetActive(true);
            _buyButton.gameObject.SetActive(true);
            _selectButton.interactable = false;
        }

        private void Unlock()
        {
            _lockPanel.gameObject.SetActive(false);
            _buyButton.gameObject.SetActive(false);
            _selectButton.interactable = true;
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

        private void Select()
        {
            DecorationSelected?.Invoke(this);
            _chooseMark.gameObject.SetActive(true);
        }

        private void UnlockByPlayerProgress()
        {
            if (_decorationState.Id == _firstIndex)
                return;

            if (_decorationState.IsBuyed == true)
            {
                Unlock();
                EquippByPlayerProgress();
            }
        }

        private void EquippByPlayerProgress()
        {
            if (_decorationState.IsEquipped == true)
                Select();
        }

        private void OnUpgradeUnlocked(int id, TypeCard typeCard) // возможно нужно будет убрать if с TypeCard
        {
            if (_decorationData.TypeCard == typeCard)
            {
                if (_decorationData.Id == id)
                    Unlock();
            }
        }

        private void OnDeselect(int id)
        {
            if (_decorationData.Id == id)
                return;

            _chooseMark.gameObject.SetActive(false);
            _decorationState.ChangeEquippedState(false);
        }
    }
}