using Assets.Source.Game.Scripts.States;
using Assets.Source.Scripts.ScriptableObjects;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Scripts.Services
{
    [Serializable]
    public class HeroService
    {
        [SerializeField] private List<HeroState> _heroStates = new();

        public List<HeroState> HeroStates => _heroStates;

        public void SetStates(List<HeroState> heroStates)
        {
            _heroStates = heroStates;
        }

        public HeroState GetState(int id)
        {
            return FindState(id);
        }

        public HeroState GetStateByData(HeroData heroData)
        {
            HeroState heroState = FindState(heroData.Id);

            if (heroState == null)
                heroState = InitState(heroData);

            return heroState;
        }

        private HeroState FindState(int id)
        {
            if (_heroStates != null)
            {
                foreach (HeroState heroState in _heroStates)
                {
                    if (heroState.Id == id)
                        return heroState;
                }
            }

            return null;
        }

        private HeroState InitState(HeroData heroData)
        {
            HeroState heroState = new(heroData.Id, false, false);
            _heroStates.Add(heroState);
            return heroState;
        }
    }
}