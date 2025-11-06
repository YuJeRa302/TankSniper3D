using Assets.Source.Game.Scripts.Enemy;
using Assets.Source.Scripts.Upgrades;
using System.Collections;
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

        private readonly float _durationCameraShake = 0.15f;
        private readonly float _minValueCameraShake = -1f;
        private readonly float _maxValueCameraShake = 1f;
        private readonly float _magnitudeCameraShake = 0.25f;
        private readonly float _waitSniperScopeValue = 1f;

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

        private Image _crosshairButtonImage;
        private CrosshairButtonView _crosshairButton;
        private Button _sniperScopeButton;
        private bool _isAiming = false;
        private bool _isReloading = false;
        private bool _isFirstShoot = true;
        private CompositeDisposable _disposables = new();
        private Camera _sniperCamera;
        private Coroutine _waitOutOfAmmoCoroutine;

        private void OnDestroy()
        {
            RemoveListeners();
        }

        public void Initialize(List<Enemy> enemies, Button sniperScopeButton)
        {
            gameObject.SetActive(false);
            _sniperCamera = Camera.main;
            _sniperCrosshairView.Initialize(enemies);
            _sniperScopeButton = sniperScopeButton;
            _crosshairButton = _sniperScopeButton.GetComponent<CrosshairButtonView>();
            _crosshairButtonImage = _sniperScopeButton.GetComponent<Image>();
            AddListeners();
            Fill();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isAiming = true;

            if (_waitOutOfAmmoCoroutine != null)
                StopCoroutine(_waitOutOfAmmoCoroutine);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_isAiming || _isReloading)
                return;

            StartCoroutine(CameraShake(_sniperCamera.transform, _durationCameraShake, _magnitudeCameraShake));
            UpdateHandleOutOfAmmo();
            Message.Publish(new M_EndAiming());
            _isAiming = false;
        }

        private void AddListeners()
        {
            _crosshairButton.ButtonPressed += OnSniperScopeButtonPressed;
            _crosshairButton.ButtonReleased += OnSniperScopeButtonReleased;
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

            TankHealth.Message
                .Receive<M_DeathTank>()
                .Subscribe(m => OnCloseButtonClicked())
                .AddTo(_disposables);

            MessageBroker.Default
                .Receive<M_FinishGame>()
                .Subscribe(m => OnCloseButtonClicked())
                .AddTo(_disposables);
        }

        private void Fill()
        {
            RefillImages(_energyImages, _energySprite, _noneEnergySprite);
            RefillAmmo();
        }

        private void RemoveListeners()
        {
            _crosshairButton.ButtonPressed -= OnSniperScopeButtonPressed;
            _crosshairButton.ButtonReleased -= OnSniperScopeButtonReleased;
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

        private void RefillAmmo()
        {
            foreach (var image in _bulletImages)
            {
                if (image.gameObject.activeSelf == false)
                    image.gameObject.SetActive(true);
            }
        }

        private void DecreaseAmmoCount()
        {
            for (int index = _bulletImages.Count - 1; index >= 0; index--)
            {
                if (_bulletImages[index].gameObject.activeSelf == true)
                {
                    _bulletImages[index].gameObject.SetActive(false);
                    break;
                }
            }
        }

        private void SetSuperShotView()
        {
            var energyImage = _energyImages.LastOrDefault();

            if (energyImage.sprite != _noneEnergySprite)
            {
                _superShotImage.gameObject.SetActive(true);
                _sniperCrosshairView.SetActiveShooterZones(true);
            }
        }

        private void EndAiming()
        {
            _isAiming = false;

            if (_isReloading == false)
                ChangeSniperScopeImageState(true);

            gameObject.SetActive(false);
            _sniperCrosshairView.gameObject.SetActive(false);
            Message.Publish(new M_Aiming(false));
        }

        private void OnCloseButtonClicked()
        {
            _sniperCrosshairView.gameObject.SetActive(false);
            gameObject.SetActive(false);
            _isAiming = false;
            ChangeSniperScopeImageState(true);
            Message.Publish(new M_CloseScope());
        }

        private void OnSniperScopeButtonPressed(PointerEventData pointerEventData)
        {
            OnPointerDown(pointerEventData);
            ChangeSniperScopeImageState(false);
            _sniperCrosshairView.gameObject.SetActive(true);
            gameObject.SetActive(true);
            Message.Publish(new M_Aiming(true));

            if (_isFirstShoot == false)
                SetSuperShotView();
        }

        private void OnSniperScopeButtonReleased(PointerEventData pointerEventData)
        {
            OnPointerUp(pointerEventData);
        }

        private void OnReloading()
        {
            _isReloading = true;
            EndAiming();
        }

        private void OnReloaded()
        {
            _isReloading = false;
            ChangeSniperScopeImageState(true);
            RefillAmmo();
        }

        private void OnSuperShooting()
        {
            RefillImages(_energyImages, _noneEnergySprite, _energySprite);
            DecreaseAmmoCount();
            _superShotImage.gameObject.SetActive(false);
            _sniperCrosshairView.SetActiveShooterZones(false);
        }

        private void OnShooting()
        {
            DecreaseAmmoCount();
            IncreaseEnergyCount();
            _sniperCrosshairView.SetPlayerShoot();
            _isFirstShoot = false;
        }

        private void UpdateHandleOutOfAmmo()
        {
            if (_waitOutOfAmmoCoroutine != null)
                StopCoroutine(_waitOutOfAmmoCoroutine);

            _waitOutOfAmmoCoroutine = StartCoroutine(WaitOutOfAmmo());
        }

        private void ChangeSniperScopeImageState(bool state)
        {
            if (_crosshairButtonImage == null)
                return;

            _crosshairButtonImage.raycastTarget = state;
            _crosshairButtonImage.enabled = state;
        }

        private IEnumerator WaitOutOfAmmo()
        {
            yield return new WaitForSeconds(_waitSniperScopeValue);
            EndAiming();
        }

        private IEnumerator CameraShake(Transform camTransform, float duration, float magnitude)
        {
            Vector3 originalPos = camTransform.localPosition;
            float elapsed = 0.0f;

            while (elapsed < duration)
            {
                float x = Random.Range(_minValueCameraShake, _maxValueCameraShake) * magnitude;
                float y = Random.Range(_minValueCameraShake, _maxValueCameraShake) * magnitude;

                camTransform.localPosition = originalPos + new Vector3(x, y, 0);

                elapsed += Time.deltaTime;
                yield return null;
            }

            camTransform.localPosition = originalPos;
        }
    }
}