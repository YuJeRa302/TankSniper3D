using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Scripts.Game
{
    public class DestructibleObjectView : MonoBehaviour
    {
        [SerializeField] private BoxCollider _collider;
        [SerializeField] private GameObject _mainObject;
        [SerializeField] private List<GameObject> _pieces = new();
        [SerializeField] private float _pieceForce;
        [SerializeField] private float _pieceUpwards;
        [SerializeField] private float _pieceTorque;

        private bool _isBroken = false;

        public void ApplyDamage(Vector3 hitPoint)
        {
            if (_isBroken)
                return;

            Destroy(hitPoint);
        }

        private void SetPartsObjectVisible()
        {
            foreach (var part in _pieces)
            {
                part.gameObject.SetActive(true);
            }
        }

        private void Destroy(Vector3 hitPoint)
        {
            if (_isBroken)
                return;

            Destroy(_mainObject);
            SetPartsObjectVisible();

            _isBroken = true;

            if (_collider)
                _collider.enabled = false;

            foreach (var piece in _pieces)
            {
                if (piece == null)
                    continue;

                var rigidbody = piece.GetComponent<Rigidbody>();

                if (rigidbody != null)
                {
                    rigidbody.isKinematic = false;
                    Vector3 dir = (piece.transform.position - hitPoint).normalized + Vector3.up * _pieceUpwards;
                    rigidbody.AddForce(dir.normalized * _pieceForce, ForceMode.Impulse);
                    rigidbody.AddTorque(Random.insideUnitSphere * _pieceTorque, ForceMode.Impulse);
                }
            }
        }
    }
}