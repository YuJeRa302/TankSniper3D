using Assets.Source.Game.Scripts.States;
using Assets.Source.Scripts.ScriptableObjects;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Scripts.Services
{
    [Serializable]
    public class TankService
    {
        [SerializeField] private List<TankState> _tankStates = new();

        public List<TankState> TankStates => _tankStates;

        public void SetStates(TankState[] tankStates)
        {
            for (int index = 0; index < tankStates.Length; index++)
            {
                _tankStates.Add(tankStates[index]);
            }
        }

        public TankState GetStateByEquip()
        {
            if (_tankStates != null)
            {
                foreach (TankState tankState in _tankStates)
                {
                    if (tankState.IsEquipped == true)
                        return tankState;
                }
            }

            return null;
        }

        public TankState GetState(TankData tankData)
        {
            TankState tankState = FindState(tankData.Id);

            if (tankState == null)
                tankState = InitState(tankData);

            return tankState;
        }

        private TankState FindState(int id)
        {
            if (_tankStates != null)
            {
                foreach (TankState tankState in _tankStates)
                {
                    if (tankState.Id == id)
                        return tankState;
                }
            }

            return null;
        }

        private TankState InitState(TankData tankData)
        {
            TankState tankState = new(tankData.Id, tankData.Level, false, false, 0, 0, 0);
            _tankStates.Add(tankState);
            return tankState;
        }
    }
}