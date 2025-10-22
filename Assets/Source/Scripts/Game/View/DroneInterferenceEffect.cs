using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Scripts.Game
{
    public class DroneInterferenceEffect : MonoBehaviour
    {
        public Image interferenceImage; // Image � ����� �� Canvas
        public float maxAlpha = 0.6f; // ������������ ������������ �����
        public float shakeMagnitude = 0.1f; // ������������ �������� ������
        public float shakeSpeed = 20f;

        private float currentAlpha = 0f;
        private Vector3 originalCamPos;

        private void Start()
        {
            if (interferenceImage != null)
            {
                Color c = interferenceImage.color;
                c.a = 0f;
                interferenceImage.color = c;
            }

            originalCamPos = transform.localPosition;
        }

        public void UpdateEffect(float intensity)
        {
            // intensity - �������� �� 0 �� 1, ��� 1 - ������������ ������

            currentAlpha = Mathf.Lerp(0f, maxAlpha, intensity);

            if (interferenceImage != null)
            {
                Color c = interferenceImage.color;
                c.a = currentAlpha;
                interferenceImage.color = c;
            }

            // ������ ��������� ������� � ������ �������������
            float shakeAmount = shakeMagnitude * intensity;

            float shakeX = Mathf.PerlinNoise(Time.time * shakeSpeed, 0f) * 2f - 1f;
            float shakeY = Mathf.PerlinNoise(0f, Time.time * shakeSpeed) * 2f - 1f;

            Vector3 shakeOffset = new Vector3(shakeX, shakeY, 0f) * shakeAmount;
            transform.localPosition = originalCamPos + shakeOffset;
        }

        public void ResetEffect()
        {
            if (interferenceImage != null)
            {
                Color c = interferenceImage.color;
                c.a = 0f;
                interferenceImage.color = c;
            }
            transform.localPosition = originalCamPos;
        }
    }
}