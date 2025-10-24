using Assets.Source.Scripts.Models;
using Assets.Source.Scripts.ScriptableObjects;
using Assets.Source.Scripts.Services;
using UniRx;
using UnityEngine;

namespace Assets.Source.Scripts.Game
{
    public class Shooting : MonoBehaviour
    {
        public static readonly IMessageBroker Message = new MessageBroker();

        private readonly int _maxProjectileCount = 4;

        [SerializeField] private Transform _shotPoint;

        private IShootingStrategy _shootingStrategy;
        private CompositeDisposable _disposables = new();
        private GameModel _gameModel;
        private TankData _tankData;
        private int _shotsCount = 0;
        private int _shotsForSuper = 3;
        private int _currentProjectileCount;

        private void Awake()
        {
            SniperScopeView.Message
                .Receive<M_Aiming>()
                .Subscribe(m => OnShooting(m.IsAiming))
                .AddTo(_disposables);
        }

        private void OnDestroy()
        {
            _disposables?.Dispose();
        }

        public void Initialize(GameModel gameModel)
        {
            _tankData = gameModel.GetTankData();
            _currentProjectileCount = _maxProjectileCount;
            _shootingStrategy = _tankData.ShootingStrategy;
            _shootingStrategy.Construct(_tankData.ProjectileData, _shotPoint);
        }

        private void OnShooting(bool isAiming)
        {
            if (isAiming)
                return;

            if (_shotsCount == _shotsForSuper)
                ShootWithEnergy();
            else
                ShootWithoutEnergy();
        }

        private void Reloading()
        {
            if (_currentProjectileCount != 0)
                return;

            _currentProjectileCount = _maxProjectileCount;
            Message.Publish(new M_Reloading(_tankData.ProjectileData.ReloadTime));
        }

        private void ShootWithoutEnergy()
        {
            _shotsCount++;
            _currentProjectileCount--;
            _shootingStrategy.ShootWithoutEnergy(_gameModel.GetVibroState());
            Message.Publish(new M_Shoot());
            Reloading();
        }

        private void ShootWithEnergy()
        {
            _shotsCount = 0;
            _currentProjectileCount--;
            _shootingStrategy.ShootWithEnergy(_gameModel.GetVibroState());
            Message.Publish(new M_SuperShoot());
            Reloading();
        }
    }
}