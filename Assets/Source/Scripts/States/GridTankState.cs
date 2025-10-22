using UnityEngine;

namespace Assets.Source.Game.Scripts.States
{
    [System.Serializable]
    public class GridTankState
    {
        [SerializeField] private int _level;
        [SerializeField] private int _gridCellId;

        public GridTankState(int level, int gridCellId)
        {
            _level = level;
            _gridCellId = gridCellId;
        }

        public int Level => _level;
        public int GridCellId => _gridCellId;

        public void ChangeOriginalCellId(int gridCellId)
        {
            _gridCellId = gridCellId;
        }
    }
}