using Assets.Source.Scripts.Grid;
using Assets.Source.Scripts.ScriptableObjects;
using UnityEngine;

namespace Assets.Source.Scripts.Upgrades
{
    public class MainTankView : ObjectView
    {
        [SerializeField] private MeshRenderer _decal;
        [SerializeField] private MeshRenderer _tankMaterial;

        public void Initialize(TankData tankData, DecorationData decal, DecorationData pattern, HeroData heroData)
        {
            UpdateDecal(decal);
            UpdatePattern(pattern);
            CreateHero(heroData);
        }

        private void UpdateDecal(DecorationData decorationData)
        {
            _decal.material.mainTexture = decorationData.Texture;
        }

        private void UpdatePattern(DecorationData decorationData)
        {
            _tankMaterial.material.mainTexture = decorationData.Texture;
        }

        private void CreateHero(HeroData heroData)
        {

        }
    }
}