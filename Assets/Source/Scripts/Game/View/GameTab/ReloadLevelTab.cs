using Assets.Source.Scripts.Models;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Scripts.Game
{
    public class ReloadLevelTab : GameTabView
    {
        [SerializeField] private Button _reloadLevelButton;
        [SerializeField] private Button _cancelButton;
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _openReloadPanelButton;

        private GameModel _gameModel;

        private void OnDestroy()
        {
            RemoveListeners();
        }

        public void Initialize(GameModel gameModel)
        {
            _gameModel = gameModel;
            gameObject.SetActive(false);
            AddListeners();
        }

        private void AddListeners()
        {
            _cancelButton.onClick.AddListener(Close);
            _backButton.onClick.AddListener(Close);
            _reloadLevelButton.onClick.AddListener(OnSceneReloaded);
            _openReloadPanelButton.onClick.AddListener(Open);
        }

        private void RemoveListeners()
        {
            _cancelButton.onClick.RemoveListener(Close);
            _backButton.onClick.RemoveListener(Close);
            _reloadLevelButton.onClick.RemoveListener(OnSceneReloaded);
            _openReloadPanelButton.onClick.RemoveListener(Open);
        }

        private void OnSceneReloaded()
        {
            _gameModel.ReloadScene();
            MessageBroker.Default.Publish(new M_ReloadLevel());
        }
    }
}