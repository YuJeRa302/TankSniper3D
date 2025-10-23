using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Scripts.Game
{
    public class RewardWheelTab : MonoBehaviour
    {
        [SerializeField] private TMP_Text _moneyText;
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private TMP_Text _moneyEarnedText;
        [SerializeField] private TMP_Text _getRewardText;
        [SerializeField] private TMP_Text _loadLevelText;
        [SerializeField] private Button _getRewardButton;
        [SerializeField] private Button _loadLevelButton;

        private int _currentMoney;

        private void Awake()
        {
            _loadLevelButton.onClick.AddListener(OnLoadLevelButtonClicked);
            _getRewardButton.onClick.AddListener(OnGetRewardButtonClicked);
        }

        private void OnDestroy()
        {
            _loadLevelButton.onClick.RemoveListener(OnLoadLevelButtonClicked);
            _getRewardButton.onClick.RemoveListener(OnGetRewardButtonClicked);
        }

        public void Open(int currentMoney, int currentLevel, int moneyEarned)
        {
            _currentMoney = currentMoney;
            _levelText.text = "Уровень " + currentLevel.ToString();
            _moneyText.text = currentMoney.ToString();
            _moneyEarnedText.text = "Награда: " + moneyEarned.ToString();
        }

        private void OnGetMultiplier(int multiplier)
        {
            _getRewardButton.gameObject.SetActive(true);
            _loadLevelButton.gameObject.SetActive(true);
            _loadLevelText.text = _currentMoney + " Достаточно";
            _getRewardText.text = "+ " + Mathf.RoundToInt(_currentMoney * multiplier);
        }

        private void OnLoadLevelButtonClicked()
        {

        }

        private void OnGetRewardButtonClicked()
        {

        }
    }
}