using Assets.Source.Game.Scripts.Enemy;
using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Scripts.Upgrades;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Scripts.Game
{
    public class SniperCrosshairView : MonoBehaviour
    {
        private readonly float _resetHighlightDelay = 0.3f;
        private readonly Color _selectZone = new(1f, 0f, 0f, 130f / 255f);
        private readonly Color _deselectZone = new(1f, 0f, 0f, 0f);

        [SerializeField] private float _maxDistance = 500f;
        [SerializeField] private RectTransform _crosshairBorder;
        [SerializeField] private EnemyDirectionIndicator _indicatorPrefab;
        [SerializeField] private Image[] _dangerZones;
        [SerializeField] private Image[] _superShootZones;

        private Enemy _currentTargetEnemy;
        private DamageableArea _currentTargetArea;
        private Camera _mainCamera;
        private List<Enemy> _enemies = new();
        private List<EnemyDirectionIndicator> _activeIndicators = new();
        private Coroutine _updateRoutine;
        private Coroutine _highlightCoroutine;
        private CompositeDisposable _disposables = new();
        private int[] _zoneEnemyCounts = new int[4];
        private bool _isPlayerShoot = false;

        public Enemy GetCurrentTargetEnemy() => _currentTargetEnemy;
        public DamageableArea GetTargetedDamageableArea() => _currentTargetArea;

        private void OnEnable()
        {
            if (_isPlayerShoot != true)
                return;

            if (_updateRoutine != null)
                StopCoroutine(_updateRoutine);

            _updateRoutine = StartCoroutine(UpdateCrosshairRoutine());
        }

        private void OnDisable()
        {
            if (_updateRoutine != null)
                StopCoroutine(_updateRoutine);

            if (_highlightCoroutine != null)
                StopCoroutine(_highlightCoroutine);

            ResetZoneColors();
        }

        private void OnDestroy()
        {
            _disposables?.Dispose();
        }

        private void Update()
        {
            if (_mainCamera == null)
                return;

            Ray ray = _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

            if (Physics.Raycast(ray, out RaycastHit hit, _maxDistance))
            {
                if (hit.collider.TryGetComponent(out DamageableArea damageableArea))
                {
                    _currentTargetArea = damageableArea;
                    _currentTargetEnemy = damageableArea.GetComponentInParent<Enemy>();
                    return;
                }
            }

            _currentTargetEnemy = null;
            _currentTargetArea = null;
        }

        public void Initialize(List<Enemy> enemies)
        {
            _enemies = enemies;
            _mainCamera = Camera.main;

            TankHealth.Message
                .Receive<M_PlayerHitFromDirection>()
                .Subscribe(m => OnPlayerHit(m.HitDirection))
                .AddTo(_disposables);
        }

        public void SetPlayerShoot()
        {
            _isPlayerShoot = true;
        }

        public void SetActiveShooterZones(bool state)
        {
            foreach (var zone in _superShootZones)
                zone.gameObject.SetActive(state);
        }

        private void OnPlayerHit(Vector3 direction)
        {
            if (!gameObject.activeSelf)
                return;

            UpdateZoneHighlight(direction);

            if (_highlightCoroutine != null)
                StopCoroutine(_highlightCoroutine);

            _highlightCoroutine = StartCoroutine(ResetHighlightAfterDelay());
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
                        ShowDirectionIndicator(enemy.transform);
                }

                yield return null;
            }
        }

        private IEnumerator ResetHighlightAfterDelay()
        {
            yield return new WaitForSeconds(_resetHighlightDelay);
            ResetZoneColors();
        }

        private void UpdateZoneHighlight(Vector3 firePoint)
        {
            var dirToHit = (_mainCamera.transform.position - firePoint).normalized;
            Vector3 localDir = _mainCamera.transform.InverseTransformDirection(dirToHit);

            float x = localDir.x;
            float y = localDir.y;

            ResetZoneColors();

            if (Mathf.Abs(x) > Mathf.Abs(y))
            {
                if (x > 0)
                    _dangerZones[(int)IndexDangerZone.BottomRight].color = _selectZone;
                else
                    _dangerZones[(int)IndexDangerZone.BottomLeft].color = _selectZone;
            }
            else
            {
                if (y > 0)
                    _dangerZones[(int)IndexDangerZone.TopLeft].color = _selectZone;
                else
                    _dangerZones[(int)IndexDangerZone.TopRight].color = _selectZone;
            }
        }

        private void ResetZoneColors()
        {
            foreach (var zone in _dangerZones)
                zone.color = _deselectZone;
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

        private void ShowDirectionIndicator(Transform enemyTransform)
        {
            var indicator = Instantiate(_indicatorPrefab, _crosshairBorder);
            indicator.Initialize(enemyTransform, _mainCamera, _crosshairBorder);
            _activeIndicators.Add(indicator);
        }
    }
}