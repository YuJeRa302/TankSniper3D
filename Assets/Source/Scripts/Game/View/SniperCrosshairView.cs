using Assets.Source.Game.Scripts.Enemy;
using Assets.Source.Game.Scripts.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Scripts.Game
{
    public class SniperCrosshairView : MonoBehaviour
    {
        private readonly Color _selectZone = new(1f, 0f, 0f, 20f / 255f);
        private readonly Color _deselectZone = new(1f, 0f, 0f, 0f);

        [Header("References")]
        [SerializeField] private RectTransform _crosshairBorder;
        [SerializeField] private EnemyDirectionIndicator _indicatorPrefab;
        [SerializeField] private Image[] _dangerZones;
        [Header("Settings")]
        [SerializeField] private float _edgeOffset = 30f;

        private Camera _mainCamera;
        private List<Enemy> _enemies = new();
        private List<EnemyDirectionIndicator> _activeIndicators = new();
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
            _mainCamera = Camera.main;
        }

        private IEnumerator UpdateCrosshairRoutine()
        {
            while (_enemies.Count > 0)
            {
                ResetEnemyCounts();
                ClearIndicators();

                foreach (var enemy in _enemies)
                {
                    if (enemy == null)
                        continue;

                    Vector3 viewportPos = _mainCamera.WorldToViewportPoint(enemy.transform.position);

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
            var indicator = Instantiate(_indicatorPrefab, _crosshairBorder);
            indicator.Initialize(enemyTransform, _mainCamera, _crosshairBorder);
            _activeIndicators.Add(indicator);
        }

        private void UpdateZoneHighlight(Vector3 enemyWorldPos)
        {
            Vector3 dirToEnemy = (enemyWorldPos - _mainCamera.transform.position).normalized;
            Vector3 localDir = _mainCamera.transform.InverseTransformDirection(dirToEnemy);

            float x = localDir.x;
            float y = localDir.y;

            if (Mathf.Abs(x) > Mathf.Abs(y))
            {
                if (x > 0)
                    _zoneEnemyCounts[(int)IndexDangerZone.BottomRight]++;
                else
                    _zoneEnemyCounts[(int)IndexDangerZone.BottomLeft]++;
            }
            else
            {
                if (y > 0)
                    _zoneEnemyCounts[(int)IndexDangerZone.TopLeft]++;
                else
                    _zoneEnemyCounts[(int)IndexDangerZone.TopRight]++;
            }
        }

        private void UpdateZoneColors()
        {
            for (int i = 0; i < _dangerZones.Length; i++)
            {
                if (_zoneEnemyCounts[i] > 0)
                    _dangerZones[i].color = _selectZone;
                else
                    _dangerZones[i].color = _deselectZone;
            }
        }
    }
}