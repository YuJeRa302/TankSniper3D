using Assets.Source.Game.Scripts.States;

namespace Assets.Source.Scripts.Grid
{
    public struct M_ItemMerged
    {
        private readonly int _currentLevel;
        private readonly GridCellView _gridCellView;
        private readonly GridTankState _firstMergedTank;
        private readonly GridTankState _secondMergedTank;

        public M_ItemMerged(
            int currentLevel,
            GridCellView gridCellView,
            GridTankState firstMergedTank,
            GridTankState secondMergedTank)
        {
            _currentLevel = currentLevel;
            _gridCellView = gridCellView;
            _firstMergedTank = firstMergedTank;
            _secondMergedTank = secondMergedTank;
        }

        public readonly int CurrentLevel => _currentLevel;
        public readonly GridCellView GridCellView => _gridCellView;
        public readonly GridTankState FirstMergedTank => _firstMergedTank;
        public readonly GridTankState SecondMergedTank => _secondMergedTank;
    }
}