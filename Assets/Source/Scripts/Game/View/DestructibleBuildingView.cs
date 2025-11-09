using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Scripts.Game
{
    public class DestructibleBuildingView : MonoBehaviour
    {
        [SerializeField] private GameObject _intactPart;
        [SerializeField] private GameObject _fracturedPart;
        [SerializeField] private float destroyRadius = 5f;
        [SerializeField] private float explosionForce = 400f;
        [SerializeField] private float explosionUpwardModifier = 0.3f;
        [SerializeField] private float debrisLifetime = 6f;
        [SerializeField] private bool deactivateFarPieces = true;

        private bool _isDestroyed = false;
        private List<Rigidbody> _pieces = new();

        private void Awake()
        {
            if (_fracturedPart != null)
            {
                foreach (var rigidbody in _fracturedPart.GetComponentsInChildren<Rigidbody>(true))
                {
                    rigidbody.isKinematic = true;
                    rigidbody.detectCollisions = true;
                    _pieces.Add(rigidbody);
                }

                //_fracturedPart.SetActive(false);
            }
        }

        public void ApplyDamage(Vector3 hitPoint)
        {
            if (_isDestroyed)
                return;

            _isDestroyed = true;

            if (_intactPart != null)
                _intactPart.SetActive(false);

            _fracturedPart.SetActive(true);

            foreach (var rb in _pieces)
            {
                if (rb == null) continue;

                float distance = Vector3.Distance(hitPoint, rb.worldCenterOfMass);

                if (distance <= destroyRadius)
                {
                    rb.isKinematic = false;
                    rb.AddExplosionForce(explosionForce, hitPoint, destroyRadius, explosionUpwardModifier, ForceMode.Impulse);
                }
                else if (deactivateFarPieces)
                {
                    rb.isKinematic = true;
                }
            }

            StartCoroutine(RemoveDebris());
        }

        private IEnumerator RemoveDebris()
        {
            yield return new WaitForSeconds(debrisLifetime);

            foreach (var rb in _pieces)
            {
                if (rb == null)
                    continue;

                if (rb.transform.position.y < -10f ||
                    rb.velocity.magnitude < 0.1f)
                {
                    Destroy(rb.gameObject);
                }
            }

            if (_fracturedPart != null && _fracturedPart.transform.childCount <= 2)
                Destroy(gameObject);
        }
    }
}