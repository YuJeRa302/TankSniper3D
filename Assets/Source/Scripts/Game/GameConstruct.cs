using Assets.Source.Game.Scripts.Enemy;
using Assets.Source.Game.Scripts.Utility;
using Assets.Source.Scripts.Models;
using Assets.Source.Scripts.Saves;
using Assets.Source.Scripts.ScriptableObjects;
using Assets.Source.Scripts.Services;
using Reflex.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Scripts.Game
{
    public class GameConstruct : MonoBehaviour, IInstaller
    {
        [SerializeField] private Shooting _shooting;
        [SerializeField] private List<Enemy> _enemies;
        [SerializeField] private GamePanelView _gamePanelView;
        [SerializeField] private CoroutineRunner _coroutineRunner;
        [Space(20)]
        [SerializeField] private ConfigData _configData;
        [SerializeField] private GameData _gameData;
        [SerializeField] private UpgradeConfig _upgradeConfig;
        [Space(20)]
        [SerializeField] private SniperScopeView _sniperScopeView;

        private SaveAndLoader _saveAndLoader;
        private PersistentDataService _persistentDataService;
        private GameModel _gameModel;

        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.AddSingleton(_coroutineRunner, typeof(ICoroutineRunner));
            LoadData();
        }

        private void Start()
        {
            Construct();
            InitEnemy();
            CreateUI();
        }

        private void CreateUI()
        {
            _sniperScopeView.Initialize(_enemies);
        }

        private void Construct()
        {
            _shooting.Initialize(_gameModel.GetTankData());
            _gamePanelView.Initialize(_gameModel, _upgradeConfig, _gameData);
        }

        private void LoadData()
        {
            _persistentDataService = new PersistentDataService();
            _saveAndLoader = new(_persistentDataService, _configData);
            _saveAndLoader.LoadDataFromConfig();
            _gameModel = new GameModel(_persistentDataService, _upgradeConfig);
            //if (_saveAndLoader.TryGetGameData())
            //    _saveAndLoader.LoadDataFromCloud();
            //else
            //    _saveAndLoader.LoadDataFromConfig();
        }

        private void InitEnemy()
        {
            foreach (Enemy enemy in _enemies)
            {
                enemy.Initialize(_gamePanelView.GetTransformPlayerTank());
            }
        }
    }
}