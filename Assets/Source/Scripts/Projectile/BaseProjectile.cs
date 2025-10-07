using UnityEngine;

namespace Assets.Source.Scripts.Projectile
{
    public abstract class BaseProjectile : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;

        private void OnCollisionEnter(Collision collision)
        {
            Hit(collision);
        }

        public virtual void Initialize() { }

        protected abstract void Hit(Collision collision);
    }
}