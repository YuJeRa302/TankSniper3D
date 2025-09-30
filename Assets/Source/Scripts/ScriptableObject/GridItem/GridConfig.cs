using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New GridConfig", menuName = "Create GridConfig", order = 51)]
    public class GridConfig : ScriptableObject
    {
        [SerializeField] private List<GridTankData> _gridItemDatas;
        [SerializeField] private List<TankData> _mainTankDatas;

        public GridTankData GetGridTankDataByLevel(int level)
        {
            foreach (var gridItemData in _gridItemDatas)
            {
                if (level == gridItemData.Level)
                    return gridItemData;
            }

            return default;
        }

        public TankData GetMainTankDataByLevel(int level)
        {
            foreach (var tankData in _mainTankDatas)
            {
                if (level == tankData.Level)
                    return tankData;
            }

            return default;
        }
    }
}
