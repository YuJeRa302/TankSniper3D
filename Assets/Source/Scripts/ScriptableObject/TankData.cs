using Assets.Source.Scripts.Services;
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
        [SerializeField] private int _health;
        [SerializeField] private int _gunsNumber;
        [SerializeField] private string _name;
        [SerializeField] private ProjectileData _projectileData;
        [SerializeReference] private IShootingStrategy _shootingStrategy;

        public int StarCount => _starCount;
        public int Level => _level;
        public int Health => _health;
        public int GunsNumber => _gunsNumber;
        public string Name => _name;
        public TankView MainTankView => _mainTankView;
        public Sprite Icon => _icon;
        public ProjectileData ProjectileData => _projectileData;
        public IShootingStrategy ShootingStrategy => _shootingStrategy;
    }
}