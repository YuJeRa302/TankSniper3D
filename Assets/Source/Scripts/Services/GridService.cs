using Assets.Source.Game.Scripts.States;
using Assets.Source.Scripts.Grid;
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

        public List<GridTankState> GridTankState => _gridTankStates;

        public void SetStates(List<GridTankState> gridTankStates)
        {
            _gridTankStates = gridTankStates;
        }

        //public void SetOriginalCell(GridTankView gridTankView)
        //{
        //    if (_gridTankStates != null)
        //    {
        //        GridTankState gridTankState = FindState(gridTankView.GridItemData.Id);

        //        if (gridTankState == null)
        //            gridTankState = InitState(gridTankView.GridItemData);
        //        else
        //            gridTankState.ChangeOriginalCell(gridTankView.OriginalCell.transform.position);
        //    }
        //}

        public GridTankState GetState(GridTankData gridTankData)
        {
            GridTankState gridTankState = FindState(gridTankData.Id);

            if (gridTankState == null)
                gridTankState = InitState(gridTankData);

            return gridTankState;
        }

        private GridTankState FindState(int id)
        {
            if (_gridTankStates != null)
            {
                foreach (GridTankState gridTankState in _gridTankStates)
                {
                    if (gridTankState.Id == id)
                        return gridTankState;
                }
            }

            return null;
        }

        private GridTankState InitState(GridTankData gridTankData)
        {
            GridTankState gridTankState = new(gridTankData.Id, Vector3.zero);
            _gridTankStates.Add(gridTankState);
            return gridTankState;
        }
    }
}