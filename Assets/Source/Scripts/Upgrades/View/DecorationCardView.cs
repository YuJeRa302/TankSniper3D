using Assets.Source.Game.Scripts.States;
using Assets.Source.Game.Scripts.Utility;
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
        [SerializeField] private Image _highlightImage;
        [SerializeField] private GameObject _chooseMark;
        [SerializeField] private GameObject _lockPanel;
        [SerializeField] private Button _selectButton;
        [Space(20)]
        [SerializeField] private UpgradeButtonAdsWaiter _buttonAdsWaiter;

        private CompositeDisposable _disposables = new();
        private DecorationData _decorationData;
        private DecorationState _decorationState;

        public event Action<DecorationCardView> DecorationSelected;

        public UpgradeButtonAdsWaiter UpgradeButtonAdsWaiter => _buttonAdsWaiter;
        public DecorationData DecorationData => _decorationData;
        public DecorationState DecorationState => _decorationState;

        private void OnDestroy()
        {
            RemoveListeners();
        }

        public void Initialize(DecorationData decorationData, DecorationState decorationState, TankState tankState)
        {
            _decorationData = decorationData;
            _decorationState = decorationState;
            _icon.sprite = decorationData.Sprite;
            _buttonAdsWaiter.Initialize(_decorationData.Id, _decorationData.TypeCard);
            AddListeners();
            Lock();

            if (_decorationData.Id == _firstIndex)
                Unlock();

            UnlockByPlayerProgress(tankState);
        }

        private void SelectButtonClick()
        {
            Select();
        }

        private void Lock()
        {
            if (_decorationData.Id == _firstIndex)
                return;

            _lockPanel.gameObject.SetActive(true);
            _buttonAdsWaiter.gameObject.SetActive(true);
            _selectButton.interactable = false;
        }

        private void Unlock()
        {
            _lockPanel.gameObject.SetActive(false);
            _buttonAdsWaiter.gameObject.SetActive(false);
            _selectButton.interactable = true;
        }

        private void AddListeners()
        {
            _selectButton.onClick.AddListener(SelectButtonClick);

            UpgradeView.Message
                .Receive<M_UpgradeUnlocked>()
                .Subscribe(m => OnUnlockByAds(m.Id))
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

        private void Select()
        {
            DecorationSelected?.Invoke(this);
            _chooseMark.gameObject.SetActive(true);
            _highlightImage.enabled = true;
        }

        private void UnlockByPlayerProgress(TankState tankState)
        {
            if (_decorationState.IsBuyed == true)
            {
                Unlock();
                EquippByPlayerProgress(tankState);
            }
        }

        private void EquippByPlayerProgress(TankState tankState)
        {
            if (tankState.TryEquipDecoration(_decorationState))
                Select();
        }

        private void OnUnlockByAds(int id)
        {
            if (_decorationData.Id == id)
                Unlock();

            _buttonAdsWaiter.SetWaitAds();
        }

        private void OnDeselect(int id)
        {
            if (_decorationData.Id == id)
                return;

            _chooseMark.gameObject.SetActive(false);
            _highlightImage.enabled = false;
        }
    }
}