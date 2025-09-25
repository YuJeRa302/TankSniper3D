namespace Assets.Source.Scripts.Grid
{
    public struct M_OriginalCellChanged
    {
        private readonly GridCellView _originalCell;

        public M_OriginalCellChanged(GridCellView gridCellView)
        {
            _originalCell = gridCellView;
        }

        public readonly GridCellView OriginalCell => _originalCell;
    }
}