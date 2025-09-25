namespace Assets.Source.Scripts.Grid
{
    public struct M_CurrentCellChanged
    {
        private readonly GridCellView _currentCell;

        public M_CurrentCellChanged(GridCellView gridCellView)
        {
            _currentCell = gridCellView;
        }

        public readonly GridCellView CurrentCell => _currentCell;
    }
}