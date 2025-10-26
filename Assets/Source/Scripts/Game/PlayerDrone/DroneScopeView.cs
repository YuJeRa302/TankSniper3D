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

        [SerializeField] private float _maxInterferenceDistance = 50f;
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
        private Button _sniperScopeButton;
        private Coroutine _droneRoutine;

        private void OnDestroy()
        {
            RemoveListeners();
        }

        public void Initialize(Button sniperScopeButton)
        {
            gameObject.SetActive(false);
            _sniperScopeButton = sniperScopeButton;
            _freeLookCamera = Camera.main;
            _interferenceEffectScript.Initialize();
            _interferenceEffectScript.gameObject.SetActive(false);
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

        private void OnAimButtonPressed()
        {
            if (_drone == null)
                return;

            _isAiming = true;
            _freeLookCamera.enabled = false;
            _droneCamera.enabled = true;
            _initialDronePosition = _drone.transform.position;
            _initialDroneRotation = _drone.transform.rotation;
            _sniperScopeButton.gameObject.SetActive(false);
            gameObject.SetActive(true);
            _interferenceEffectScript.gameObject.SetActive(true);

            Message.Publish(new M_Aiming(true));
            Message.Publish(new M_Shoot());

            if (_droneRoutine != null)
                StopCoroutine(_droneRoutine);

            _droneRoutine = StartCoroutine(DroneMovementRoutine());
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
            //_drone = null;

            StopDroneRoutine();

            _isAiming = false;
            _interferenceEffectScript.ResetEffect();
            _interferenceEffectScript.gameObject.SetActive(false);
            _freeLookCamera.enabled = true;
            _droneCamera.enabled = false;
            _sniperScopeButton.gameObject.SetActive(true);
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
            _sniperScopeButton.onClick.AddListener(OnAimButtonPressed);

            DroneHealth.Message
                .Receive<M_DeathDrone>()
                .Subscribe(m => OnDroneDeath())
                .AddTo(_disposables);

            DroneView.Message
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
            _sniperScopeButton.onClick.RemoveListener(OnAimButtonPressed);
            _disposables?.Dispose();
        }

        private void OnDroneDeath()
        {
            _isAiming = false;
            _interferenceEffectScript.ResetEffect();
            _freeLookCamera.enabled = true;
            _sniperScopeButton.gameObject.SetActive(true);
            gameObject.SetActive(false);
            Message.Publish(new M_Aiming(false));
        }

        private void OnDroneCreated(DroneView droneView)
        {
            _drone = droneView;
            _rotationSpeed = droneView.RotationSpeed;
            _droneSpeed = droneView.DroneSpeed;
            _droneCamera = droneView.DroneCamera;
        }
    }
}