using Assets.Source.Game.Scripts.States;
using Assets.Source.Scripts.ScriptableObjects;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Source.Scripts.Grid
{
    public class GridTankView : MonoBehaviour, IPointerDownHandler, IPointerMoveHandler, IPointerUpHandler
    {
        private readonly float _timeToMove = 0.5f;

        [SerializeField] private GridTankLevelView _itemLevelView;

        private float _elapsedTime = 0f;
        private bool _isHolding = false;
        private GridCellView _targetCell = null;
        private GridTankState _gridTankState;
        private GridTankData _gridItemData;

        public int Level { get; private set; }
        public GridCellView OriginalCell { get; private set; }
        public GridTankState GridTankState => _gridTankState;
        public GridTankData GridItemData => _gridItemData;

        public void Initialize(GridTankData gridTankData, GridTankState gridTankState)
        {
            _gridItemData = gridTankData;
            _gridTankState = gridTankState;
            Level = gridTankData.Level;
            _itemLevelView.SetLevelValue(Level);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isHolding = true;
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            if (_isHolding)
            {
                transform.position = CalculateWorldPosition(this, eventData);

                if (_targetCell != FindTargetCell(this))
                    DeselectTargetCell();

                _targetCell = FindTargetCell(this);

                if (_targetCell != null)
                    _targetCell.Select(this);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isHolding = false;

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

        public void ChangeOriginalCell(GridCellView originalCell)
        {
            OriginalCell = originalCell;
            _gridTankState.ChangeOriginalCell(OriginalCell.transform.position);
        }

        private void DeselectTargetCell()
        {
            if (_targetCell != null)
                _targetCell.Deselect();
        }

        private void MovetToTargetCellSmoothly(GridCellView targetCell)
        {
            Vector3 targetPosition = targetCell.transform.position;
            targetPosition.y = transform.position.y;
            StartCoroutine(MoveObjectToPosition(this, targetPosition));
            transform.position = targetPosition;
            OriginalCell.SetFree();
            targetCell.SetOccupied(this);
            targetCell.Deselect();
            OriginalCell = targetCell;
            ChangeOriginalCell(targetCell);
        }

        private void MoveToOriginalCell(GridTankView gridItem)
        {
            StartCoroutine(MoveObjectToPosition(gridItem, new Vector3(
                gridItem.OriginalCell.transform.position.x,
                gridItem.transform.position.y,
                gridItem.OriginalCell.transform.position.z)));

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
    }
}