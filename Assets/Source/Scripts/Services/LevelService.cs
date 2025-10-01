using Assets.Source.Game.Scripts.States;
using Assets.Source.Scripts.ScriptableObjects;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Scripts.Services
{
    [Serializable]
    public class LevelService
    {
        [SerializeField] private List<LevelState> _levelStates = new();

        public List<LevelState> LevelStates => _levelStates;

        public void SetStates(LevelState[] levelStates)
        {
            for (int index = 0; index < levelStates.Length; index++)
            {
                _levelStates.Add(levelStates[index]);
            }
        }

        public LevelState GetState(LevelData levelData, int biomId)
        {
            LevelState levelState = FindState(levelData.Id, biomId);

            if (levelState == null)
                levelState = InitState(levelData, biomId);

            return levelState;
        }

        private LevelState FindState(int id, int biomId)
        {
            if (_levelStates != null)
            {
                foreach (LevelState levelState in _levelStates)
                {
                    if (levelState.BiomId == biomId)
                    {
                        if (levelState.Id == id)
                            return levelState;
                    }
                }
            }

            return null;
        }

        private LevelState InitState(LevelData levelData, int biomId)
        {
            LevelState levelState = new(levelData.Id, biomId, false);
            _levelStates.Add(levelState);
            return levelState;
        }
    }
}