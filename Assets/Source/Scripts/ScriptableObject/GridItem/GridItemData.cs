using Assets.Source.Scripts.Grid;
using UnityEngine;

namespace Assets.Source.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New GridItem", menuName = "Create GridItem", order = 51)]
    public class GridItemData : ObjectData
    {
        [SerializeField] private int _level;
        [SerializeField] private ObjectView _itemView;

        public int Level => _level;
        public ObjectView TankView => _itemView;
    }
}