using Assets.Source.Game.Scripts.States;
using Assets.Source.Scripts.ScriptableObjects;
using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Source.Scripts.Grid
{
    public class GridTankView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        private readonly float _tweenAnimationDuration = 1f;
        private readonly float _tweenAnimationScaler = 1.2f;
        private readonly float _delay = 0.25f;
        private readonly float _timeToMove = 0.5f;

        [SerializeField] private GridTankLevelView _itemLevelView;

        private float _elapsedTime = 0f;
        private bool _isHolding = false;
        private GridCellView _targetCell = null;
        private GridTankState _gridTankState;
        private Coroutine _moveCoroutine;
        private Coroutine _coroutineTankAnimation;

        public int Level { get; private set; }
        public GridCellView OriginalCell { get; private set; }
        public GridTankState GridTankState => _gridTankState;

        public void Initialize(GridTankData gridTankData, GridTankState gridTankState, bool isCreateByLoad)
        {
            _gridTankState = gridTankState;
            Level = gridTankData.Level;
            _itemLevelView.SetLevelValue(Level);

            if (isCreateByLoad == false)
                AnimateGridTank();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isHolding = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_isHolding)
                return;

            transform.position = CalculateWorldPosition(this, eventData);
            SetTargetCell();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isHolding = false;
            MoveTankToCell();
        }

        public void ChangeOriginalCell(GridCellView originalCell)
        {
            OriginalCell = originalCell;
            _gridTankState.ChangeOriginalCellId(OriginalCell.Id);
        }

        private void DeselectTargetCell()
        {
            if (_targetCell != null)
                _targetCell.Deselect();
        }

        private void AnimateGridTank()
        {
            if (_coroutineTankAnimation != null)
                StopCoroutine(_coroutineTankAnimation);

            _coroutineTankAnimation = StartCoroutine(SetAnimation());
        }

        private void StartMoveCoroutine(GridTankView gridItem, Vector3 targetPosition)
        {
            if (_moveCoroutine != null)
                StopCoroutine(_moveCoroutine);

            _moveCoroutine = StartCoroutine(MoveObjectToPosition(gridItem, targetPosition));
        }

        private void MovetToTargetCellSmoothly(GridCellView targetCell)
        {
            Vector3 targetPosition = targetCell.transform.position;
            targetPosition.y = transform.position.y;
            StartMoveCoroutine(this, targetPosition);
            transform.position = targetPosition;
            OriginalCell.SetFree();
            targetCell.SetOccupied(this);
            targetCell.Deselect();
            OriginalCell = targetCell;
            ChangeOriginalCell(targetCell);
        }

        private void MoveToOriginalCell(GridTankView gridItem)
        {
            StartMoveCoroutine(gridItem, new Vector3(
                gridItem.OriginalCell.transform.position.x,
                gridItem.transform.position.y,
                gridItem.OriginalCell.transform.position.z));

            gridItem.OriginalCell.SetOccupied(gridItem);
            DeselectTargetCell();
        }

        private IEnumerator MoveObjectToPosition(GridTankView gridItem, Vector3 targetPosition)
        {
            Vector3 startPosition = gridItem.transform.position;
            _elapsedTime = 0f;

            while (_elapsedTime < _timeToMove)
            {
                gridItem.transform.position = Vector3.Lerp(startPosition, targetPosition, _elapsedTime / _timeToMove);
                _elapsedTime += Time.deltaTime;
                yield return null;
            }

            gridItem.transform.position = targetPosition;
        }

        private void SetTargetCell()
        {
            if (_targetCell != FindTargetCell(this))
                DeselectTargetCell();

            _targetCell = FindTargetCell(this);

            if (_targetCell != null)
                _targetCell.Select(this);
        }

        private void MoveTankToCell()
        {
            if (_targetCell != null)
            {
                if (_targetCell.IsOccupied)
                {
                    if (!_targetCell.TryToMerge(this))
                        MoveToOriginalCell(this);
                }
                else
                {
                    MovetToTargetCellSmoothly(_targetCell);
                }
            }
            else
            {
                MoveToOriginalCell(this);
            }
        }

        private GridCellView FindTargetCell(GridTankView item)
        {
            Vector3 origin = item.transform.position;
            Ray ray = new(origin, Vector3.down);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.TryGetComponent(out GridCellView cell))
                    return cell;
            }

            return null;
        }

        private Vector3 CalculateWorldPosition(GridTankView gridItem, PointerEventData eventData)
        {
            float distanceToPlane = Camera.main.WorldToScreenPoint(transform.position).z;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, distanceToPlane));
            worldPosition.y = gridItem.transform.position.y;
            return worldPosition;
        }

        private IEnumerator SetAnimation()
        {
            float endValue = gameObject.transform.localScale.x;
            gameObject.transform.localScale *= _tweenAnimationScaler;

            gameObject.transform.DOScale(endValue, _tweenAnimationDuration)
                .SetEase(Ease.OutBounce)
                .SetLink(gameObject);

            yield return new WaitForSeconds(_delay);
        }
    }
}