using Assets.Source.Game.Scripts.Enums;

namespace Assets.Source.Scripts.Upgrades
{
    public struct M_UpgradeUnlocked
    {
        private readonly int _id;
        private readonly TypeCard _typeCard;

        public M_UpgradeUnlocked(int id, TypeCard typeCard)
        {
            _id = id;
            _typeCard = typeCard;
        }

        public readonly int Id => _id;
        public readonly TypeCard TypeCard => _typeCard;
    }
}