using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New BiomChangerConfig", menuName = "Create BiomChangerConfig", order = 51)]
    public class BiomChangerConfig : ScriptableObject
    {
        [SerializeField] private List<Color> _gridPlaceColors;
        [SerializeField] private List<Color> _gridCellColors;
        [SerializeField] private List<Color> _antiTankColors;
        [SerializeField] private List<Color> _groundColor;
        [SerializeField] private List<Color> _rockColors;
        [SerializeField] private List<GameObject> _treeGameObjects;
        [Space(20)]
        [SerializeField] private GameObject _desertRockGameObject;
        [SerializeField] private GameObject _smallRocksGameObject;

        public Color GetGridPlaceColor(int id)
        {
            return _gridPlaceColors[id];
        }

        public Color GridCellColor(int id)
        {
            return _gridCellColors[id];
        }

        public Color GetAntiTankPlaceColor(int id)
        {
            return _antiTankColors[id];
        }

        public Color GetGroundColor(int id)
        {
            return _groundColor[id];
        }

        public Color GetRockColor(int id)
        {
            return _rockColors[id];
        }

        public GameObject GetTreeGameObject(int id)
        {
            return _treeGameObjects[id];
        }

        public GameObject GetDesertRockGameObject()
        {
            return _desertRockGameObject;
        }

        public GameObject GetSmallRocksGameObject()
        {
            return _smallRocksGameObject;
        }
    }
}