using UnityEngine;

namespace Assets.Source.Game.Scripts.States
{
    [System.Serializable]
    public class GridTankState
    {
        [SerializeField] private int _id;
        [SerializeField] private Vector3 _originalCell = new ();

        public GridTankState(int id, Vector3 originalCell)
        {
            _id = id;
            _originalCell = originalCell;
        }

        public int Id => _id;
        public Vector3 OriginalCell => _originalCell;

        public void ChangeOriginalCell(Vector3 originalCell)
        {
            _originalCell = originalCell;
        }
    }
}