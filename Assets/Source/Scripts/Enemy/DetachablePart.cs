using UnityEngine;

namespace Assets.Source.Game.Scripts.Enemy
{
    public class DetachablePart : MonoBehaviour
    {
        [SerializeField] private float _velocityMultiplier = 5f;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private float _lifeTime = 5f;

        private bool _detached = false;

        public void Detach(Vector3 hitPoint)
        {
            if (_detached)
                return;

            _detached = true;
            Vector3 hitForce = _rigidbody.velocity * _velocityMultiplier;
            transform.parent = null;
            _rigidbody.isKinematic = false;
            _rigidbody.AddForceAtPosition(hitForce, hitPoint, ForceMode.Impulse);
            Destroy(gameObject, _lifeTime);
        }
    }
}