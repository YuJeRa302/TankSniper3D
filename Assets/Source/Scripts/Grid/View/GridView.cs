using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Game.Scripts.States;
using Assets.Source.Scripts.Models;
using Assets.Source.Scripts.ScriptableObjects;
using Assets.Source.Scripts.Sound;
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
        [SerializeField] private TMP_Text _moneyText;
        [SerializeField] private TMP_Text _tankCostText;
        [SerializeField] private Vector3 _gridTankSpawnRotation;
        [Space(20)]
        [SerializeField] private Button _createGridTankButton;
        [SerializeField] private Button _openUpgrade;
        [SerializeField] private Button _loadGameSceneButton;
        [Space(20)]
        [SerializeField] private Transform _mainTankSpawnPoint;
        [SerializeField] private Transform _gridTankTransformParent;
        [Space(20)]
        [SerializeField] private GameObject _sceneGameObjects;
        [Space(20)]
        [SerializeField] private Slider _buyTankSlider;
        [SerializeField] private TMP_Text _currentGridTankLevelText;

        private AudioPlayer _audioPlayer;
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

        public void Initialize(
            GridModel gridModel,
            GridConfig gridConfig,
            UpgradeConfig upgradeConfig,
            GridPlacer gridPlacer,
            AudioPlayer audioPlayer)
        {
            _gridConfig = gridConfig;
            _upgradeConfig = upgradeConfig;
            _gridPlacer = gridPlacer;
            _gridModel = gridModel;
            _audioPlayer = audioPlayer;
            AddListeners();
            ChangeSetActiveObjects(gameObject, _sceneGameObjects, true);
            CreateTankBySaves();
            CreateGridTanksBySaves();
            UpdateBuyTankSlider(_gridModel.GetMaxTankCountForBuy(), _gridModel.GetCountBuyedTanks());
            UpdateMoneyTextValue();
            UnlockButtons();
        }

        private void AddListeners()
        {
            _createGridTankButton.onClick.AddListener(SpawnObjectInFirstAvailableCell);
            _openUpgrade.onClick.AddListener(Close);
            _loadGameSceneButton.onClick.AddListener(OnLoadGameSceneButtonClicked);

            GridCellView.Message
                .Receive<M_ItemMerged>()
                .Subscribe(m => OnItemMerged(
                    m.CurrentLevel,
                    m.GridCellView,
                    m.FirstMergedTank,
                    m.SecondMergedTank))
                .AddTo(_disposables);

            UpgradeView.Message
                .Receive<M_CloseUpgrade>()
                .Subscribe(m => OnOpen())
                .AddTo(_disposables);
        }

        private void RemoveListeners()
        {
            _createGridTankButton.onClick.RemoveListener(SpawnObjectInFirstAvailableCell);
            _openUpgrade.onClick.RemoveListener(Close);
            _loadGameSceneButton.onClick.RemoveListener(OnLoadGameSceneButtonClicked);
            _disposables?.Dispose();
        }

        private void Close()
        {
            ChangeSetActiveObjects(gameObject, _sceneGameObjects, false);
            Message.Publish(new M_CloseGrid());
        }

        private void OnLoadGameSceneButtonClicked()
        {
            _gridModel.LoadGameScene();
        }

        private void OnOpen()
        {
            ChangeSetActiveObjects(gameObject, _sceneGameObjects, true);
            UpdateTankEntities();
        }

        private void OnItemMerged(
            int currentLevel,
            GridCellView gridCellView,
            GridTankState firstMergedTank,
            GridTankState secondMergedTank)
        {
            currentLevel++;
            _gridModel.IncreaseMainTankLevel(currentLevel);
            _gridModel.RemoveGridTankStateByMerge(firstMergedTank);
            _gridModel.RemoveGridTankStateByMerge(secondMergedTank);
            CreateGridTank(gridCellView, currentLevel, true);
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

        private void UpdateMoneyTextValue()
        {
            _currentGridTankLevelText.text = "Уровень" + _gridModel.CurrentGridTankLevel.ToString();
            _moneyText.text = _gridModel.GetCurrentCountMoney().ToString();
            _tankCostText.text = _gridModel.GetCurrentTankCost().ToString();
        }

        private void UpdateBuyTankSlider(int maxValue, int currentValue)
        {
            _buyTankSlider.maxValue = maxValue;
            _buyTankSlider.value = currentValue;
        }

        private void UnlockButtons()
        {
            if (_gridModel.GetTankStateByEquip() == null)
                return;

            _openUpgrade.gameObject.SetActive(true);
            _loadGameSceneButton.gameObject.SetActive(true);
        }

        private void SpawnObjectInFirstAvailableCell()
        {
            foreach (GridCellView cell in _gridPlacer.GridCellViews)
            {
                if (!cell.IsOccupied)
                {
                    if (_gridModel.TryBuyGridTank(_gridModel.GetCurrentTankCost()))
                    {
                        CreateGridTank(cell, _gridModel.CurrentGridTankLevel, false);
                        UpdateMainTank();
                        UpdateBuyTankSlider(_gridModel.GetMaxTankCountForBuy(), _gridModel.GetCountBuyedTanks());
                        UpdateMoneyTextValue();
                        UnlockButtons();
                    }
                    break;
                }
            }
        }

        private void CreateTankBySaves()
        {
            if (_gridModel.GetTankStateByEquip() == null)
                return;

            CreateMainTank(_gridModel.GetTankStateByEquip().Level);
            UpdateMainTankUI();
        }

        private void CreateGridTanksBySaves()
        {
            if (_gridModel.GetGridTankStates().Count == 0)
                return;

            for (int index = 0; index < _gridModel.GetGridTankStates().Count; index++)
            {
                CreateGridTank(_gridPlacer
                    .GetGridCellById(
                    _gridModel.GetGridTankStates()[index].GridCellId),
                    _gridModel.GetGridTankStates()[index].Level,
                    true);
            }
        }

        private void CreateGridTank(GridCellView gridCellView, int currentTankLevel, bool isCreateSoundMute)
        {
            GridTankData gridTankData = _gridConfig.GetGridTankDataByLevel(currentTankLevel);
            GridTankState gridTankState = _gridModel.CreateGridTankState(gridTankData);

            GridTankView tank = Instantiate(gridTankData.TankView, new Vector3(
                gridCellView.transform.position.x,
                gridTankData.TankView.transform.position.y,
                gridCellView.transform.position.z), Quaternion.identity);

            tank.transform.SetParent(_gridTankTransformParent.transform, worldPositionStays: true);
            tank.Initialize(gridTankData, gridTankState);
            tank.ChangeOriginalCell(gridCellView);
            gridCellView.SetOccupied(tank);

            if (isCreateSoundMute == false)
                _audioPlayer.PlayCreateTankAudio();
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