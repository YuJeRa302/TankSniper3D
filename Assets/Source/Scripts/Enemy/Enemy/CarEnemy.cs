using UnityEngine;

namespace Assets.Source.Game.Scripts.Enemy
{
    public class CarEnemy : Enemy
    {
        private readonly int _multiplierRotation = 2;
        private readonly float _degrees = 360f;

        [SerializeField] private Transform[] _wheels;
        [SerializeField] private float _wheelRadius = 0.3f;

        public override void Initialize(Transform tankTransform)
        {
            base.Initialize(tankTransform);
            EnemyStateStrategy.Initialize(this, null);
        }

        private void Update()
        {
            //RotateWheels(MoveSpeed);
        }

        private void RotateWheels(float moveSpeed)
        {
            float rotationAngle = (moveSpeed * Time.deltaTime) /
                (_multiplierRotation * Mathf.PI * _wheelRadius) * _degrees;

            foreach (Transform wheel in _wheels)
            {
                wheel.Rotate(rotationAngle, 0, 0);
            }
        }
    }
}