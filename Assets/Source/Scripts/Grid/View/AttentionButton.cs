using DG.Tweening;
using System.Collections;
using UnityEngine;

namespace Assets.Source.Scripts.Grid
{
    public class AttentionButton : MonoBehaviour
    {
        private readonly float _durationAnimation = 2f;
        private readonly float _endScaleValue = 1f;
        private readonly Vector3 _vectorScale = new(0.7f, 0.7f, 0.7f);

        private Coroutine _animationCoroutine;

        private void OnEnable()
        {
            if (_animationCoroutine != null)
                StopCoroutine(_animationCoroutine);

            _animationCoroutine = StartCoroutine(SetAnimation());
        }

        private void OnDisable()
        {
            if (_animationCoroutine != null)
                StopCoroutine(_animationCoroutine);
        }

        private IEnumerator SetAnimation()
        {
            while (true)
            {
                transform.localScale = _vectorScale;

                transform.DOScale(_endScaleValue, _durationAnimation)
                    .SetEase(Ease.OutBounce)
                    .SetLink(gameObject);

                yield return new WaitForSeconds(_durationAnimation);
            }
        }
    }
}