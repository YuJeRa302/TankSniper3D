using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Game.Scripts.States;
using Assets.Source.Scripts.ScriptableObjects;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Scripts.Services
{
    [Serializable]
    public class DecorationService
    {
        [SerializeField] private List<DecorationState> _decorationStates = new();

        public List<DecorationState> DecorationStates => _decorationStates;

        public void SetStates(DecorationState[] decorationStates)
        {
            for (int index = 0; index < decorationStates.Length; index++)
            {
                _decorationStates.Add(decorationStates[index]);
            }
        }

        public DecorationState GetState(DecorationData decorationData)
        {
            DecorationState decorationState = FindState(decorationData.Id, decorationData.TypeCard);

            if (decorationState == null)
                decorationState = InitState(decorationData);

            return decorationState;
        }

        private DecorationState FindState(int id, TypeCard typeCard)
        {
            if (_decorationStates != null)
            {
                foreach (DecorationState decorationState in _decorationStates)
                {
                    if (decorationState.TypeCard == typeCard)
                    {
                        if (decorationState.Id == id)
                            return decorationState;
                    }
                }
            }

            return null;
        }

        private DecorationState InitState(DecorationData decorationData)
        {
            DecorationState decorationState = new(decorationData.Id, false, decorationData.TypeCard);
            _decorationStates.Add(decorationState);
            return decorationState;
        }
    }
}