using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Scripts.ScriptableObjects;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Scripts.Views
{
    public class HeroView : MonoBehaviour
    {
        [SerializeField] private Transform _vrSetSpawnPoint;
        [SerializeField] private Transform _vrControllerSpawnPoint;
        [SerializeField] private List<GameObject> _headGears;
        [SerializeField] private Animator _animator;

        private HeroData _heroData;

        public void Initialize(HeroData heroData, TypeHeroSpawn typeHeroSpawn)
        {
            _heroData = heroData;
            PlayAnimation(typeHeroSpawn);
            SetNewGear(typeHeroSpawn);
        }

        private void PlayAnimation(TypeHeroSpawn typeHeroSpawn)
        {
            _animator.Play(typeHeroSpawn.ToString());
        }

        private void SetNewGear(TypeHeroSpawn typeHeroSpawn)
        {
            if (typeHeroSpawn != TypeHeroSpawn.Drone)
                return;

            ChangeSetActiveHeadGears();
            Instantiate(_heroData.VrSet, _vrSetSpawnPoint);
            Instantiate(_heroData.VrController, _vrControllerSpawnPoint);
        }

        private void ChangeSetActiveHeadGears()
        {
            if (_headGears.Count > 0)
            {
                foreach (GameObject gear in _headGears)
                    gear.gameObject.SetActive(false);
            }
        }
    }
}