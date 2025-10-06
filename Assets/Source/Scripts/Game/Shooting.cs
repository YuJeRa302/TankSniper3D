using UniRx;
using UnityEngine;

namespace Assets.Source.Scripts.Game
{
    public class Shooting : MonoBehaviour
    {
        public static readonly IMessageBroker Message = new MessageBroker();

        [SerializeField] private Transform _shotPoint;

        private CompositeDisposable _disposables = new();
        private int _shotsCount = 0;
        private int _shotsForSuper = 3;
        private int _currentBulletCount;
        private int _maxBulletCount;

        private void Awake()
        {
            SniperScopeView.Message
                .Receive<M_EndAiming>()
                .Subscribe(m => OnShooting())
                .AddTo(_disposables);
        }

        private void OnDestroy()
        {
            _disposables?.Dispose();
        }

        private void OnShooting()
        {
            if (_shotsCount == _shotsForSuper)
                ShootWithEnergy();
            else
                ShootWithoutEnergy();
        }

        private void Reloading()
        {
            if (_currentBulletCount != 0)
                return;

            _currentBulletCount = _maxBulletCount;
            Message.Publish(new M_Reloading());
        }

        private void ShootWithoutEnergy()
        {
            _shotsCount++;
            _currentBulletCount--;
            Reloading();
            Message.Publish(new M_Shoot());
            //Instantiate(bulletPrefab, _shotPoint.position, _shotPoint.rotation);
        }

        private void ShootWithEnergy()
        {
            _shotsCount = 0;
            _currentBulletCount--;
            Reloading();
            Message.Publish(new M_SuperShoot());
            //Instantiate(bulletPrefab, _shotPoint.position, _shotPoint.rotation);
        }
    }
}