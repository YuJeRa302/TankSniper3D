using System;
using UniRx;
using UnityEngine;

namespace Assets.Source.Scripts.Game
{
    public class CameraMover : MonoBehaviour
    {
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private float _normalFOV = 60f;
        [SerializeField] private float _sniperFOV = 20f;
        [SerializeField] private float _zoomSpeed = 5f;
        [SerializeField] private Vector3 _sniperPositionOffset = new(0, 0.5f, 0.5f);

        private CompositeDisposable _disposables = new();
        private bool _isSniperMode = false;
        private Vector3 _normalPosition;
        private float _targetFOV;
        private Vector3 _targetPosition;
        private float _rotationX = 0f;
        private float _rotationY = 0f;

        public float normalSensitivity = 0.1f;
        public float sniperSensitivity = 0.05f;

        public Transform cameraPivot;

        private void Awake()
        {
            SniperScopeView.Message
                .Receive<M_EndAiming>()
                .Subscribe(m => SetNormalMod())
                .AddTo(_disposables);

            SniperScopeView.Message
                .Receive<M_Aiming>()
                .Subscribe(m => SetSniperMode())
                .AddTo(_disposables);
        }

        private void Start()
        {
            _normalPosition = _mainCamera.transform.localPosition;
            _mainCamera.fieldOfView = _normalFOV;

            _targetFOV = _normalFOV;
            _targetPosition = _normalPosition;

            _mainCamera.fieldOfView = _normalFOV;

            Vector3 angles = cameraPivot.localEulerAngles;
            _rotationY = angles.y;
            _rotationX = angles.x;
        }

        private void Update()
        {
            HandleTouchRotation();

            //if (Input.GetKeyDown(KeyCode.Tab)) // Переключение между режимами (на мобильных можно заменить на кнопку)
            //{
            //    isSniperMode = !isSniperMode;
            //    _sniperScopeView.gameObject.SetActive(isSniperMode);
            //}

            if (_isSniperMode)
            {
                _mainCamera.fieldOfView = Mathf.Lerp(_mainCamera.fieldOfView, _sniperFOV, Time.deltaTime * _zoomSpeed);
                _mainCamera.transform.localPosition = Vector3.Lerp(_mainCamera.transform.localPosition, _normalPosition + _sniperPositionOffset, Time.deltaTime * _zoomSpeed);
            }
            else
            {
                _mainCamera.fieldOfView = Mathf.Lerp(_mainCamera.fieldOfView, _normalFOV, Time.deltaTime * _zoomSpeed);
                _mainCamera.transform.localPosition = Vector3.Lerp(_mainCamera.transform.localPosition, _normalPosition, Time.deltaTime * _zoomSpeed);
            }

            // Добавьте сюда управление поворотом камеры, если нужно
        }

        private void HandleTouchRotation()
        {
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP_8_1
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Moved)
                {
                    float sensitivity = isSniperMode ? sniperSensitivity : normalSensitivity;

                    rotationY += touch.deltaPosition.x * sensitivity;
                    rotationX -= touch.deltaPosition.y * sensitivity;
                    rotationX = Mathf.Clamp(rotationX, -45f, 45f);

                    cameraPivot.localRotation = Quaternion.Euler(rotationX, rotationY, 0);
                }
            }
#endif

            if (Input.GetMouseButton(0))
            {
                float sensitivity = _isSniperMode ? sniperSensitivity : normalSensitivity;
                _rotationY += Input.mousePosition.normalized.x * sensitivity;
                _rotationX -= Input.mousePosition.normalized.y * sensitivity;
                _rotationX = Mathf.Clamp(_rotationX, -45f, 45f);

                cameraPivot.localRotation = Quaternion.Euler(_rotationX, _rotationY, 0);
            }
            else 
            {
                SetNormalMod();
            }
        }

        private void SetSniperMode()
        {
            _isSniperMode = true;
            _mainCamera.fieldOfView = Mathf.Lerp(_mainCamera.fieldOfView, _sniperFOV, Time.deltaTime * _zoomSpeed);
            _mainCamera.transform.localPosition = Vector3.Lerp(_mainCamera.transform.localPosition, _normalPosition + _sniperPositionOffset, Time.deltaTime * _zoomSpeed);
        }

        private void SetNormalMod()
        {
            _isSniperMode = false;
            _mainCamera.fieldOfView = Mathf.Lerp(_mainCamera.fieldOfView, _normalFOV, Time.deltaTime * _zoomSpeed);
            _mainCamera.transform.localPosition = Vector3.Lerp(_mainCamera.transform.localPosition, _normalPosition, Time.deltaTime * _zoomSpeed);
        }
    }
}