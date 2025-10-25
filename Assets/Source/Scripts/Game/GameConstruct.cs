using Assets.Source.Game.Scripts.Enemy;
using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Game.Scripts.Utility;
using Assets.Source.Scripts.Levels;
using Assets.Source.Scripts.Models;
using Assets.Source.Scripts.Saves;
using Assets.Source.Scripts.ScriptableObjects;
using Assets.Source.Scripts.Services;
using Assets.Source.Scripts.Sound;
using Reflex.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Scripts.Game
{
    public class GameConstruct : MonoBehaviour, IInstaller
    {
        [SerializeField] private Shooting _shooting;
        [SerializeField] private List<Enemy> _enemies;
        [Space(20)]
        [SerializeField] private CameraMover _cameraMover;
        [Space(20)]
        [SerializeField] private CoroutineRunner _coroutineRunner;
        [Space(20)]
        [SerializeField] private ConfigData _configData;
        [SerializeField] private GameData _gameData;
        [SerializeField] private UpgradeConfig _upgradeConfig;
        [Space(20)]
        [SerializeField] private LevelsView _levelsView;
        [SerializeField] private SettingsView _settingsView;
        [SerializeField] private GamePanelView _gamePanelView;
        [SerializeField] private GameParametersView _gameParametersView;
        [SerializeField] private ReloadLevelTab _reloadLevelTab;
        [SerializeField] private DefeatTab _defeatTab;
        [Space(20)]
        [SerializeField] private Transform _scopeParent;
        //[SerializeField] private LevelData _levelData; для тестирования дрона
        [Space(20)]
        [SerializeField] private Button _sniperScopeButton;
        [Space(20)]
        [SerializeField] private AudioPlayer _audioPlayer;

        private GamePauseService _gamePauseService;
        private SaveAndLoader _saveAndLoader;
        private PersistentDataService _persistentDataService;
        private GameModel _gameModel;
        private LevelModel _levelModel;
        private SettingsModel _settingsModel;
        private LevelData _levelData;

        private void OnDestroy()
        {
            _gamePauseService.Dispose();
        }

        private void Start()
        {
            Construct();
        }

        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            LoadData();
            _gamePauseService = new GamePauseService(_persistentDataService);

            containerBuilder
                .AddSingleton(_gamePauseService)
                .AddSingleton(_coroutineRunner, typeof(ICoroutineRunner));
        }

        private void CreateUI()
        {
            CreateScope(_sniperScopeButton);
            _gamePanelView.Initialize(_gameModel, _upgradeConfig, _gameData, _levelData);
            _shooting.Initialize(_gameModel);
            _levelsView.Initialize(_levelModel, _gameData.BiomsConfig);
            _gameParametersView.Initialize(_gameModel.GetTankData().Health, _enemies.Count);
            _settingsView.Initialize(_settingsModel, _audioPlayer);
            _reloadLevelTab.Initialize(_gameModel);
            _defeatTab.Initialize(_gameModel);
        }

        private void CreateScope(Button sniperScopeButton)
        {
            if (_levelData.TypeLevel == TypeLevel.Drone)
                CreateDroneScope(sniperScopeButton);
            else
                CreateTankScope(sniperScopeButton);
        }

        private void CreateTankScope(Button sniperScopeButton)
        {
            var crosshairInstance = Instantiate(_gameData.SniperScopeView);
            crosshairInstance.transform.SetParent(_scopeParent, false);
            crosshairInstance.Initialize(_enemies, sniperScopeButton);
        }

        private void CreateDroneScope(Button sniperScopeButton)
        {
            var crosshairInstance = Instantiate(_gameData.DroneScopeView);
            crosshairInstance.transform.SetParent(_scopeParent, false);
            crosshairInstance.Initialize(sniperScopeButton);
        }

        private void Construct()
        {
            _gameModel = new GameModel(_persistentDataService, _upgradeConfig, _gameData);
            _levelModel = new LevelModel(_persistentDataService, _gameData.BiomsConfig);
            _settingsModel = new SettingsModel(_persistentDataService, _audioPlayer, _gamePauseService);
            _levelData = _gameModel.GetLevelData();
            _cameraMover.Initialize(_gamePauseService);
            CreateUI();

            if (_levelData.TypeLevel == TypeLevel.Drone)
                InitEnemy(_gamePanelView.TransformPlayerDrone);
            else
                InitEnemy(_gamePanelView.TransformPlayerTank);
        }

        private void LoadData()
        {
            _persistentDataService = new PersistentDataService();
            _saveAndLoader = new(_persistentDataService, _configData);
            _saveAndLoader.LoadDataFromPrefs();
            //_saveAndLoader.LoadDataFromConfig(); для тестирования дрона
        }

        private void InitEnemy(Transform playerTransform)
        {
            foreach (Enemy enemy in _enemies)
            {
                enemy.Initialize(playerTransform, _gameModel);
            }
        }
    }
}