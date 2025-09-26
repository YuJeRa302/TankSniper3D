using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Scripts.Grid;
using Assets.Source.Scripts.ScriptableObjects;
using UnityEngine;

namespace Assets.Source.Scripts.Upgrades
{
    public class TankView : ObjectView
    {
        [SerializeField] private MeshRenderer _decal;
        [SerializeField] private MeshRenderer _tankMaterial;
        [SerializeField] private Transform _heroSpawnPoint;

        public Transform HeroSpawnPoint => _heroSpawnPoint;

        public void Initialize(
            TankData tankData,
            DecorationData decal,
            DecorationData pattern,
            HeroData heroData,
            TypeHeroSpawn typeHeroSpawn)
        {
            UpdateDecal(decal);
            UpdatePattern(pattern);
            CreateHero(heroData, typeHeroSpawn);
        }

        private void UpdateDecal(DecorationData decorationData)
        {
            _decal.material.mainTexture = decorationData.Texture;
        }

        private void UpdatePattern(DecorationData decorationData)
        {
            _tankMaterial.material.mainTexture = decorationData.Texture;
        }

        private void CreateHero(HeroData heroData, TypeHeroSpawn typeHeroSpawn)
        {
            if (typeHeroSpawn == TypeHeroSpawn.Upgrade)
                return;
        }
    }
}