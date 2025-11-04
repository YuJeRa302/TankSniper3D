using DG.Tweening;
using UnityEngine;

namespace Assets.Source.Scripts.Grid
{
    public class AttentionButton : MonoBehaviour
    {
        private readonly int _valueForLoopTween = -1;
        private readonly float _durationAnimation = 2f;
        private readonly float _endScaleValue = 1f;
        private readonly Vector3 _vectorScale = new(0.7f, 0.7f, 0.7f);

        private Tween _tween;

        private void OnEnable()
        {
            StartPulse();
        }

        private void OnDisable()
        {
            StopPulse();
        }

        private void StartPulse()
        {
            transform.localScale = _vectorScale;

            _tween = transform
                .DOScale(_endScaleValue, _durationAnimation)
                .SetEase(Ease.InOutSine)
                .SetLoops(_valueForLoopTween, LoopType.Yoyo)
                .SetLink(gameObject);
        }

        private void StopPulse()
        {
            if (_tween != null && _tween.IsActive())
                _tween.Kill();
        }
    }
}