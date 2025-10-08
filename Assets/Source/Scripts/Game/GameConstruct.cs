using Assets.Source.Scripts.Grid;
using Assets.Source.Scripts.Levels;
using Assets.Source.Scripts.Models;
using Assets.Source.Scripts.Saves;
using Assets.Source.Scripts.ScriptableObjects;
using Assets.Source.Scripts.Services;
using Assets.Source.Scripts.Upgrades;
using UnityEngine;

namespace Assets.Source.Scripts.Game
{
    public class GameConstruct : MonoBehaviour
    {
        [SerializeField] private Shooting _shooting;
        [SerializeField] private TankData _tankData;//test

        private SaveAndLoader _saveAndLoader;
        private PersistentDataService _persistentDataService;
        private GameModel _gameModel;

        private void Awake()
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
    }
}