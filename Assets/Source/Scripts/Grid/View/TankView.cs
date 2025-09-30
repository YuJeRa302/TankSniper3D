using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Scripts.ScriptableObjects;
using Assets.Source.Scripts.Views;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Scripts.Upgrades
{
    public class TankView : MonoBehaviour
    {
        private readonly string _materialName = "Main";

        [SerializeField] private List<MeshRenderer> _decals;
        [SerializeField] private List<MeshRenderer> _tankMaterials;
        [SerializeField] private Transform _heroSpawnPoint;

        private HeroView _heroView;

        public int Level { get; private set; }

        public void Initialize(
            TankData tankData,
            DecorationData decal,
            DecorationData pattern,
            HeroData heroData,
            TypeHeroSpawn typeHeroSpawn)
        {
            Level = tankData.Level;
            UpdateDecal(decal);
            UpdatePattern(pattern);
            CreateHero(heroData, typeHeroSpawn);
        }

        private void UpdateDecal(DecorationData decorationData)
        {
            if (_decals.Count > 0)
            {
                foreach (var decal in _decals)
                    decal.material.mainTexture = decorationData.Texture;
            }
        }

        private void UpdatePattern(DecorationData decorationData)
        {
            if (_tankMaterials.Count > 0)
            {
                foreach (var meshRenderer in _tankMaterials)
                {
                    foreach (var material in meshRenderer.materials)
                    {
                        if (material.name.Contains(_materialName))
                            material.mainTexture = decorationData.Texture;
                    }
                }
            }
        }

        private void CreateHero(HeroData heroData, TypeHeroSpawn typeHeroSpawn)
        {
            if (typeHeroSpawn == TypeHeroSpawn.Upgrade)
                return;

            if (heroData == null)
                return;

            _heroView = Instantiate(heroData.HeroView, _heroSpawnPoint);
            _heroView.Initialize(heroData, typeHeroSpawn);
        }
    }
}