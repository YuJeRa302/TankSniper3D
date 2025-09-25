using UnityEngine;

namespace Assets.Source.Scripts.Grid
{
    public struct M_ItemMerged
    {
        private readonly int _currentLevel;
        private readonly GridCellView _gridCellView;

        public M_ItemMerged(int currentLevel, GridCellView gridCellView)
        {
            _currentLevel = currentLevel;
            _gridCellView = gridCellView;
        }

        public readonly int CurrentLevel => _currentLevel;
        public readonly GridCellView GridCellView => _gridCellView;
    }
}