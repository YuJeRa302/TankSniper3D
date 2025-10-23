using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Scripts.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Scripts.Game
{
    public class EndGamePanelView : MonoBehaviour
    {


        private LevelData _levelData;

        public void Initialize(LevelData levelData) 
        {
            _levelData = levelData;
        }
    }
}