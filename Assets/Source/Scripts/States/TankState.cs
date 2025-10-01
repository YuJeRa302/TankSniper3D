using Assets.Source.Game.Scripts.Enums;
using UnityEngine;

namespace Assets.Source.Game.Scripts.States
{
    [System.Serializable]
    public class TankState
    {
        [SerializeField] private int _id;
        [SerializeField] private bool _isOpened;
        [SerializeField] private bool _isEquipped;
        [SerializeField] private int _decalId;
        [SerializeField] private int _patternId;
        [SerializeField] private int _heroId;

        public TankState(int id, bool isOpened, bool isEquipped, int decalId, int patternId, int heroId)
        {
            _id = id;
            _isOpened = isOpened;
            _isEquipped = isEquipped;
            _decalId = decalId;
            _patternId = patternId;
            _heroId = heroId;
        }

        public int Id => _id;
        public bool IsOpened => _isOpened;
        public bool IsEquipped => _isEquipped;
        public int DecalId => _decalId;
        public int PatternId => _patternId;
        public int HeroId => _heroId;

        public void ChangeOpenState(bool isOpened)
        {
            _isOpened = isOpened;
        }

        public void ChangeEquippedState(bool isEquipped)
        {
            _isEquipped = isEquipped;
        }

        public void ChangeHero(int heroId)
        {
            _heroId = heroId;
        }

        public void ChangeDecoration(DecorationState decorationState)
        {
            if (decorationState.TypeCard == TypeCard.Decal)
                ChangeDecal(decorationState.Id);
            else
                ChangePattern(decorationState.Id);
        }

        public bool TryEquipDecoration(DecorationState decorationState)
        {
            if (decorationState.TypeCard == TypeCard.Decal)
            {
                if (_decalId == decorationState.Id)
                    return true;
            }
            else
            {
                if (_patternId == decorationState.Id)
                    return true;
            }

            return false;
        }

        private void ChangeDecal(int decalId)
        {
            _decalId = decalId;
        }

        private void ChangePattern(int patternId)
        {
            _patternId = patternId;
        }
    }
}