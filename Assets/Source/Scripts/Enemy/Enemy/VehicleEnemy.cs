using UnityEngine;

namespace Assets.Source.Game.Scripts.Enemy
{
    public class VehicleEnemy : Enemy
    {
        public override void Initialize(Transform tankTransform)
        {
            base.Initialize(tankTransform);
            EnemyStateStrategy.Initialize(this, null);
        }
    }
}