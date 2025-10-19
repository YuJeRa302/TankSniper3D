using Assets.Source.Game.Scripts.Enemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Scripts.Game
{
    public class SniperCrosshairView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform crosshairUI;
        [SerializeField] private RectTransform indicatorPrefab;
        [SerializeField] private Image[] dangerZones; // 0: Top, 1: Bottom, 2: Left, 3: Right
        [SerializeField] private Camera mainCamera;

        [Header("Settings")]
        [SerializeField] private float edgeOffset = 30f;

        private List<Enemy> _enemies = new();
        private List<RectTransform> _activeIndicators = new();
        private int[] _zoneEnemyCounts = new int[4];
        private Coroutine _updateRoutine;

        private void OnEnable()
        {
            if (_updateRoutine != null)
                StopCoroutine(_updateRoutine);

            _updateRoutine = StartCoroutine(UpdateCrosshairRoutine());
        }

        private void OnDisable()
        {
            if (_updateRoutine != null)
                StopCoroutine(_updateRoutine);
        }

        public void Initialize(List<Enemy> enemies)
        {
            _enemies = enemies;
        }

        private IEnumerator UpdateCrosshairRoutine()
        {
            while (_enemies.Count > 0)
            {
                ResetEnemyCounts();
                ClearIndicators();

                foreach (var enemy in _enemies)
                {
                    if (enemy == null) continue;

                    Vector3 viewportPos = mainCamera.WorldToViewportPoint(enemy.transform.position);

                    bool isBehind = viewportPos.z < 0;
                    bool isOutside = viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.y > 1;

                    if (isBehind || isOutside)
                    {
                        ShowDirectionIndicator(enemy.transform.position, enemy.transform);
                        UpdateZoneHighlight(enemy.transform.position);
                    }
                }

                UpdateZoneColors();

                yield return null;
            }
        }

        private void ResetEnemyCounts()
        {
            for (int i = 0; i < _zoneEnemyCounts.Length; i++)
                _zoneEnemyCounts[i] = 0;
        }

        private void ClearIndicators()
        {
            foreach (var ind in _activeIndicators)
                Destroy(ind.gameObject);

            _activeIndicators.Clear();
        }

        private void ShowDirectionIndicator(Vector3 enemyWorldPos, Transform enemyTransform)
        {
            RectTransform indicator = Instantiate(indicatorPrefab, crosshairUI);
            var component = indicator.gameObject.AddComponent<EnemyDirectionIndicator>();
            component.Initialize(enemyTransform, mainCamera, crosshairUI);

            _activeIndicators.Add(indicator);
        }

        private void UpdateZoneHighlight(Vector3 enemyWorldPos)
        {
            Vector3 dirToEnemy = (enemyWorldPos - mainCamera.transform.position).normalized;
            Vector3 localDir = mainCamera.transform.InverseTransformDirection(dirToEnemy);

            float x = localDir.x;
            float y = localDir.y;

            if (Mathf.Abs(x) > Mathf.Abs(y))
            {
                if (x > 0)
                    _zoneEnemyCounts[3]++; // Right
                else
                    _zoneEnemyCounts[2]++; // Left
            }
            else
            {
                if (y > 0)
                    _zoneEnemyCounts[0]++; // Top
                else
                    _zoneEnemyCounts[1]++; // Bottom
            }
        }

        private void UpdateZoneColors()
        {
            for (int i = 0; i < dangerZones.Length; i++)
            {
                if (_zoneEnemyCounts[i] > 0)
                {
                    // Красный с альфой 20 из 255
                    dangerZones[i].color = new Color(1f, 0f, 0f, 20f / 255f);
                }
                else
                {
                    // Полностью прозрачный
                    dangerZones[i].color = new Color(1f, 0f, 0f, 0f);
                }
            }
        }
    }
}