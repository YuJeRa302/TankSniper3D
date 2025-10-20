using System.Collections;
using UnityEngine;

namespace Assets.Source.Scripts.Game
{
    public class EnemyDirectionIndicator : MonoBehaviour
    {
        private readonly float _screenCenterValue = 0.5f;
        private readonly float _angleRotation = 90f;

        [SerializeField] private float _edgeOffset = 30f;
        [SerializeField] private float _pulseSpeed = 4f;
        [SerializeField] private float _pulseAmplitude = 5f;

        private Transform _enemy;
        private Camera _camera;
        private RectTransform _rectTransform;
        private RectTransform _crosshair;
        private Coroutine _followCoroutine;

        public void Initialize(Transform enemy, Camera camera, RectTransform crosshair)
        {
            _enemy = enemy;
            _camera = camera;
            _crosshair = crosshair;
            _rectTransform = GetComponent<RectTransform>();
            _followCoroutine = StartCoroutine(FollowTarget());
        }

        private IEnumerator FollowTarget()
        {
            while (_enemy != null)
            {
                Vector3 viewportPos = _camera.WorldToViewportPoint(_enemy.position);

                if (viewportPos.z > 0 && viewportPos.x >= 0 && viewportPos.x <= 1 && viewportPos.y >= 0 && viewportPos.y <= 1)
                {
                    gameObject.SetActive(false);
                }
                else
                {
                    gameObject.SetActive(true);

                    Vector2 screenCenter = new(_screenCenterValue, _screenCenterValue);
                    Vector2 dir = ((Vector2)viewportPos - screenCenter).normalized;
                    float pulse = Mathf.Sin(Time.time * _pulseSpeed) * _pulseAmplitude;
                    float radius = (_crosshair.sizeDelta.x / 2f) - _edgeOffset + pulse;
                    Vector2 finalPos = dir * radius;
                    _rectTransform.anchoredPosition = finalPos;
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    _rectTransform.rotation = Quaternion.Euler(0, 0, angle - _angleRotation);
                }

                yield return null;
            }

            Destroy(gameObject);
        }
    }
}