using Assets.Source.Scripts.Saves;
using Assets.Source.Scripts.ScriptableObjects;
using Assets.Source.Scripts.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Scripts.Game
{
    public class GameConstruct : MonoBehaviour
    {
        private SaveAndLoader _saveAndLoader;
        private PersistentDataService _persistentDataService;

        private void LoadData()
        {
            _persistentDataService = new PersistentDataService();
            _saveAndLoader = new SaveAndLoader(_persistentDataService);
            _saveAndLoader.LoadDataFromPrefs();
        }
    }
}