using Assets.Source.Game.Scripts.Enums;
using UnityEngine;

namespace Assets.Source.Game.Scripts.States
{
    [System.Serializable]
    public class DecorationState
    {
        [SerializeField] private int _id;
        [SerializeField] private bool _isBuyed;
        [SerializeField] private TypeCard _typeCard;

        public DecorationState(int id, bool isBuyed, TypeCard typeCard)
        {
            _id = id;
            _isBuyed = isBuyed;
            _typeCard = typeCard;
        }

        public int Id => _id;
        public bool IsBuyed => _isBuyed;
        public TypeCard TypeCard => _typeCard;

        public void ChangeBuyState(bool isBuyed)
        {
            _isBuyed = isBuyed;
        }
    }
}