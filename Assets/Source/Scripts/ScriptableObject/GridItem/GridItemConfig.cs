using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New GridItemConfig", menuName = "Create GridItemConfig", order = 51)]
    public class GridItemConfig : ScriptableObject
    {
        [SerializeField] private List<GridItemData> _gridItemDatas;
        [SerializeField] private List<TankData> _mainTankDatas;

        public GridItemData GetGridTankDataByLevel(int level)
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
