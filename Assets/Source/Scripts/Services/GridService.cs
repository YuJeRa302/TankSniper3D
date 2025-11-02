using Assets.Source.Game.Scripts.States;
using Assets.Source.Scripts.ScriptableObjects;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Scripts.Services
{
    [Serializable]
    public class GridService
    {
        [SerializeField] private List<GridTankState> _gridTankStates = new();

        public List<GridTankState> GridTankStates => _gridTankStates;

        public void SetStates(List<GridTankState> gridTankStates)
        {
            _gridTankStates = gridTankStates;
        }

        public GridTankState GetGridTankStateByIndex(int index)
        {
            return _gridTankStates[index];
        }

        public void RemoveGridTankStateByMerge(GridTankState gridTankState)
        {
            if (gridTankState != null)
                _gridTankStates.Remove(gridTankState);
        }

        public GridTankState CreateState(GridTankData gridTankData)
        {
            GridTankState gridTankState = InitState(gridTankData);
            return gridTankState;
        }

        private GridTankState InitState(GridTankData gridTankData)
        {
            GridTankState gridTankState = new(gridTankData.Level, 0);
            _gridTankStates.Add(gridTankState);
            return gridTankState;
        }
    }
}