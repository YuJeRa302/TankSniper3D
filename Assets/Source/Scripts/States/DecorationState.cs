using Assets.Source.Game.Scripts.Enums;
using UnityEngine;

namespace Assets.Source.Game.Scripts.States
{
    [System.Serializable]
    public class DecorationState
    {
        [SerializeField] private int _id;
        [SerializeField] private bool _isBuyed;
        [SerializeField] private bool _isEquipped;
        [SerializeField] private TypeCard _typeCard;

        public DecorationState(int id, bool isBuyed, bool isEquipped, TypeCard typeCard)
        {
            _id = id;
            _isBuyed = isBuyed;
            _isEquipped = isEquipped;
            _typeCard = typeCard;
        }

        public int Id => _id;
        public bool IsBuyed => _isBuyed;
        public bool IsEquipped => _isEquipped;
        public TypeCard TypeCard => _typeCard;

        public void ChangeBuyState(bool isBuyed)
        {
            _isBuyed = isBuyed;
        }

        public void ChangeEquippedState(bool isEquipped)
        {
            _isEquipped = isEquipped;
        }
    }
}