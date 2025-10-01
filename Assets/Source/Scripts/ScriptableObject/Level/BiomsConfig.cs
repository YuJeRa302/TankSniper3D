using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New BiomsConfig", menuName = "Create BiomsConfig", order = 51)]
    public class BiomsConfig : ScriptableObject
    {
        [SerializeField] private List<BiomData> _biomDatas;

        public List<BiomData> BiomDatas => _biomDatas;

        public BiomData GetBiomDataById(int id)
        {
            foreach (var biomData in _biomDatas)
            {
                if (biomData.Id == id)
                    return biomData;
            }

            return default;
        }
    }
}