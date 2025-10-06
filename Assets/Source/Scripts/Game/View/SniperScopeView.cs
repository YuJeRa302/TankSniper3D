using System.Collections.Generic;
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
        [SerializeField] private Button _sniperScopeButton;
        [Space(20)]
        [SerializeField] private Sprite _ammoSprite;
        [SerializeField] private Sprite _noneAmmoSprite;
        [Space(20)]
        [SerializeField] private Sprite _energySprite;
        [SerializeField] private Sprite _noneEnergySprite;
        [Space(20)]
        [SerializeField] private Image _superShotImage;
        [Space(20)]
        [SerializeField] private GameObject _scopeEntities;

        private bool _isAiming = false;
        private CompositeDisposable _disposables = new();

        private void Awake()
        {
            AddListeners();
        }
        
        private void OnDestroy()
        {
            RemoveListeners();
        }

        public void Initialize()
        {
            gameObject.SetActive(false);
            AddListeners();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isAiming = true;
            Debug.Log("_isAiming" + _isAiming);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_isAiming)
                return;

            //Message.Publish(new M_EndAiming());
            _isAiming = false;
        }

        private void AddListeners()
        {
            _sniperScopeButton.onClick.AddListener(OnSniperScopeButtonClicked);

            Shooting.Message
                .Receive<M_Reloading>()
                .Subscribe(m => OnReloading())
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

        private void RemoveListeners()
        {
            _sniperScopeButton.onClick.RemoveListener(OnSniperScopeButtonClicked);
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
            foreach (var image in _energyImages)
            {
                if (image.sprite == _noneEnergySprite)
                    image.sprite = _energySprite;
            }
        }

        private void DecreaseAmmoCount()
        {
            for (int index = _bulletImages.Count - 1; index > 0; index--)
            {
                if (_bulletImages[index].sprite == _ammoSprite)
                    _bulletImages[index].sprite = _noneAmmoSprite;
            }
        }

        private bool TrySetSuperShotImage()
        {
            foreach (var image in _energyImages)
            {
                if (image.sprite != _noneEnergySprite)
                    return true;
            }

            return false;
        }

        private void OnSniperScopeButtonClicked()
        {
            _isAiming = true;
            _sniperScopeButton.gameObject.SetActive(false);
            _scopeEntities.SetActive(true);
        }

        private void OnReloading()
        {
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

            if (TrySetSuperShotImage())
                _superShotImage.gameObject.SetActive(true);
        }
    }
}