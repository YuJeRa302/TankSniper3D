using Assets.Source.Game.Scripts.Enemy;
using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Game.Scripts.Utility;
using Assets.Source.Scripts.Levels;
using Assets.Source.Scripts.Models;
using Assets.Source.Scripts.Saves;
using Assets.Source.Scripts.ScriptableObjects;
using Assets.Source.Scripts.Services;
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
        [SerializeField] private CoroutineRunner _coroutineRunner;
        [Space(20)]
        [SerializeField] private ConfigData _configData;
        [SerializeField] private GameData _gameData;
        [SerializeField] private UpgradeConfig _upgradeConfig;
        [Space(20)]
        [SerializeField] private LevelsView _levelsView;
        [SerializeField] private GamePanelView _gamePanelView;
        [SerializeField] private GameParametersView _gameParametersView;
        [Space(20)]
        [SerializeField] private Transform _scopeParent;

        private GamePauseService _gamePauseService;
        private SaveAndLoader _saveAndLoader;
        private PersistentDataService _persistentDataService;
        private GameModel _gameModel;
        private LevelModel _levelModel;
        private LevelData _levelData;

        private void OnDestroy()
        {
            _gamePauseService.Dispose();
        }

        private void Start()
        {
            Construct();
            InitEnemy();
        }

        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            LoadData();
            CreateInstanceBindings();

            containerBuilder
                .AddSingleton(_gamePauseService)
                .AddSingleton(_coroutineRunner, typeof(ICoroutineRunner));
        }

        private void CreateUI()
        {
            _gamePanelView.Initialize(_gameModel, _upgradeConfig, _gameData);
            _shooting.Initialize(_gameModel.GetTankData());
            _levelsView.Initialize(_levelModel, _gameData.BiomsConfig);
            _gameParametersView.Initialize(_gameModel.GetTankData().Health, _enemies.Count);
            CreateScope(_gamePanelView.SniperScopeButton);
        }

        private void CreateScope(Button sniperScopeButton)
        {
            _levelData = _gameModel.GetLevelData();

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

        private void CreateDroneScope(Button sniperScopeButton) { }

        private void CreateInstanceBindings()
        {
            _gamePauseService = new GamePauseService(_persistentDataService);
        }

        private void Construct()
        {
            _gameModel = new GameModel(_persistentDataService, _upgradeConfig, _gameData);
            _levelModel = new LevelModel(_persistentDataService, _gameData.BiomsConfig);
            CreateUI();
        }

        private void LoadData()
        {
            _persistentDataService = new PersistentDataService();
            _saveAndLoader = new(_persistentDataService, _configData);
            _saveAndLoader.LoadDataFromPrefs();
        }

        private void InitEnemy()
        {
            foreach (Enemy enemy in _enemies)
            {
                enemy.Initialize(_gamePanelView.TransformPlayerTank);
            }
        }
    }
}