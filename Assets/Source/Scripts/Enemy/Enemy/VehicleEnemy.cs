using UniRx;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Enemy
{
    public class VehicleEnemy : Enemy
    {
        private readonly Vector3 _hitPoint = Vector3.zero;
        private readonly int _killZoneDamage = 1000;

        [SerializeField] private DamageableArea _enemyInVehicle;

        private CompositeDisposable _disposables = new();

        private void OnDestroy()
        {
            _disposables?.Dispose();
        }

        public override void Initialize(Transform tankTransform)
        {
            base.Initialize(tankTransform);
            EnemyStateStrategy.Initialize(this, null);

            EnemyHealth.Message
                .Receive<M_DeathEnemy>()
                .Subscribe(m => OnEnemyDeath())
                .AddTo(_disposables);
        }

        private void OnEnemyDeath()
        {
            if (_enemyInVehicle != null)
                _enemyInVehicle.ApplyDamage(_killZoneDamage, _hitPoint);
        }
    }
}