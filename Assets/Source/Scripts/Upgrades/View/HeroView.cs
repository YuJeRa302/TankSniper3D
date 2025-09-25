using Assets.Source.Game.Scripts.Enums;
using UnityEngine;

namespace Assets.Source.Scripts.Views
{
    public class HeroView : MonoBehaviour
    {
        [SerializeField] private GameObject _vrHeadSet;
        [SerializeField] private GameObject _droneController;
        [SerializeField] private GameObject _headGear;
        [SerializeField] private Animator _animator;

        public void Initialize(TypeHeroSpawn typeHeroSpawn)
        {
            PlayAnimation(typeHeroSpawn);
            SetNewGear(typeHeroSpawn);
        }

        private void PlayAnimation(TypeHeroSpawn typeHeroSpawn)
        {
            _animator.SetTrigger(typeHeroSpawn.ToString());
        }

        private void SetNewGear(TypeHeroSpawn typeHeroSpawn)
        {
            if (typeHeroSpawn != TypeHeroSpawn.Drone)
                return;

            _vrHeadSet.gameObject.SetActive(true);
            _droneController.gameObject.SetActive(true);
            _headGear.gameObject.SetActive(false);
        }
    }
}