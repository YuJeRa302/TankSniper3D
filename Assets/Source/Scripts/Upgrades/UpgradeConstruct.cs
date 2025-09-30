using Assets.Source.Scripts.Grid;
using Assets.Source.Scripts.Models;
using Assets.Source.Scripts.Saves;
using Assets.Source.Scripts.ScriptableObjects;
using Assets.Source.Scripts.Services;
using UnityEngine;
using YG;

namespace Assets.Source.Scripts.Upgrades
{
    public class UpgradeConstruct : MonoBehaviour
    {
        [SerializeField] private ConfigData _configData;
        [SerializeField] private GridConfig _gridItemConfig;
        [SerializeField] private UpgradeConfig _upgradeConfig;
        [Space(20)]
        [SerializeField] private GridPlacer _gridPlacer;
        [SerializeField] private GridView _gridView;
        [SerializeField] private UpgradeView _upgradeView;

        private SaveAndLoader _saveAndLoader;
        private PersistentDataService _persistentDataService;
        private GridModel _gridModel;
        private UpgradeModel _upgradeModel;

        private void Awake()
        {
            Init();

            if (YG2.isGameplaying != true)
                InitYandexGameEntities();
        }

        private void InitYandexGameEntities()
        {
            YG2.GameReadyAPI();
            YG2.GameplayStart();
        }

        private void Init()
        {
            LoadData();
            Construct();
        }

        private void Construct()
        {
            _upgradeModel = new UpgradeModel(_persistentDataService);
            _gridModel = new GridModel(_persistentDataService);
            _gridView.Initialize(_gridModel, _gridItemConfig, _upgradeConfig, _gridPlacer);
            _upgradeView.Initialize(_upgradeModel, _upgradeConfig);
        }

        private void LoadData()
        {
            _persistentDataService = new PersistentDataService();
            _saveAndLoader = new(_persistentDataService, _configData);
            _saveAndLoader.LoadDataFromConfig();
            //if (_saveAndLoader.TryGetGameData())
            //    _saveAndLoader.LoadDataFromCloud();
            //else
            //    _saveAndLoader.LoadDataFromConfig();
        }
    }
}