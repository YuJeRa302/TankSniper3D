using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New BiomData", menuName = "Create BiomData", order = 51)]
    public class BiomData : ScriptableObject
    {
        [SerializeField] private int _id;
        [SerializeField] private List<LevelData> _levelDatas;
        [SerializeField] private Sprite _icon;

        public int Id => _id;
        public List<LevelData> LevelDatas => _levelDatas;
        public Sprite Icon => _icon;
    }
}