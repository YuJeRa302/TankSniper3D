using Assets.Source.Scripts.Sound;
using DG.Tweening;
using System.Collections;
using UnityEngine;

namespace Assets.Source.Scripts.Upgrades
{
    public class TankSpawnAnimation : MonoBehaviour
    {
        private readonly float _tweenMultiplier = 2f;
        private readonly float _effectLifeTime = 0.5f;

        [SerializeField] private float _spawnHeight = 2f;
        [SerializeField] private float _fallDuration = 0.4f;
        [SerializeField] private float _tweenAnimationDuration = 0.3f;
        [SerializeField] private float _tweenAnimationScaler = 0.7f;
        [SerializeField] private ParticleSystem _smokeEffectPrefab;

        private AudioPlayer _audioPlayer;
        private TankShootAnimation _tankShootAnimation;
        private Coroutine _coroutineTankAnimation;

        public void SetSpawnAnimation(TankShootAnimation tankShootAnimation, AudioPlayer audioPlayer)
        {
            _tankShootAnimation = tankShootAnimation;
            _audioPlayer = audioPlayer;

            if (_coroutineTankAnimation != null)
                StopCoroutine(_coroutineTankAnimation);

            _coroutineTankAnimation = StartCoroutine(SpawnAnimation());
        }

        private IEnumerator SpawnAnimation()
        {
            Transform tank = transform;
            Vector3 targetPos = tank.position;
            Vector3 startPos = targetPos + Vector3.up * _spawnHeight;
            tank.position = startPos;

            tank.DOMoveY(targetPos.y, _fallDuration)
                .SetEase(Ease.InQuad);

            yield return new WaitForSeconds(_fallDuration);

            CreateSpawnEffect(targetPos);

            yield return StartCoroutine(SetLandingDeformation(tank.gameObject, tank.localScale));
        }

        private IEnumerator SetLandingDeformation(GameObject gameObject, Vector3 defaultScale)
        {
            Transform transform = gameObject.transform;
            Vector3 startScale = defaultScale;
            Vector3 targetScale = new(startScale.x, startScale.y * _tweenAnimationScaler, startScale.z);

            transform.DOKill();

            Sequence seq = DOTween.Sequence();

            seq.Append(transform
                .DOScale(targetScale, _tweenAnimationDuration / _tweenMultiplier)
                .SetEase(Ease.OutQuad));

            seq.Append(transform
                .DOScale(startScale, _tweenAnimationDuration)
                .SetEase(Ease.OutBounce));

            seq.SetLink(gameObject);
            seq.Play();

            _audioPlayer.PlaySpawnTankAudio();
            yield return new WaitForSeconds(_tweenAnimationDuration);
            _tankShootAnimation.SetFire();
        }

        private void CreateSpawnEffect(Vector3 targetPos)
        {
            if (_smokeEffectPrefab == null)
                return;

            var effect = Instantiate(_smokeEffectPrefab, targetPos, Quaternion.identity);
            Destroy(effect.gameObject, _effectLifeTime);
        }
    }
}