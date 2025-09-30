using UnityEngine;

namespace Assets.Source.Game.Scripts.States
{
    [System.Serializable]
    public class LevelState
    {
        [SerializeField] private int _id;
        [SerializeField] private int _biomId;
        [SerializeField] private bool _isCurrent;
        [SerializeField] private bool _isComplete;

        public LevelState(int id, int biomId, bool isCurrent, bool isComplete)
        {
            _id = id;
            _biomId = biomId;
            _isCurrent = isCurrent;
            _isComplete = isComplete;
        }

        public int Id => _id;
        public bool IsCurrent => _isCurrent;
        public bool IsComplete => _isComplete;
        public int BiomId => _biomId;

        public void SetCurrent(bool isCurrent)
        {
            _isCurrent = isCurrent;
        }

        public void SetComplete(bool isComplete)
        {
            _isComplete = isComplete;
        }
    }
}