using Assets.Source.Scripts.Upgrades;
using UnityEngine;

namespace Assets.Source.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Tank", menuName = "Create Tank", order = 51)]
    public class TankData : ObjectData
    {
        [SerializeField] private TankView _mainTankView;
        [SerializeField] private Sprite _icon;
        [SerializeField] private int _starCount;
        [SerializeField] private int _level;

        public int StarCount => _starCount;
        public int Level => _level;
        public TankView MainTankView => _mainTankView;
        public Sprite Icon => _icon;
    }
}