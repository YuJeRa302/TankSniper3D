using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Scripts.ScriptableObjects;
using Assets.Source.Scripts.Services;
using Assets.Source.Scripts.Sound;
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
        private TypeHeroSpawn _typeHeroSpawn;
        private IShootingStrategy _shootingStrategy;
        private bool _isFiring = false;
        private Coroutine _fireCoroutine;

        public void Initialize(TankData tankData, AudioPlayer audioPlayer, Transform firePoint, TypeHeroSpawn typeHeroSpawn)
        {
            _shootingStrategy = tankData.ShootingStrategy;
            _typeHeroSpawn = typeHeroSpawn;
            _shootingStrategy.Construct(tankData.ProjectileData, audioPlayer, firePoint);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_typeHeroSpawn != TypeHeroSpawn.Upgrade)
            {
                if (!_isFiring)
                    SetFire();
            }
        }

        public void SetFire()
        {
            if (_fireCoroutine != null)
                StopCoroutine(_fireCoroutine);

            _fireCoroutine = StartCoroutine(Firing());
        }

        private IEnumerator Firing()
        {
            _isFiring = true;
            DoRecoil();
            _shootingStrategy.ShootWithEnergy(false);

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