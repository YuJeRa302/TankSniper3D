using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Game.Scripts.States;
using Assets.Source.Scripts.Levels;
using Assets.Source.Scripts.Models;
using Assets.Source.Scripts.ScriptableObjects;
using Assets.Source.Scripts.Upgrades;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Scripts.Game
{
    public class GamePanelView : MonoBehaviour
    {
        public static readonly IMessageBroker Message = new MessageBroker();

        private readonly TypeHeroSpawn _typeHeroSpawn = TypeHeroSpawn.Tank;
        private readonly float _rotationAngle = -90f;
        private readonly float _rotationDuration = 0.5f;
        private readonly float _moveDuration = 1f;
        private readonly Ease _rotationEase = Ease.InOutSine;
        private readonly Ease _moveEase = Ease.InOutQuad;

        [SerializeField] private GameParametersView _gameParametersView;
        [SerializeField] private LevelsView _levelsView;
        [SerializeField] private GameObject _moneyBar;
        [Space(20)]
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _openReloadPanelButton;
        [Space(20)]
        [SerializeField] private Transform _startPosition;
        [SerializeField] private Transform _firePosition;
        [SerializeField] private Transform _mainTankParent;
        [Space(20)]
        [SerializeField] private Vector3 _tankSpawnRotation;
        [Space(20)]
        [SerializeField] private GameObject _reloadPanel;
        [Space(20)]
        [SerializeField] private Button _reloadLevelButton;
        [SerializeField] private Button _cancelButton;
        [SerializeField] private Button _backButton;
        [Space(20)]
        [SerializeField] private Button _sniperScopeButton;

        private Transform _turret;
        private TankView _mainTank;
        private GameData _gameData;
        private UpgradeConfig _upgradeConfig;
        private GameModel _gameModel;
        private CompositeDisposable _disposables = new();
        private Quaternion _initialRotation;

        public Button SniperScopeButton => _sniperScopeButton;
        public Transform TransformPlayerTank => _mainTank.transform;

        private void OnDestroy()
        {
            RemoveListeners();
        }

        public void Initialize(GameModel gameModel, UpgradeConfig upgradeConfig, GameData gameData)
        {
            _gameModel = gameModel;
            _upgradeConfig = upgradeConfig;
            _gameData = gameData;
            CreateMainTank();
            AddListeners();
        }

        private void AddListeners()
        {
            _settingsButton.onClick.AddListener(OnSettingsButton);
            _cancelButton.onClick.AddListener(OnCloseReloadPanelClicked);
            _backButton.onClick.AddListener(OnCloseReloadPanelClicked);
            _reloadLevelButton.onClick.AddListener(OnSceneReloaded);
            _openReloadPanelButton.onClick.AddListener(OnOpenReloadPanelClicked);

            SniperScopeView.Message
                .Receive<M_Aiming>()
                .Subscribe(m => OnSniperScopeUsed(m.IsAiming))
                .AddTo(_disposables);
        }

        private void RemoveListeners()
        {
            _settingsButton.onClick.RemoveListener(OnSettingsButton);
            _cancelButton.onClick.RemoveListener(OnCloseReloadPanelClicked);
            _backButton.onClick.RemoveListener(OnCloseReloadPanelClicked);
            _reloadLevelButton.onClick.RemoveListener(OnSceneReloaded);
            _openReloadPanelButton.onClick.RemoveListener(OnOpenReloadPanelClicked);
            _disposables?.Dispose();
        }

        private void OnOpenReloadPanelClicked()
        {
            _reloadPanel.SetActive(true);
            MessageBroker.Default.Publish(new M_OpenPanel());
        }

        private void OnCloseReloadPanelClicked()
        {
            _reloadPanel.SetActive(false);
            MessageBroker.Default.Publish(new M_ClosePanel());
        }

        private void OnSceneReloaded()
        {
            _gameModel.ReloadScene();
        }

        private void OnSettingsButton()
        {

        }

        private void OnSniperScopeUsed(bool state)
        {
            ChangeSetActive();
            gameObject.SetActive(!state);

            if (state)
                RotateAndMoveFirePosition();
            else
                RotateAndMoveBack();
        }

        private void ChangeSetActive()
        {
            if (_moneyBar.activeSelf == false)
                return;

            _levelsView.gameObject.SetActive(false);
            _moneyBar.SetActive(false);
            _openReloadPanelButton.gameObject.SetActive(true);
            _gameParametersView.gameObject.SetActive(true);
        }

        private void CreateMainTank()
        {
            TankData tankData = _gameModel.GetTankData();
            TankState tankState = _gameModel.GetTankState(tankData);

            _mainTank = Instantiate(tankData.MainTankView, new Vector3(
                _startPosition.position.x,
                _startPosition.position.y,
                _startPosition.position.z),
                Quaternion.identity);

            _mainTank.transform.eulerAngles = new Vector3(
                _tankSpawnRotation.x,
                _tankSpawnRotation.y,
                _tankSpawnRotation.z);

            _mainTank.transform.SetParent(_mainTankParent.transform, worldPositionStays: true);
            _turret = _mainTank.TurretTransform;
            _initialRotation = _turret.localRotation;

            _mainTank.Initialize(
                tankState,
                tankData,
                _upgradeConfig.GetDecalDataById(tankState.DecalId),
                _upgradeConfig.GetPatternDataById(tankState.PatternId),
                _upgradeConfig.GetHeroDataById(tankState.HeroId),
                _typeHeroSpawn);
        }

        private void RotateAndMoveFirePosition()
        {
            _turret.DOLocalRotate(new Vector3(0, _rotationAngle, 0), _rotationDuration)
                .SetEase(_rotationEase);

            _mainTank.transform
                .DOMove(_firePosition.position, _moveDuration)
                .SetEase(_moveEase);
        }

        private void RotateAndMoveBack()
        {
            _turret.DOLocalRotateQuaternion(_initialRotation, _rotationDuration)
                .SetEase(_rotationEase);

            _mainTank.transform
                .DOMove(_startPosition.position, _moveDuration)
                .SetEase(_moveEase);
        }
    }
}