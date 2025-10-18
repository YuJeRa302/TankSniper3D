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
        [SerializeField] private TankData _tankData;//test
        [SerializeField] private Transform _plyaerTank; // test
        [SerializeField] private CoroutineRunner _coroutineRunner;

        private SaveAndLoader _saveAndLoader;
        private PersistentDataService _persistentDataService;
        private GameModel _gameModel;

        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.AddSingleton(_coroutineRunner, typeof(ICoroutineRunner));
            LoadData();
            //CreateGameEntities(containerBuilder);
            InitEnemy();
        }

        private void Start()
        {
            _shooting.Initialize(_tankData);
        }

        private void Init()
        {
            LoadData();
            Construct();
        }

        private void Construct()
        {
            //_gameModel = new GameModel(_persistentDataService);
        }

        private void LoadData()
        {
            //_gameConfig = Resources.Load<GameConfig>(DataPath.GameConfigDataPath);
            //_persistentDataService = new PersistentDataService();
            //_saveAndLoader = new SaveAndLoader(_persistentDataService);
            //_saveAndLoader.LoadDataFromPrefs();
        }

        private void InitEnemy()
        {
            foreach (Enemy enemy in _enemies)
            {
                enemy.Initialize(_plyaerTank);
            }
        }
    }
}