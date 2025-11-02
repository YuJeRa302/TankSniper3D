using UnityEngine;

namespace Assets.Source.Scripts.Game
{
    public struct M_PlayerHitFromDirection
    {
        private readonly Vector3 _hitDirection;

        public M_PlayerHitFromDirection(Vector3 hitDirection)
        {
            _hitDirection = hitDirection;
        }

        public readonly Vector3 HitDirection => _hitDirection;
    }
}