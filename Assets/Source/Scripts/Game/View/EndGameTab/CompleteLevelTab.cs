using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Scripts.Game
{
    public class CompleteLevelTab : MonoBehaviour
    {
        [SerializeField] private TMP_Text _moneyText;
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private TMP_Text _moneyEarnedText;
        [SerializeField] private Button _loadLevelButton;

        private void Awake()
        {
            _loadLevelButton.onClick.AddListener(OnLoadLevelButtonClicked);
        }

        private void OnDestroy()
        {
            _loadLevelButton.onClick.RemoveListener(OnLoadLevelButtonClicked);
        }

        public void Open(int currentMoney, int currentLevel, int moneyEarned)
        {
            _levelText.text = "Уровень " + currentLevel.ToString();
            _moneyText.text = currentMoney.ToString();
            _moneyEarnedText.text = "Награда: " + moneyEarned.ToString();
        }

        private void OnLoadLevelButtonClicked()
        {

        }
    }
}