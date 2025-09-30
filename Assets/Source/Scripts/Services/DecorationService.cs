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

        public void SetStateByReward(DecorationState newDecorationState)
        {
            if (_decorationStates != null)
            {
                DecorationState decorationState = FindState(newDecorationState.Id, newDecorationState.TypeCard);

                if (decorationState == null)
                {
                    _decorationStates.Add(new(
                        newDecorationState.Id,
                        newDecorationState.IsBuyed,
                        newDecorationState.IsEquipped,
                        newDecorationState.TypeCard));
                }
                else
                {
                    decorationState.ChangeBuyState(newDecorationState.IsBuyed);
                }
            }
        }

        public void SetStateChangeEquipped(DecorationState newDecorationState)
        {
            if (_decorationStates != null)
            {
                DecorationState decorationState = FindState(newDecorationState.Id, newDecorationState.TypeCard);

                if (decorationState == null)
                {
                    _decorationStates.Add(new(
                        newDecorationState.Id,
                        newDecorationState.IsBuyed,
                        newDecorationState.IsEquipped,
                        newDecorationState.TypeCard));
                }
                else
                {
                    decorationState.ChangeEquippedState(newDecorationState.IsEquipped);
                }
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
            DecorationState decorationState = new(decorationData.Id, false, false, decorationData.TypeCard);
            _decorationStates.Add(decorationState);
            return decorationState;
        }
    }
}