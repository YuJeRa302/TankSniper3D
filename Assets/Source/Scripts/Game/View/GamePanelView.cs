using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Game.Scripts.States;
using Assets.Source.Scripts.Levels;
using Assets.Source.Scripts.Models;
using Assets.Source.Scripts.ScriptableObjects;
using Assets.Source.Scripts.Sound;
using Assets.Source.Scripts.Upgrades;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Scripts.Game
{
    public class GamePanelView : MonoBehaviour
    {
        public static readonly IMessageBroker Message = new MessageBroker();

        private readonly TypeHeroSpawn _droneHero = TypeHeroSpawn.Drone;
        private readonly TypeHeroSpawn _tankHero = TypeHeroSpawn.Tank;
        private readonly float _rotationAngle = -90f;
        private readonly float _rotationDuration = 0.5f;
        private readonly float _moveDuration = 1f;
        private readonly Ease _rotationEase = Ease.InOutSine;
        private readonly Ease _moveEase = Ease.InOutQuad;

        [SerializeField] private TMP_Text _moneyText;
        [Space(20)]
        [SerializeField] private GameParametersView _gameParametersView;
        [SerializeField] private LevelsView _levelsView;
        [SerializeField] private GameObject _moneyBar;
        [Space(20)]
        [SerializeField] private Transform _droneSpawnPoint;
        [SerializeField] private Transform _startPosition;
        [SerializeField] private Transform _firePosition;
        [SerializeField] private Transform _mainTankParent;
        [Space(20)]
        [SerializeField] private Vector3 _tankSpawnRotation;
        [Space(20)]
        [SerializeField] private GameObject _reloadPanel;
        [Space(20)]
        [SerializeField] private Button _openReloadPanelButton;

        private Transform _turret;
        private TankView _mainTank;
        private GameData _gameData;
        private UpgradeConfig _upgradeConfig;
        private GameModel _gameModel;
        private CompositeDisposable _disposables = new();
        private AudioPlayer _audioPlayer;
        private Quaternion _initialRotation;

        public Transform TransformPlayerTank => _mainTank.transform;
        public Transform TransformPlayerDrone => _startPosition;

        private void OnDestroy()
        {
            RemoveListeners();
        }

        public void Initialize(
            GameModel gameModel,
            UpgradeConfig upgradeConfig,
            GameData gameData,
            AudioPlayer audioPlayer)
        {
            _gameModel = gameModel;
            _upgradeConfig = upgradeConfig;
            _gameData = gameData;
            _audioPlayer = audioPlayer;
            _moneyText.text = _gameModel.GetMoney().ToString();

            if (_gameModel.GetLevelData().TypeLevel == TypeLevel.Drone)
                CreateDroneEntities();
            else
                CreateMainTank();

            AddListeners();
        }

        private void AddListeners()
        {
            SniperScopeView.Message
                .Receive<M_Aiming>()
                .Subscribe(m => OnTankSniperScopeUsed(m.IsAiming))
                .AddTo(_disposables);

            SniperScopeView.Message
                .Receive<M_CloseScope>()
                .Subscribe(m => OnTankSniperScopeUsed(false))
                .AddTo(_disposables);

            DroneScopeView.Message
                .Receive<M_Aiming>()
                .Subscribe(m => OnDroneSniperScopeUsed(m.IsAiming))
                .AddTo(_disposables);

            DefeatTab.Message
                .Receive<M_RecoverPlayer>()
                .Subscribe(m => OnRecoverPlayer())
                .AddTo(_disposables);

            DroneHealth.Message
                .Receive<M_DeathDrone>()
                .Subscribe(m => OnDroneDeath())
                .AddTo(_disposables);

            TankHealth.Message
                .Receive<M_DeathTank>()
                .Subscribe(m => OnTankDeath())
                .AddTo(_disposables);

            MessageBroker.Default
                .Receive<M_FinishGame>()
                .Subscribe(m => OnTankDeath())
                .AddTo(_disposables);
        }

        private void RemoveListeners()
        {
            _disposables?.Dispose();
        }

        private void OnRecoverPlayer()
        {
            _gameParametersView.gameObject.SetActive(true);
            gameObject.SetActive(true);

            if (_gameModel.GetLevelData().TypeLevel == TypeLevel.Drone)
            {
                _gameModel.RecoverDroneCount();

                if (_gameModel.TryCreateDrone())
                    CreateDrone();
            }
        }

        private void OnTankDeath()
        {
            _gameParametersView.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }

        private void OnDroneSniperScopeUsed(bool state)
        {
            ChangeSetActive();
            gameObject.SetActive(!state);
        }

        private void OnTankSniperScopeUsed(bool state)
        {
            ChangeSetActive();
            gameObject.SetActive(!state);

            if (state)
                RotateAndMoveFirePosition();
            else
                RotateAndMoveBack();
        }

        private void OnDroneDeath()
        {
            if (_gameModel.TryCreateDrone())
                CreateDrone();
            else
                Message.Publish(new M_DefeatByDrone());
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

        private void CreateDroneEntities()
        {
            CreateDrone();
            CreateHero();
        }

        private void CreateHero()
        {
            HeroData heroData = _upgradeConfig.GetHeroDataById(_gameModel.GetHeroId());

            if (heroData == null)
                return;

            var hero = Instantiate(heroData.HeroView, _startPosition);
            hero.Initialize(heroData, _droneHero);
        }

        private void CreateDrone()
        {
            var drone = Instantiate(_gameData.DroneViewPrefab, new Vector3(
                _droneSpawnPoint.position.x,
                _droneSpawnPoint.position.y,
                _droneSpawnPoint.position.z),
                Quaternion.identity);

            drone.Initialize(_droneSpawnPoint);
            Message.Publish(new M_CreateDrone(drone));
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
                _audioPlayer,
                _tankHero);
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