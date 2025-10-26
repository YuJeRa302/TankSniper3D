using UnityEngine;

namespace Assets.Source.Game.Scripts.States
{
    [System.Serializable]
    public class HeroState
    {
        [SerializeField] private int _id;
        [SerializeField] private bool _isBuyed;
        [SerializeField] private bool _isOpened;

        public HeroState(int id, bool isBuyed, bool isOpened)
        {
            _id = id;
            _isBuyed = isBuyed;
            _isOpened = isOpened;
        }

        public int Id => _id;
        public bool IsBuyed => _isBuyed;
        public bool IsOpened => _isOpened;

        public void SetOpenedState()
        {
            _isOpened = true;
        }

        public void ChangeBuyState(bool isBuyed)
        {
            _isBuyed = isBuyed;
        }
    }
}