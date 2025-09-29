using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Source.Scripts.Upgrades
{
    public class PlacePreviewView : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        private readonly float _decalRotationViewValue = 130f;
        private readonly float _heroRotationViewValue = -51f;
        private readonly float _heroContainerRotationValue = 51f;
        private readonly float _controlValue = 0.1f;

        [SerializeField] private float _defaultRotationValue;
        [SerializeField] private float _rotationSpeed = 0.5f;
        [SerializeField] private float _returnSpeed = 5f;
        [Space(20)]
        [SerializeField] private GameObject _heroContainer;

        private Coroutine _rotationCoroutine;
        private float _currentRotationY;
        private bool _isDragging = false;
        private bool _isCanRotation = true;

        private void Start()
        {
            _currentRotationY = _defaultRotationValue;
            _rotationCoroutine = StartCoroutine(ReturnToDefaultRotation(_defaultRotationValue));
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isDragging = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_isCanRotation)
                return;

            if (!_isDragging)
                return;

            _currentRotationY += eventData.delta.x * _rotationSpeed;
            transform.rotation = Quaternion.Euler(0, _currentRotationY, 0);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isDragging = false;

            if (!_isCanRotation)
                return;

            if (_rotationCoroutine != null)
                StopCoroutine(_rotationCoroutine);

            _rotationCoroutine = StartCoroutine(ReturnToDefaultRotation(_defaultRotationValue));
        }

        public void ResetRotation()
        {
            _isCanRotation = true;

            if (_rotationCoroutine != null)
                StopCoroutine(_rotationCoroutine);

            _rotationCoroutine = StartCoroutine(ReturnToDefaultRotation(_defaultRotationValue));
        }

        public void SetDecalRotationView()
        {
            _isCanRotation = false;

            if (_rotationCoroutine != null)
                StopCoroutine(_rotationCoroutine);

            _rotationCoroutine = StartCoroutine(ReturnToDefaultRotation(_decalRotationViewValue));
        }

        public void SetHeroRotationView()
        {
            _isCanRotation = false;
            _heroContainer.transform.rotation = Quaternion.Euler(0, _heroContainerRotationValue, 0);

            if (_rotationCoroutine != null)
                StopCoroutine(_rotationCoroutine);

            _rotationCoroutine = StartCoroutine(ReturnToDefaultRotation(_heroRotationViewValue));
        }

        private IEnumerator ReturnToDefaultRotation(float targetRotationValue)
        {
            while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetRotationValue)) > _controlValue)
            {
                float rotationValue = Mathf.LerpAngle(transform.eulerAngles.y, targetRotationValue, Time.deltaTime * _returnSpeed);
                transform.rotation = Quaternion.Euler(0, rotationValue, 0);
                yield return null;
            }

            transform.rotation = Quaternion.Euler(0, targetRotationValue, 0);
            _currentRotationY = targetRotationValue;
        }
    }
}