using UnityEngine;

namespace Assets.Source.Game.Scripts.States
{
    [System.Serializable]
    public class HeroState
    {
        [SerializeField] private int _id;
        [SerializeField] private bool _isBuyed;
        [SerializeField] private bool _isEquipped;

        public HeroState(int id, bool isBuyed, bool isEquipped)
        {
            _id = id;
            _isBuyed = isBuyed;
            _isEquipped = isEquipped;
        }

        public int Id => _id;
        public bool IsBuyed => _isBuyed;
        public bool IsEquipped => _isEquipped;

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