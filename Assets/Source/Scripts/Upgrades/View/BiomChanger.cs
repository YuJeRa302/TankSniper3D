using Assets.Source.Scripts.Grid;
using Assets.Source.Scripts.Models;
using Assets.Source.Scripts.ScriptableObjects;
using UnityEngine;

namespace Assets.Source.Scripts.Upgrades
{
    public class BiomChanger : MonoBehaviour
    {
        private readonly int _desertBiomId = 1;

        [SerializeField] private MeshRenderer _groundMeshRenderer;
        [SerializeField] private MeshRenderer _gridPlaceMeshRenderer;
        [SerializeField] private MeshRenderer _tankPlaceMeshRenderer;
        [SerializeField] private MeshRenderer _antiTankMeshRenderer;
        [SerializeField] private MeshRenderer _rockMeshRender;
        [Space(20)]
        [SerializeField] private Transform _treeSpawnPoint;
        [SerializeField] private Transform _desertBigRockSpawnPoint;
        [SerializeField] private Transform _desertSmallRocksSpawnPoint;
        [Space(20)]
        [SerializeField] private GridCellView _gridCellView;

        private BiomChangerConfig _biomChangerConfig;
        private LevelModel _levelModel;
        private MaterialPropertyBlock _propertyBlock;

        public void Initialize(LevelModel levelModel, BiomChangerConfig biomChangerConfig)
        {
            _levelModel = levelModel;
            _biomChangerConfig = biomChangerConfig;
            _propertyBlock = new MaterialPropertyBlock();

            ChangeGridCellMaterial();
            CreateTree();
            CreateRocks();

            ChangeColorWithPropertyBlock(
                _groundMeshRenderer,
                _biomChangerConfig.GetGroundColor(_levelModel.GetCurrentBiomIndex()));

            ChangeColorWithPropertyBlock(
                _gridPlaceMeshRenderer,
                _biomChangerConfig.GetGridPlaceColor(_levelModel.GetCurrentBiomIndex()));

            ChangeColorWithPropertyBlock(
                _tankPlaceMeshRenderer,
                _biomChangerConfig.GetGridPlaceColor(_levelModel.GetCurrentBiomIndex()));

            ChangeColorWithPropertyBlock(
                _antiTankMeshRenderer,
                _biomChangerConfig.GetAntiTankPlaceColor(_levelModel.GetCurrentBiomIndex()));

            ChangeColorWithPropertyBlock(
                _rockMeshRender,
                _biomChangerConfig.GetRockColor(_levelModel.GetCurrentBiomIndex()));
        }

        private void ChangeGridCellMaterial()
        {
            _gridCellView.SetMaterialColor(_biomChangerConfig.GridCellColor(_levelModel.GetCurrentBiomIndex()));
        }

        private void CreateTree()
        {
            Instantiate(_biomChangerConfig.GetTreeGameObject(_levelModel.GetCurrentBiomIndex()), _treeSpawnPoint);
        }

        private void CreateRocks()
        {
            if (_levelModel.GetCurrentBiomIndex() != _desertBiomId)
                return;

            Instantiate(_biomChangerConfig.GetDesertRockGameObject(), _desertBigRockSpawnPoint);
            Instantiate(_biomChangerConfig.GetSmallRocksGameObject(), _desertSmallRocksSpawnPoint);
        }

        private void ChangeColorWithPropertyBlock(MeshRenderer meshRenderer, Color color)
        {
            meshRenderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor("_Color", color);
            meshRenderer.SetPropertyBlock(_propertyBlock);
        }
    }
}