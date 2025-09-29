using Assets.Source.Game.Scripts.States;
using Assets.Source.Scripts.Grid;
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

        public void SetState(GridTankView gridTankView)
        {
            if (_gridTankStates != null)
            {
                GridTankState gridTankState = FindState(gridTankView.GridItemData.Id);

                if (gridTankState == null)
                    gridTankState = InitState(gridTankView);
                else
                    gridTankState.ChangeOriginalCell(gridTankView.OriginalCell.transform.position);
            }
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

        private GridTankState InitState(GridTankView gridTankView)
        {
            GridTankState gridTankState = new(gridTankView.GridItemData.Id, gridTankView.OriginalCell.transform.position);
            _gridTankStates.Add(gridTankState);
            return gridTankState;
        }
    }
}