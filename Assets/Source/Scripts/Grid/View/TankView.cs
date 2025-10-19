using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Game.Scripts.States;
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
        [SerializeField] private Transform _turretTransform;

        private TypeHeroSpawn _typeHeroSpawn;
        private DecorationData _decalData;
        private DecorationData _patternData;
        private HeroData _heroData;
        private HeroView _heroView;
        private TankState _tankState;

        public int Level { get; private set; }
        public TankState TankState => _tankState;
        public Transform TurretTransform => _turretTransform;

        public void Initialize(
            TankState tankState,
            int level,
            DecorationData decal,
            DecorationData pattern,
            HeroData heroData,
            TypeHeroSpawn typeHeroSpawn)
        {
            Level = level;
            _tankState = tankState;
            UpdateDecal(decal);
            UpdatePattern(pattern);
            CreateHero(heroData, typeHeroSpawn);
        }

        public void UpdateTankEntities(DecorationData decal, DecorationData pattern, HeroData heroData)
        {
            if (decal.Id != _decalData.Id)
                UpdateDecal(decal);

            if (pattern.Id != _patternData.Id)
                UpdatePattern(pattern);

            if (heroData != null)
            {
                if (heroData.Id != _heroData.Id)
                    CreateHero(heroData, _typeHeroSpawn);
            }
        }

        private void UpdateDecal(DecorationData decorationData)
        {
            _decalData = decorationData;

            if (_decals.Count > 0)
            {
                foreach (var decal in _decals)
                    decal.material.mainTexture = decorationData.Texture;
            }
        }

        private void UpdatePattern(DecorationData decorationData)
        {
            _patternData = decorationData;

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

        private void CreateHero(HeroData newHeroData, TypeHeroSpawn typeHeroSpawn)
        {
            DestroyHero();
            _heroData = newHeroData;
            _typeHeroSpawn = typeHeroSpawn;

            if (typeHeroSpawn == TypeHeroSpawn.Upgrade)
                return;

            if (newHeroData == null)
                return;

            _heroView = Instantiate(newHeroData.HeroView, _heroSpawnPoint);
            _heroView.Initialize(newHeroData, typeHeroSpawn);
        }

        private void DestroyHero()
        {
            if (_heroView != null)
                Destroy(_heroView.gameObject);
        }
    }
}