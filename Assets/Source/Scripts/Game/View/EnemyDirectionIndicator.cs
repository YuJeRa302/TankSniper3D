using System.Collections;
using UnityEngine;

namespace Assets.Source.Scripts.Game
{
    public class EnemyDirectionIndicator : MonoBehaviour
    {
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

                // Враг в поле зрения — скрыть индикатор
                if (viewportPos.z > 0 && viewportPos.x >= 0 && viewportPos.x <= 1 && viewportPos.y >= 0 && viewportPos.y <= 1)
                {
                    gameObject.SetActive(false);
                }
                else
                {
                    gameObject.SetActive(true);

                    Vector2 screenCenter = new Vector2(0.5f, 0.5f);
                    Vector2 dir = ((Vector2)viewportPos - screenCenter).normalized;

                    // Пульсация
                    float pulse = Mathf.Sin(Time.time * _pulseSpeed) * _pulseAmplitude;

                    // Радиус с учётом отступа
                    float radius = (_crosshair.sizeDelta.x / 2f) - _edgeOffset + pulse;

                    Vector2 finalPos = dir * radius;

                    // Применить позицию в UI
                    _rectTransform.anchoredPosition = finalPos;

                    // Также можно вращать стрелку по направлению
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    _rectTransform.rotation = Quaternion.Euler(0, 0, angle - 90f); // чтобы стрелка смотрела в нужную сторону
                }

                yield return null; // каждый кадр
            }

            // Уничтожаем объект, если враг исчез
            Destroy(gameObject);
        }
    }
}