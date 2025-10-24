using Assets.Source.Scripts.Services;
using System.Collections;
using UniRx;
using UnityEngine;

namespace Assets.Source.Scripts.Game
{
    public class CameraMover : MonoBehaviour
    {
        [SerializeField] private Camera _mainCamera;
        [Space(20)]
        [SerializeField] private float _normalFOV = 60f;
        [SerializeField] private float _sniperFOV = 20f;
        [SerializeField] private float _angleRotationY = 60f;
        [SerializeField] private float _angleRotationX = 45f;
        [SerializeField] private float _zoomSpeed = 8f;
        [SerializeField] private Vector3 _sniperPositionOffset = new(0, 0.5f, 0.5f);
        [Space(20)]
        [SerializeField] private float _normalSensitivity = 0.42f;
        [SerializeField] private float _sniperSensitivity = 0.05f;

        private GamePauseService _gamePauseService;
        private Coroutine _zoomCoroutine;
        private CompositeDisposable _disposables = new();
        private bool _isSniperMode = false;
        private bool _isCanRotation = true;
        private Vector3 _normalPosition;
        private float _rotationX = 0f;
        private float _rotationY = 0f;

        private void OnDestroy()
        {
            RemoveListeners();
        }

        private void Update()
        {
            if (_isCanRotation)
                RotationCamera();
        }

        public void Initialize(GamePauseService gamePauseService)
        {
            _gamePauseService = gamePauseService;
            SetCameraParameters();
            AddListeners();
        }

        private void AddListeners()
        {
            _gamePauseService.GamePaused += OnGamePause;
            _gamePauseService.GameResumed += OnGameResume;

            SniperScopeView.Message
                .Receive<M_Aiming>()
                .Subscribe(m => SetCameraZoom(m.IsAiming))
                .AddTo(_disposables);

            SniperScopeView.Message
                .Receive<M_CloseScope>()
                .Subscribe(m => SetCameraZoom(false))
                .AddTo(_disposables);
        }

        private void RemoveListeners()
        {
            _disposables.Dispose();
            _gamePauseService.GamePaused -= OnGamePause;
            _gamePauseService.GameResumed -= OnGameResume;
        }

        private void OnGamePause(bool state)
        {
            _isCanRotation = false;
        }

        private void OnGameResume(bool state)
        {
            _isCanRotation = true;
        }

        private void SetCameraParameters()
        {
            _normalPosition = _mainCamera.transform.localPosition;
            _mainCamera.fieldOfView = _normalFOV;

            Vector3 angles = _mainCamera.transform.localEulerAngles;
            _rotationY = angles.y;
            _rotationX = angles.x;
        }

        private void RotationCamera()
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
                float sensitivity = _isSniperMode ? _sniperSensitivity : _normalSensitivity;
                _rotationY += Input.GetAxis("Mouse X") * sensitivity;
                _rotationX -= Input.GetAxis("Mouse Y") * sensitivity;

                _rotationX = Mathf.Clamp(_rotationX, -_angleRotationX, _angleRotationX);
                _rotationY = Mathf.Clamp(_rotationY, -_angleRotationY, _angleRotationY);
                _mainCamera.transform.localRotation = Quaternion.Euler(_rotationX, _rotationY, 0);
            }
        }

        private void SetCameraZoom(bool isSniper)
        {
            float targetFOV = isSniper ? _sniperFOV : _normalFOV;
            Vector3 targetPosition = isSniper ? _normalPosition + _sniperPositionOffset : _normalPosition;

            if (_zoomCoroutine != null)
                StopCoroutine(_zoomCoroutine);

            _zoomCoroutine = StartCoroutine(ChangeCameraZoom(targetFOV, targetPosition));
        }

        private IEnumerator ChangeCameraZoom(float targetFOV, Vector3 targetPosition)
        {
            while (_mainCamera.fieldOfView != targetFOV)
            {
                _mainCamera.fieldOfView = Mathf.Lerp(_mainCamera.fieldOfView, targetFOV, Time.deltaTime * _zoomSpeed);

                _mainCamera.transform.localPosition = Vector3.Lerp(
                    _mainCamera.transform.localPosition,
                    targetPosition,
                    Time.deltaTime * _zoomSpeed);

                yield return null;
            }

            _mainCamera.fieldOfView = targetFOV;
            _mainCamera.transform.localPosition = targetPosition;
        }
    }
}