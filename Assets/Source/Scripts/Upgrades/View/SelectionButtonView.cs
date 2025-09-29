using Assets.Source.Scripts.ScriptableObjects;
using Assets.Source.Scripts.Services;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Scripts.Upgrades
{
    public class SelectionButtonView : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private Image _buttonImage;
        [SerializeField] private Color _selectColorIcon;
        [SerializeField] private Color _deselectColorIcon;
        [SerializeField] private Color _selectColorButton;
        [SerializeField] private Color _deselectColorButton;
        [SerializeField] private Button _button;

        private CompositeDisposable _disposables = new();
        private IUseButtonStrategy _useButtonStrategy;
        private SelectionButtonData _selectionButtonData;

        public SelectionButtonData SelectionButtonData => _selectionButtonData;
        public IUseButtonStrategy IUseButtonStrategy => _useButtonStrategy;

        private void OnDestroy()
        {
            RemoveListeners();
        }

        public void Initialize(SelectionButtonData selectionButtonData)
        {
            _selectionButtonData = selectionButtonData;
            _useButtonStrategy = selectionButtonData.UseButtonStrategy;
            _icon.sprite = _selectionButtonData.Icon;
            AddListeners();
        }

        private void AddListeners()
        {
            _button.onClick.AddListener(ClickButton);

            UpgradeView.Message
                .Receive<M_DeselectButtons>()
                .Subscribe(m => Deselect(m.SelectionButtonView))
                .AddTo(_disposables);
        }

        private void RemoveListeners()
        {
            _button.onClick.RemoveListener(ClickButton);
            _disposables?.Dispose();
        }

        private void ClickButton()
        {
            _useButtonStrategy.ClickButton(this, _selectionButtonData.TypeCard);
            Select();
        }

        private void Select()
        {
            _icon.color = new Color(_selectColorIcon.r, _selectColorIcon.g, _selectColorIcon.b);
            _buttonImage.color = new Color(_selectColorButton.r, _selectColorButton.g, _selectColorButton.b);
        }

        private void Deselect(SelectionButtonView selectionButtonView)
        {
            if (this == selectionButtonView)
                return;

            _icon.color = new Color(_deselectColorIcon.r, _deselectColorIcon.g, _deselectColorIcon.b);
            _buttonImage.color = new Color(_deselectColorButton.r, _deselectColorButton.g, _deselectColorButton.b);
        }
    }
}