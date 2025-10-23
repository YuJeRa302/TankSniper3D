using Assets.Source.Game.Scripts.Enemy;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Source.Scripts.Game
{
    public class SniperScopeView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public static readonly IMessageBroker Message = new MessageBroker();

        [SerializeField] private List<Image> _bulletImages;
        [SerializeField] private List<Image> _energyImages;
        [Space(20)]
        [SerializeField] private Sprite _ammoSprite;
        [SerializeField] private Sprite _noneAmmoSprite;
        [Space(20)]
        [SerializeField] private Sprite _energySprite;
        [SerializeField] private Sprite _noneEnergySprite;
        [Space(20)]
        [SerializeField] private Image _superShotImage;
        [Space(20)]
        [SerializeField] private Button _closeScope;
        [Space(20)]
        [SerializeField] private SniperCrosshairView _sniperCrosshairView;

        private Button _sniperScopeButton;
        private bool _isAiming = false;
        private CompositeDisposable _disposables = new();

        private void OnDestroy()
        {
            RemoveListeners();
        }

        public void Initialize(List<Enemy> enemies, Button sniperScopeButton)
        {
            gameObject.SetActive(false);
            _sniperCrosshairView.Initialize(enemies);
            _sniperScopeButton = sniperScopeButton;
            AddListeners();
            Fill();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isAiming = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_isAiming)
                return;

            EndAiming();
        }

        private void AddListeners()
        {
            _sniperScopeButton.onClick.AddListener(OnSniperScopeButtonClicked);
            _closeScope.onClick.AddListener(OnCloseButtonClicked);

            Shooting.Message
                .Receive<M_Reloading>()
                .Subscribe(m => OnReloading())
                .AddTo(_disposables);

            ReloadingView.Message
                .Receive<M_EndReloading>()
                .Subscribe(m => OnReloaded())
                .AddTo(_disposables);

            Shooting.Message
                .Receive<M_Shoot>()
                .Subscribe(m => OnShooting())
                .AddTo(_disposables);

            Shooting.Message
                .Receive<M_SuperShoot>()
                .Subscribe(m => OnSuperShooting())
                .AddTo(_disposables);
        }

        private void Fill()
        {
            RefillImages(_bulletImages, _ammoSprite, _noneAmmoSprite);
            RefillImages(_energyImages, _noneEnergySprite, _energySprite);
        }

        private void RemoveListeners()
        {
            _sniperScopeButton.onClick.RemoveListener(OnSniperScopeButtonClicked);
            _closeScope.onClick.RemoveListener(OnCloseButtonClicked);
            _disposables?.Dispose();
        }

        private void RefillImages(List<Image> images, Sprite fillingSprite, Sprite foundSprite)
        {
            foreach (var image in images)
            {
                if (image.sprite == foundSprite)
                    image.sprite = fillingSprite;
            }
        }

        private void IncreaseEnergyCount()
        {
            for (int index = 0; index < _energyImages.Count; index++)
            {
                if (_energyImages[index].sprite == _noneEnergySprite)
                {
                    _energyImages[index].sprite = _energySprite;
                    break;
                }
            }
        }

        private void DecreaseAmmoCount()
        {
            for (int index = _bulletImages.Count - 1; index >= 0; index--)
            {
                if (_bulletImages[index].sprite == _ammoSprite)
                {
                    _bulletImages[index].sprite = _noneAmmoSprite;
                    break;
                }
            }
        }

        private void SetSuperShotImage()
        {
            var energyImage = _energyImages.LastOrDefault();

            if (energyImage.sprite != _noneEnergySprite)
                _superShotImage.gameObject.SetActive(true);
        }

        private void EndAiming()
        {
            _isAiming = false;
            _sniperScopeButton.gameObject.SetActive(true);
            gameObject.SetActive(false);
            Message.Publish(new M_Aiming(false));
        }

        private void OnCloseButtonClicked()
        {
            gameObject.SetActive(false);
            _isAiming = false;
            _sniperScopeButton.gameObject.SetActive(true);
            Message.Publish(new M_CloseScope());
        }

        private void OnSniperScopeButtonClicked()
        {
            _sniperScopeButton.gameObject.SetActive(false);
            gameObject.SetActive(true);
            Message.Publish(new M_Aiming(true));
            SetSuperShotImage();
        }

        private void OnReloading()
        {
            _sniperScopeButton.gameObject.SetActive(false);
            _isAiming = false;
        }

        private void OnReloaded()
        {
            _sniperScopeButton.gameObject.SetActive(true);
            RefillImages(_bulletImages, _ammoSprite, _noneAmmoSprite);
        }

        private void OnSuperShooting()
        {
            RefillImages(_energyImages, _noneEnergySprite, _energySprite);
            DecreaseAmmoCount();
            _superShotImage.gameObject.SetActive(false);
        }

        private void OnShooting()
        {
            DecreaseAmmoCount();
            IncreaseEnergyCount();
        }
    }
}