using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Scripts.Game
{
    public class DefeatTab : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Image timerCircle;        // круг с Fill Amount
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button noThanksButton;

        [Header("Timer Settings")]
        [SerializeField] private float totalTime = 5f;     // общее время таймера (в секундах)
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color warningColor = Color.red;

        private float timeLeft;
        private bool isRunning = false;

        private void Start()
        {
            StartTimer(5f);
        }

        public void StartTimer(float duration = 5f)
        {
            totalTime = duration;
            timeLeft = totalTime;
            isRunning = true;
            gameObject.SetActive(true);
            timerCircle.fillAmount = 1f;
            timerText.text = Mathf.CeilToInt(timeLeft).ToString();

            StartCoroutine(UpdateTimer());
        }

        private IEnumerator UpdateTimer()
        {
            while (timeLeft > 0f)
            {
                timeLeft -= Time.deltaTime;
                float normalized = Mathf.Clamp01(timeLeft / totalTime);

                timerCircle.fillAmount = normalized;
                timerText.text = Mathf.CeilToInt(timeLeft).ToString();

                // меняем цвет, если время почти вышло
                if (timeLeft < 1.5f)
                    timerCircle.color = warningColor;
                else
                    timerCircle.color = normalColor;

                yield return null;
            }

            OnTimerFinished();
        }

        private void OnTimerFinished()
        {
            isRunning = false;
            timerCircle.fillAmount = 0f;
            timerText.text = "0";
            continueButton.interactable = false;
        }

        public void OnContinuePressed()
        {
            if (!isRunning) return;
            isRunning = false;
            StopAllCoroutines();
        }

        public void OnNoThanksPressed()
        {
            if (!isRunning) return;
            isRunning = false;
            StopAllCoroutines();
        }
    }
}