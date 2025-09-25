using Assets.Source.Scripts.Services;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New UpgradeConfig", menuName = "Create UpgradeConfig", order = 51)]
    public class UpgradeConfig : ScriptableObject
    {
        [SerializeField] private List<DecorationData> _patternDatas;
        [SerializeField] private List<DecorationData> _decalDatas;
        [SerializeField] private List<HeroData> _heroDatas;
        [SerializeField] private List<TankData> _tankDatas;
        [SerializeField] private List<SelectionButtonData> _selectionButtonDatas;

        public List<DecorationData> PatternDatas => _patternDatas;
        public List<DecorationData> DecalDatas => _decalDatas;
        public List<HeroData> HeroDatas => _heroDatas;
        public List<TankData> TankDatas => _tankDatas;
        public List<SelectionButtonData> SelectionButtonDatas => _selectionButtonDatas;

        public DecorationData GetPatternDataById(int id)
        {
            return GetData(_patternDatas, id);
        }

        public DecorationData GetDecalDataById(int id)
        {
            return GetData(_decalDatas, id);
        }

        public HeroData GetHeroDataById(int id)
        {
            return GetData(_heroDatas, id);
        }

        public TankData GetTankDataById(int id)
        {
            return GetData(_tankDatas, id);
        }

        private T GetData<T>(List<T> data, int id)
            where T : IIdData
        {
            foreach (var d in data)
            {
                if (id == d.Id)
                    return d;
            }

            return default;
        }
    }
}