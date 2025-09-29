using Assets.Source.Game.Scripts.States;
using Assets.Source.Scripts.ScriptableObjects;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Scripts.Upgrades
{
    public class TankCardView : MonoBehaviour
    {
        [SerializeField] private Color _lockedColor = new Color32(66, 72, 132, 255);
        [SerializeField] private Color _defaultColor = new Color32(255, 255, 255, 255);
        [SerializeField] private Image _icon;
        [SerializeField] private Image _selectButtonImage;
        [SerializeField] private Image _lockStarImage;
        [SerializeField] private Image _mainImage;
        [SerializeField] private GameObject _chooseMark;
        [SerializeField] private Button _selectButton;
        [Space(20)]
        [SerializeField] private ScrollRect _scrollStarView;
        [Space(20)]
        [SerializeField] private List<Image> _starImages;
        [SerializeField] private Sprite _star;

        private TankState _tankState;
        private TankData _tankData;
        private CompositeDisposable _disposables = new();

        public event Action<TankCardView> Selected;

        public TankData TankData => _tankData;
        public TankState TankState => _tankState;

        private void OnDestroy()
        {
            RemoveListeners();
        }

        public void Initialize(TankData tankData, TankState tankState)
        {
            _tankData = tankData;
            _tankState = tankState;
            AddListeners();
            Fill();
            Lock();
            UnlockByPlayerProgress();
        }

        private void SelectButtonClick()
        {
            Select();
        }

        private void Fill()
        {
            _icon.sprite = _tankData.Icon;
            ChangeStarCount();
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
            _selectButton.interactable = true;
            _scrollStarView.gameObject.SetActive(true);
            _mainImage.gameObject.SetActive(true);
            _selectButtonImage.color = _defaultColor;
            _lockStarImage.gameObject.SetActive(false);
        }

        private void ChangeStarCount()
        {
            int index = 0;

            while (index < _tankData.StarCount)
            {
                _starImages[index].sprite = _star;
                index++;
            }
        }

        private void UnlockByPlayerProgress()
        {
            if (_tankState.IsOpened == true)
            {
                Unlock();
                EquippByPlayerProgress();
            }
        }

        private void EquippByPlayerProgress()
        {
            if (_tankState.IsEquipped == true)
                Select();
        }

        private void Select()
        {
            Selected?.Invoke(this);
            _chooseMark.gameObject.SetActive(true);
        }

        private void AddListeners()
        {
            _selectButton.onClick.AddListener(SelectButtonClick);

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

        private void OnDeselect(int id)
        {
            if (_tankData.Id == id)
                return;

            _chooseMark.gameObject.SetActive(false);
            _tankState.ChangeEquippedState(false);
        }
    }
}