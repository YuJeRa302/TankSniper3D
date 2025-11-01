using Assets.Source.Game.Scripts.Utility;
using Assets.Source.Scripts.Game;
using Assets.Source.Scripts.Grid;
using Assets.Source.Scripts.Levels;
using Assets.Source.Scripts.Models;
using Assets.Source.Scripts.Saves;
using Assets.Source.Scripts.ScriptableObjects;
using Assets.Source.Scripts.Services;
using Assets.Source.Scripts.Sound;
using Reflex.Core;
using UnityEngine;
using YG;

namespace Assets.Source.Scripts.Upgrades
{
    public class UpgradeConstruct : MonoBehaviour, IInstaller
    {
        [SerializeField] private BiomsConfig _biomsConfig;
        [SerializeField] private ConfigData _configData;
        [SerializeField] private GridConfig _gridItemConfig;
        [SerializeField] private UpgradeConfig _upgradeConfig;
        [SerializeField] private BiomChangerConfig _biomChangerConfig;
        [Space(20)]
        [SerializeField] private GridPlacer _gridPlacer;
        [SerializeField] private GridView _gridView;
        [SerializeField] private UpgradeView _upgradeView;
        [SerializeField] private LevelsView _levelsView;
        [SerializeField] private SettingsView _settingsView;
        [Space(20)]
        [SerializeField] private CoroutineRunner _coroutineRunner;
        [Space(20)]
        [SerializeField] private AudioPlayer _audioPlayer;
        [Space(20)]
        [SerializeField] private BiomChanger _biomChanger;

        private SaveAndLoader _saveAndLoader;
        private PersistentDataService _persistentDataService;
        private GridModel _gridModel;
        private UpgradeModel _upgradeModel;
        private LevelModel _levelModel;
        private SettingsModel _settingsModel;

        private void Awake()
        {
            Init();

            if (YG2.isGameplaying != true)
                InitYandexGameEntities();
        }

        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.AddSingleton(_coroutineRunner, typeof(ICoroutineRunner));
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
            _gridModel = new GridModel(_persistentDataService, _coroutineRunner, _saveAndLoader, _biomsConfig);
            _levelModel = new LevelModel(_persistentDataService, _biomsConfig);
            _settingsModel = new SettingsModel(_persistentDataService, _audioPlayer);
            _gridPlacer.Initialize(_audioPlayer);
            _biomChanger.Initialize(_levelModel, _biomChangerConfig);
            _gridView.Initialize(_gridModel, _gridItemConfig, _upgradeConfig, _gridPlacer, _audioPlayer);
            _upgradeView.Initialize(_upgradeModel, _upgradeConfig, _audioPlayer);
            _levelsView.Initialize(_levelModel, _biomsConfig);
            _settingsView.Initialize(_settingsModel, _audioPlayer);
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