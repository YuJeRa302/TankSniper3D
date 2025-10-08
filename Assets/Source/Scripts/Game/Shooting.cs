using Assets.Source.Scripts.ScriptableObjects;
using Assets.Source.Scripts.Services;
using UniRx;
using UnityEngine;

namespace Assets.Source.Scripts.Game
{
    public class Shooting : MonoBehaviour
    {
        public static readonly IMessageBroker Message = new MessageBroker();

        [SerializeField] private Transform _shotPoint;

        private IShootingStrategy _shootingStrategy;
        private CompositeDisposable _disposables = new();
        private TankData _tankData;
        private int _shotsCount = 0;
        private int _shotsForSuper = 3;
        private int _currentProjectileCount;
        private int _maxProjectileCount;

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

        public void Initialize(TankData tankData)
        {
            _tankData = tankData;
            _maxProjectileCount = _tankData.ProjectileData.ProjectileCount;
            _currentProjectileCount = _maxProjectileCount;
            _shootingStrategy = _tankData.ProjectileData.ShootingStrategy;
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
            _shootingStrategy.ShootWithoutEnergy();
            Message.Publish(new M_Shoot());
            Reloading();
        }

        private void ShootWithEnergy()
        {
            _shotsCount = 0;
            _currentProjectileCount--;
            _shootingStrategy.ShootWithEnergy();
            Message.Publish(new M_SuperShoot());
            Reloading();
        }
    }
}