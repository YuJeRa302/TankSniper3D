using Assets.Source.Scripts.Grid;
using UnityEngine;

namespace Assets.Source.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New GridTank", menuName = "Create GridTank", order = 51)]
    public class GridTankData : ScriptableObject
    {
        [SerializeField] private int _id;
        [SerializeField] private int _level;
        [SerializeField] private GridTankView _gridTankView;

        public int Id => _id;
        public int Level => _level;
        public GridTankView TankView => _gridTankView;
    }
}