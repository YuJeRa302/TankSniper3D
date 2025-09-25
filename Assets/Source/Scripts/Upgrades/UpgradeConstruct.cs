using Assets.Source.Scripts.Grid;
using Assets.Source.Scripts.Models;
using Assets.Source.Scripts.ScriptableObjects;
using Assets.Source.Scripts.Services;
using UnityEngine;

namespace Assets.Source.Scripts.Upgrades
{
    public class UpgradeConstruct : MonoBehaviour
    {
        [SerializeField] private GridItemConfig _gridItemConfig;
        [SerializeField] private UpgradeConfig _upgradeConfig;
        [Space(20)]
        [SerializeField] private GridPlacer _gridPlacer;
        [SerializeField] private GridView _gridView;
        [SerializeField] private UpgradeView _upgradeView;

        private PersistentDataService _persistentDataService;
        private GridModel _gridModel;
        private UpgradeModel _upgradeModel;

        private void Awake()
        {
            Construct();
        }

        private void Construct()
        {
            _persistentDataService = new PersistentDataService();
            _upgradeModel = new UpgradeModel(_persistentDataService);
            _gridModel = new GridModel(_persistentDataService);

            _gridView.Initialize(_gridModel, _gridItemConfig, _gridPlacer);
            _upgradeView.Initialize(_upgradeModel, _upgradeConfig);
        }
    }
}