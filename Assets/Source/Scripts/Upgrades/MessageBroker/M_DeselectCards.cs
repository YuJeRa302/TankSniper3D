namespace Assets.Source.Scripts.Upgrades
{
    public struct M_DeselectCards
    {
        private readonly int _id;

        public M_DeselectCards(int id)
        {
            _id = id;
        }

        public readonly int Id => _id;
    }
}