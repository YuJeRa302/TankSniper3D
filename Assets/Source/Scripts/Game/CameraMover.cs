using Assets.Source.Scripts.Services;
using DG.Tweening;
using System.Collections;
using UniRx;
using UnityEngine;

namespace Assets.Source.Scripts.Game
{
    public class CameraMover : MonoBehaviour
    {
        private readonly float _defaultXRotation = 9;
        private readonly float _defaultYRotation = 24;
        private readonly float _cameraZoomMultiplier = 1f;

        [SerializeField] private Camera _mainCamera;
        [SerializeField] private Transform _target;
        [Space(20)]
        [SerializeField] private float _normalFOV = 60f;
        [SerializeField] private float _sniperFOV = 20f;
        [SerializeField] private float _angleRotationMinY = 60f;
        [SerializeField] private float _angleRotationMaxY = 45f;
        [SerializeField] private float _angleRotationMinX = 45f;
        [SerializeField] private float _angleRotationMaxX = 45f;
        [SerializeField] private float _zoomSpeed = 8f;
        [SerializeField] private Vector3 _sniperPositionOffset = new(0, 0.5f, 0.5f);
        [Space(20)]
        [SerializeField] private float _normalSensitivity = 0.42f;
        [SerializeField] private float _sniperSensitivity = 0.05f;
        [Space(10)]
        [SerializeField] private float _distanceFromTarget = 7f;

        private GamePauseService _gamePauseService;
        private CompositeDisposable _disposables = new();
        private Coroutine _rotationCoroutine;
        private bool _isSniperMode;
        private bool _isCanRotate = true;
        private float _rotationX;
        private float _rotationY;

        private void OnDestroy()
        {
            RemoveListeners();
        }

        public void Initialize(GamePauseService gamePauseService)
        {
            _gamePauseService = gamePauseService;
            AddListeners();
            ResetCameraToDefault();

            if (_rotationCoroutine != null)
                StopCoroutine(_rotationCoroutine);

            _rotationCoroutine = StartCoroutine(RotateCameraLoop());
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

            DefeatTab.Message.
                Receive<M_OpenPanel>().
                Subscribe(m => OnGamePause(false)).
                AddTo(_disposables);

            EndGameTabView.Message.
                Receive<M_OpenPanel>().
                Subscribe(m => OnGamePause(false)).
                AddTo(_disposables);
        }

        private void RemoveListeners()
        {
            _disposables.Dispose();
            _gamePauseService.GamePaused -= OnGamePause;
            _gamePauseService.GameResumed -= OnGameResume;
        }

        private void OnGamePause(bool state)
        {
            if (_rotationCoroutine != null)
                StopCoroutine(_rotationCoroutine);
        }

        private void OnGameResume(bool state)
        {
            if (_rotationCoroutine != null)
                StopCoroutine(_rotationCoroutine);

            _rotationCoroutine = StartCoroutine(RotateCameraLoop());
        }

        private IEnumerator RotateCameraLoop()
        {
            while (_isCanRotate)
            {
#if UNITY_IOS || UNITY_ANDROID
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Moved)
                {
                    float mouseX = touch.deltaPosition.x;
                    float mouseY = touch.deltaPosition.y;

                    if (_isSniperMode)
                        HandleSniperRotation(mouseX, mouseY);
                    else
                        HandleFreeLookRotation(mouseX, mouseY);
                }
            }
#else
                if (Input.GetMouseButton(0))
                {
                    float mouseX = Input.GetAxis("Mouse X");
                    float mouseY = Input.GetAxis("Mouse Y");

                    if (_isSniperMode)
                        HandleSniperRotation(mouseX, mouseY);
                    else
                        HandleFreeLookRotation(mouseX, mouseY);
                }
#endif
                yield return null;
            }
        }

        private void HandleFreeLookRotation(float mouseX, float mouseY)
        {
            float sensitivity = _normalSensitivity;

            _rotationY += mouseX * sensitivity;
            _rotationX -= mouseY * sensitivity;

            _rotationX = Mathf.Clamp(_rotationX, _angleRotationMinX, _angleRotationMaxX);
            _rotationY = Mathf.Clamp(_rotationY, _angleRotationMinY, _angleRotationMaxY);

            Quaternion rotation = Quaternion.Euler(_rotationX, _rotationY, 0);
            Vector3 position = _target.position - rotation * Vector3.forward * _distanceFromTarget;

            _mainCamera.transform.SetPositionAndRotation(position, rotation);
        }

        private void HandleSniperRotation(float mouseX, float mouseY)
        {
            float sensitivity = _sniperSensitivity;

            _rotationY += mouseX * sensitivity;
            _rotationX -= mouseY * sensitivity;

            _rotationX = Mathf.Clamp(_rotationX, _angleRotationMinX, _angleRotationMaxX);
            _rotationY = Mathf.Clamp(_rotationY, _angleRotationMinY, _angleRotationMaxY);

            _mainCamera.transform.rotation = Quaternion.Euler(_rotationX, _rotationY, 0);
        }

        private void SetCameraZoom(bool isSniper)
        {
            _isSniperMode = isSniper;
            float targetFOV = isSniper ? _sniperFOV : _normalFOV;

            Vector3 targetPosition;
            Quaternion targetRotation;

            if (isSniper)
            {
                targetPosition = _target.TransformPoint(_sniperPositionOffset);
                targetRotation = _mainCamera.transform.rotation;
            }
            else
            {
                targetRotation = _mainCamera.transform.rotation;

                Vector3 direction = targetRotation * Vector3.forward;
                targetPosition = _target.position - direction * _distanceFromTarget;

                Vector3 angles = targetRotation.eulerAngles;
                _rotationX = NormalizeAngle(angles.x);
                _rotationY = NormalizeAngle(angles.y);
            }

            DOTween.Kill(_mainCamera);

            _mainCamera
                .DOFieldOfView(targetFOV, _cameraZoomMultiplier / _zoomSpeed)
                .SetEase(Ease.InOutSine)
                .SetLink(_mainCamera.gameObject);

            _mainCamera.transform
                .DOMove(targetPosition, _cameraZoomMultiplier / _zoomSpeed)
                .SetEase(Ease.InOutSine)
                .SetLink(_mainCamera.gameObject);

            _mainCamera.transform
                .DORotateQuaternion(targetRotation, _cameraZoomMultiplier / _zoomSpeed)
                .SetEase(Ease.InOutSine)
                .SetLink(_mainCamera.gameObject);
        }

        private void ResetCameraToDefault()
        {
            _rotationX = _defaultXRotation;
            _rotationY = _defaultYRotation;

            Quaternion rotation = Quaternion.Euler(_rotationX, _rotationY, 0);
            Vector3 position = _target.position - rotation * Vector3.forward * _distanceFromTarget;

            _mainCamera.transform.SetPositionAndRotation(position, rotation);
        }

        private float NormalizeAngle(float angle)
        {
            if (angle > 180f)
                angle -= 360f;

            return angle;
        }
    }
}