using Assets.Source.Scripts.ScriptableObjects;
using Assets.Source.Scripts.Services;
using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Source.Scripts.Upgrades
{
    public class TankRecoil : MonoBehaviour, IPointerDownHandler
    {
        private readonly Ease _recoilEase = Ease.OutQuad;

        [Header("Recoil settings")]
        [SerializeField] private float recoilDistance = 0.3f;
        [SerializeField] private float recoilDuration = 0.08f;
        [SerializeField] private float returnDuration = 0.12f;

        private float _shotInterval = 4.0f;
        private IShootingStrategy _shootingStrategy;
        private bool _isFiring = false;

        public void Initialize(TankData tankData, Transform firePoint)
        {
            _shootingStrategy = tankData.ShootingStrategy;
            _shootingStrategy.Construct(tankData.ProjectileData, firePoint);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_isFiring)
                StartCoroutine(Firing());
        }

        private IEnumerator Firing()
        {
            _isFiring = true;
            DoRecoil();
            _shootingStrategy.ShootWithEnergy();

            yield return new WaitForSeconds(_shotInterval);
            _isFiring = false;
        }

        private void DoRecoil()
        {
            Vector3 worldRecoil = -transform.forward * recoilDistance;

            transform.DOBlendableMoveBy(worldRecoil, recoilDuration).SetEase(_recoilEase)
                .OnComplete(() =>
                {
                    transform.DOBlendableMoveBy(-worldRecoil, returnDuration).SetEase(_recoilEase);
                });
        }
    }
}