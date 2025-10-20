using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Game.Scripts.States;
using Assets.Source.Scripts.Models;
using Assets.Source.Scripts.ScriptableObjects;
using Assets.Source.Scripts.Upgrades;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Scripts.Grid
{
    public class GridView : BaseView
    {
        public static readonly IMessageBroker Message = new MessageBroker();

        private readonly TypeHeroSpawn _typeHeroSpawn = TypeHeroSpawn.Tank;

        [SerializeField] private Image _levelMainTankImage;
        [SerializeField] private TMP_Text _levelMainTankText;
        [SerializeField] private Vector3 _gridTankSpawnRotation;
        [Space(20)]
        [SerializeField] private Button _createGridItemButton;
        [SerializeField] private Button _openUpgrade;
        [Space(20)]
        [SerializeField] private Transform _mainTankSpawnPoint;
        [SerializeField] private Transform _gridTankTransformParent;
        [Space(20)]
        [SerializeField] private GameObject _sceneGameObjects;

        private GridModel _gridModel;
        private GridConfig _gridConfig;
        private UpgradeConfig _upgradeConfig;
        private GridPlacer _gridPlacer;
        private TankView _mainTank;
        private CompositeDisposable _disposables = new();

        private void OnDestroy()
        {
            RemoveListeners();
        }

        public void Initialize(GridModel gridModel, GridConfig gridConfig, UpgradeConfig upgradeConfig, GridPlacer gridPlacer)
        {
            _gridConfig = gridConfig;
            _upgradeConfig = upgradeConfig;
            _gridPlacer = gridPlacer;
            _gridModel = gridModel;
            AddListeners();
            ChangeSetActiveObjects(gameObject, _sceneGameObjects, true);
        }

        private void AddListeners()
        {
            _createGridItemButton.onClick.AddListener(SpawnObjectInFirstAvailableCell);
            _openUpgrade.onClick.AddListener(Close);

            GridCellView.Message
                .Receive<M_ItemMerged>()
                .Subscribe(m => OnItemMerged(m.CurrentLevel, m.GridCellView))
                .AddTo(_disposables);

            UpgradeView.Message
                .Receive<M_CloseUpgrade>()
                .Subscribe(m => OnOpen())
                .AddTo(_disposables);
        }

        private void RemoveListeners()
        {
            _createGridItemButton.onClick.RemoveListener(SpawnObjectInFirstAvailableCell);
            _openUpgrade.onClick.RemoveListener(Close);
            _disposables?.Dispose();
        }

        private void Close()
        {
            ChangeSetActiveObjects(gameObject, _sceneGameObjects, false);
            Message.Publish(new M_CloseGrid());
        }

        private void OnOpen()
        {
            ChangeSetActiveObjects(gameObject, _sceneGameObjects, true);
            UpdateTankEntities();
        }

        private void OnItemMerged(int currentLevel, GridCellView gridCellView)
        {
            currentLevel++;
            _gridModel.IncreaseMainTankLevel(currentLevel);
            CreateGridTank(gridCellView, currentLevel);
            UpdateMainTank();
        }

        private void UpdateMainTankUI()
        {
            if (_levelMainTankImage.gameObject.activeSelf == false)
                _levelMainTankImage.gameObject.SetActive(true);

            if (_levelMainTankText.gameObject.activeSelf == false)
                _levelMainTankText.gameObject.SetActive(true);

            _levelMainTankText.text = _mainTank.Level.ToString();
        }

        private void UpdateMainTank()
        {
            if (_mainTank == null)
            {
                CreateMainTank(_gridModel.CurrentMainTankLevel);
            }

            if (_mainTank.Level < _gridModel.CurrentMainTankLevel)
            {
                Destroy(_mainTank.gameObject);
                CreateMainTank(_gridModel.CurrentMainTankLevel);
            }

            UpdateMainTankUI();
        }

        private void UpdateTankEntities()
        {
            if (_gridModel.GetTankStateByEquip().Id != _mainTank.TankState.Id)
            {
                Destroy(_mainTank.gameObject);
                CreateMainTank(_gridModel.GetTankStateByEquip().Level);
            }
            else
            {
                _mainTank.UpdateTankEntities(
                    _upgradeConfig.GetDecalDataById(_mainTank.TankState.DecalId),
                    _upgradeConfig.GetPatternDataById(_mainTank.TankState.PatternId),
                    _upgradeConfig.GetHeroDataById(_mainTank.TankState.HeroId));
            }

            UpdateMainTankUI();
        }

        private void SpawnObjectInFirstAvailableCell()
        {
            foreach (GridCellView cell in _gridPlacer.GridCellViews)
            {
                if (!cell.IsOccupied)
                {
                    CreateGridTank(cell, _gridModel.CurrentGridTankLevel);
                    UpdateMainTank();
                    break;
                }
            }
        }

        private void CreateGridTank(GridCellView gridCellView, int currentTankLevel)
        {
            GridTankData gridTankData = _gridConfig.GetGridTankDataByLevel(currentTankLevel);
            GridTankState gridTankState = _gridModel.GetGridTankState(gridTankData);

            GridTankView tank = Instantiate(gridTankData.TankView, new Vector3(
                gridCellView.transform.position.x,
                gridTankData.TankView.transform.position.y,
                gridCellView.transform.position.z), Quaternion.identity);

            tank.transform.SetParent(_gridTankTransformParent.transform, worldPositionStays: true);
            tank.Initialize(gridTankData, gridTankState);
            tank.ChangeOriginalCell(gridCellView);
            gridCellView.SetOccupied(tank);
        }

        private void CreateMainTank(int currentLevel)
        {
            TankData tankData = _gridConfig.GetMainTankDataByLevel(currentLevel);
            TankState tankState = _gridModel.GetTankStateByData(tankData);
            _gridModel.ChangeEquippedTank(tankState);

            _mainTank = Instantiate(tankData.MainTankView, new Vector3(
                _mainTankSpawnPoint.position.x,
                tankData.MainTankView.transform.position.y,
                _mainTankSpawnPoint.position.z),
                Quaternion.identity);

            _mainTank.transform.eulerAngles = new Vector3(
                _gridTankSpawnRotation.x,
                _gridTankSpawnRotation.y,
                _gridTankSpawnRotation.z);

            _mainTank.transform.SetParent(_mainTankSpawnPoint.transform, worldPositionStays: true);

            _mainTank.Initialize(
                tankState,
                tankData,
                _upgradeConfig.GetDecalDataById(tankState.DecalId),
                _upgradeConfig.GetPatternDataById(tankState.PatternId),
                _upgradeConfig.GetHeroDataById(tankState.HeroId),
                _typeHeroSpawn);
        }
    }
}