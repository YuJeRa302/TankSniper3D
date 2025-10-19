using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Game.Scripts.States;
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

        [SerializeField] private Button _settingsButton;
        [SerializeField] private Transform _startPosition;
        [SerializeField] private Transform _firePosition;
        [SerializeField] private Transform _mainTankParent;
        [Space(20)]
        [SerializeField] private Vector3 _tankSpawnRotation;

        private Transform _turret;
        private TankView _mainTank;
        private GameData _gameData;
        private UpgradeConfig _upgradeConfig;
        private GameModel _gameModel;
        private CompositeDisposable _disposables = new();
        [SerializeField] private Quaternion _initialRotation;

        private void Awake()
        {
            AddListeners();
        }

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
        }

        public Transform GetTransformPlayerTank()
        {
            return _mainTank.transform;
        }

        private void AddListeners()
        {
            _settingsButton.onClick.AddListener(OnSettingsButton);

            SniperScopeView.Message
                .Receive<M_Aiming>()
                .Subscribe(m => OnSniperScopeUsed(m.IsAiming))
                .AddTo(_disposables);
        }

        private void RemoveListeners()
        {
            _settingsButton.onClick.RemoveListener(OnSettingsButton);
            _disposables?.Dispose();
        }

        private void OnSettingsButton()
        {

        }

        private void OnSniperScopeUsed(bool state)
        {
            gameObject.SetActive(!state);

            if (state)
                RotateAndMoveFirePosition();
            else
                RotateAndMoveBack();
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
                tankData.Level,
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