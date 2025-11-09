using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Source.Scripts.Game
{
    public class DroneScopeView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        public static readonly IMessageBroker Message = new MessageBroker();

        [SerializeField] private float _maxInterferenceDistance = 30f;
        [SerializeField] private DroneInterferenceEffect _interferenceEffectScript;

        private float _droneSpeed;
        private float _rotationSpeed;
        private DroneView _drone;
        private Camera _droneCamera;
        private Camera _freeLookCamera;
        private CompositeDisposable _disposables = new();
        private bool _isAiming = false;
        private Vector2 _dragInput;
        private Vector3 _initialDronePosition;
        private Quaternion _initialDroneRotation;
        private Coroutine _droneRoutine;
        private Image _crosshairButtonImage;
        private CrosshairButtonView _crosshairButton;

        private void OnDestroy()
        {
            RemoveListeners();
        }

        public void Initialize(Button sniperScopeButton)
        {
            gameObject.SetActive(false);
            _freeLookCamera = Camera.main;
            _interferenceEffectScript.Initialize();
            _interferenceEffectScript.gameObject.SetActive(false);
            _crosshairButton = sniperScopeButton.GetComponent<CrosshairButtonView>();
            _crosshairButtonImage = sniperScopeButton.GetComponent<Image>();
            AddListeners();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_isAiming)
                _dragInput = Vector2.zero;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_isAiming)
                _dragInput = eventData.delta;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _dragInput = Vector2.zero;
        }

        private IEnumerator DroneMovementRoutine()
        {
            while (_isAiming && _drone != null)
            {
                _drone.transform.position += _drone.transform.forward * _droneSpeed * Time.deltaTime;

                float yaw = _dragInput.x * _rotationSpeed * Time.deltaTime;
                float pitch = -_dragInput.y * _rotationSpeed * Time.deltaTime;

                _drone.transform.Rotate(pitch, yaw, 0);

                Vector3 euler = _drone.transform.rotation.eulerAngles;
                euler.z = 0;
                _drone.transform.rotation = Quaternion.Euler(euler);

                float distance = Vector3.Distance(_initialDronePosition, _drone.transform.position);
                float interferenceAmount = Mathf.Clamp01(distance / _maxInterferenceDistance);

                _interferenceEffectScript.UpdateEffect(interferenceAmount);

                if (distance >= _maxInterferenceDistance)
                {
                    DestroyDrone();
                    yield break;
                }

                yield return null;
            }
        }

        private void DestroyDrone()
        {
            if (_drone == null)
                return;

            Destroy(_drone.gameObject);
            StopDroneRoutine();

            _isAiming = false;
            _interferenceEffectScript.ResetEffect();
            _interferenceEffectScript.gameObject.SetActive(false);

            _freeLookCamera.enabled = true;
            _droneCamera.enabled = false;

            ChangeSniperScopeImageState(true);
            gameObject.SetActive(false);

            Message.Publish(new M_Aiming(false));
            Message.Publish(new M_DeathDrone());
        }

        private void StopDroneRoutine()
        {
            if (_droneRoutine != null)
                StopCoroutine(_droneRoutine);
        }

        private void AddListeners()
        {
            _crosshairButton.ButtonPressed += OnSniperScopeButtonPressed;
            _crosshairButton.ButtonReleased += OnSniperScopeButtonReleased;
            _crosshairButton.ButtonDragged += OnSniperScopeButtonDragged;

            DroneHealth.Message
                .Receive<M_DeathDrone>()
                .Subscribe(m => OnDroneDeath())
                .AddTo(_disposables);

            DroneHealth.Message
                .Receive<M_DeathDrone>()
                .Subscribe(m => OnDroneDeath())
                .AddTo(_disposables);

            GamePanelView.Message
                .Receive<M_CreateDrone>()
                .Subscribe(m => OnDroneCreated(m.DroneView))
                .AddTo(_disposables);
        }

        private void RemoveListeners()
        {
            _crosshairButton.ButtonDragged -= OnSniperScopeButtonDragged;
            _crosshairButton.ButtonPressed -= OnSniperScopeButtonPressed;
            _crosshairButton.ButtonReleased -= OnSniperScopeButtonReleased;
            _disposables?.Dispose();
        }

        private void OnSniperScopeButtonPressed(PointerEventData pointerEventData)
        {
            if (_drone == null)
                return;

            _isAiming = true;
            _freeLookCamera.enabled = false;
            _droneCamera.enabled = true;
            _drone.DisableHover();

            _initialDronePosition = _drone.transform.position;
            _initialDroneRotation = _drone.transform.rotation;

            ChangeSniperScopeImageState(false);
            gameObject.SetActive(true);
            _interferenceEffectScript.gameObject.SetActive(true);

            Message.Publish(new M_Aiming(true));
            Message.Publish(new M_Shoot());

            if (_droneRoutine != null)
                StopCoroutine(_droneRoutine);

            _droneRoutine = StartCoroutine(DroneMovementRoutine());

            OnPointerDown(pointerEventData);
        }

        private void OnSniperScopeButtonDragged(PointerEventData pointerEventData)
        {
            OnDrag(pointerEventData);
        }

        private void OnSniperScopeButtonReleased(PointerEventData pointerEventData)
        {
            OnPointerUp(pointerEventData);
        }

        private void ChangeSniperScopeImageState(bool state)
        {
            if (_crosshairButtonImage == null)
                return;

            _crosshairButtonImage.raycastTarget = state;
            _crosshairButtonImage.enabled = state;
        }

        private void OnDroneDeath()
        {
            _isAiming = false;
            _interferenceEffectScript.ResetEffect();
            _freeLookCamera.enabled = true;
            ChangeSniperScopeImageState(true);
            gameObject.SetActive(false);
            Message.Publish(new M_Aiming(false));
        }

        private void OnDroneCreated(DroneView droneView)
        {
            _drone = droneView;
            _rotationSpeed = droneView.RotationSpeed;
            _droneSpeed = droneView.DroneSpeed;
            _droneCamera = droneView.DroneCamera;
            _drone.transform.forward = _freeLookCamera.transform.forward;
        }
    }
}