using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Scripts.Grid
{
    public class GridPlacer : MonoBehaviour
    {
        [SerializeField] private GridCellView _gridCellView;
        [Space(20)]
        [SerializeField] private int _rows;
        [SerializeField] private int _columns;
        [SerializeField] private int _cellSize;

        private int _cellId = 0;
        private List<GridCellView> _gridCellViews = new();

        public List<GridCellView> GridCellViews => _gridCellViews;

        private void OnDestroy()
        {
            ClearGrid();
        }

        public void Initialize()
        {
            if (_gridCellView == null)
                return;

            for (int row = 0; row < _rows; row++)
            {
                for (int column = 0; column < _columns; column++)
                {
                    var cell = Instantiate(_gridCellView, new Vector3(
                        transform.position.x + row * _cellSize,
                        transform.position.y,
                        transform.position.y + column * _cellSize), Quaternion.identity);

                    cell.transform.SetParent(transform, false);
                    cell.Initialize(_cellId);
                    _gridCellViews.Add(cell);
                    _cellId++;
                }
            }
        }

        public GridCellView GetGridCellById(int cellId)
        {
            foreach (var gridCellView in _gridCellViews)
            {
                if (gridCellView.Id == cellId)
                    return gridCellView;
            }

            return default;
        }

        private void ClearGrid()
        {
            if (_gridCellViews.Count == 0)
                return;

            foreach (GridCellView view in _gridCellViews)
            {
                Destroy(view.gameObject);
            }

            _gridCellViews.Clear();
        }
    }
}