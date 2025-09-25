namespace Assets.Source.Scripts.Upgrades
{
    public struct M_DeselectButtons
    {
        private readonly SelectionButtonView _selectionButtonView;

        public M_DeselectButtons(SelectionButtonView selectionButtonView)
        {
            _selectionButtonView = selectionButtonView;
        }

        public readonly SelectionButtonView SelectionButtonView => _selectionButtonView;
    }
}