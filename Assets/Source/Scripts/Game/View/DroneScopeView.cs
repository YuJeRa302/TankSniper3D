using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Source.Scripts.Game
{
    public class DroneScopeView : MonoBehaviour
    {
        public Camera freeLookCamera;
        public Camera droneCamera;
        public GameObject drone;
        public float droneSpeed = 10f;
        public float rotationSpeed = 5f;
        public GameObject aimingButton; // кнопка прицеливания
        public GameObject interferenceEffect; // эффект помех
        public float maxInterferenceDistance = 100f;
        public DroneInterferenceEffect interferenceEffectScript;

        private bool isAiming = false;
        private Vector2 _dragInput;
        private Vector3 _initialDronePosition;
        private Quaternion _initialDroneRotation;

        private Button _sniperScopeButton;

        private void OnTriggerEnter(Collider other)
        {
            if (isAiming)
            {
                if (other.CompareTag("Building") || other.CompareTag("Enemy"))
                {
                    ExplodeDrone();
                }
            }
        }

        public void Initialize(Button sniperScopeButton, Camera mainCamera)
        {
            gameObject.SetActive(false);
            _sniperScopeButton = sniperScopeButton;
            freeLookCamera = mainCamera;
            droneCamera.enabled = false;
            drone.SetActive(false);
            interferenceEffect.SetActive(false);
        }

        public void OnAimButtonPressed()
        {
            isAiming = true;
            freeLookCamera.enabled = false;
            droneCamera.enabled = true;
            drone.SetActive(true);

            // Запоминаем позицию и поворот дрона, если нужно
            _initialDronePosition = drone.transform.position;
            _initialDroneRotation = drone.transform.rotation;

            interferenceEffect.SetActive(true);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (isAiming)
                _dragInput = Vector2.zero;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (isAiming)
                _dragInput = eventData.delta;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _dragInput = Vector2.zero;
        }

        private void Update()
        {
            if (isAiming)
            {
                // Автоматический полёт вперёд
                drone.transform.position += drone.transform.forward * droneSpeed * Time.deltaTime;

                // Управление дроном с помощью dragInput
                float yaw = _dragInput.x * rotationSpeed * Time.deltaTime;
                float pitch = -_dragInput.y * rotationSpeed * Time.deltaTime;

                drone.transform.Rotate(pitch, yaw, 0);

                Vector3 euler = drone.transform.rotation.eulerAngles;
                euler.z = 0;
                drone.transform.rotation = Quaternion.Euler(euler);

                float distance = Vector3.Distance(_initialDronePosition, drone.transform.position);
                float interferenceAmount = Mathf.Clamp01(distance / maxInterferenceDistance);

                // Обновляем эффект помех через скрипт
                interferenceEffectScript.UpdateEffect(interferenceAmount);
            }
        }

        private void ExplodeDrone()
        {
            isAiming = false;
            interferenceEffectScript.ResetEffect();
            drone.SetActive(false);
            freeLookCamera.enabled = true;
            droneCamera.enabled = false;

            Debug.Log("Drone exploded!");
            // Можно добавить визуальные эффекты взрыва здесь
        }
    }
}