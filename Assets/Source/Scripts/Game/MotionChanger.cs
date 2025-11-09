using Assets.Source.Game.Scripts.Enemy;
using Assets.Source.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace Assets.Source.Scripts.Game
{
    public class MotionChanger : MonoBehaviour
    {
        public static readonly IMessageBroker Message = new MessageBroker();

        private readonly float _maxValue = 1f;
        private readonly float _multiplier = 0.02f;

        [SerializeField] private float _slowMotionScale = 0.25f;
        [SerializeField] private float _slowMotionDuration = 2.5f;
        [SerializeField] private float _pitchDuringSlowMo = 0.5f;
        [SerializeField] private float _returnMotion = 0.8f;

        private List<Enemy> _enemies = new();
        private bool _isSlowMotionActive = false;
        private CompositeDisposable _disposables = new();

        private void OnDestroy()
        {
            _disposables?.Dispose();
        }

        public void Initialize(List<Enemy> enemies)
        {
            _enemies = enemies;

            GameModel.Message
                .Receive<M_DeathEnemy>()
                .Subscribe(_ => CheckForSlowMotionTrigger())
                .AddTo(_disposables);
        }

        private void CheckForSlowMotionTrigger()
        {
            if (_isSlowMotionActive)
                return;

            if (_enemies.Count == 0)
                return;

            bool allDead = _enemies.All(e => e == null || e.IsDead || !e.gameObject.activeInHierarchy);

            if (allDead)
                StartCoroutine(SlowMotionRoutine());
        }

        private IEnumerator SlowMotionRoutine()
        {
            _isSlowMotionActive = true;
            Message.Publish(new M_SlowMotionStarted());
            Time.timeScale = _slowMotionScale;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;

            yield return new WaitForSecondsRealtime(_slowMotionDuration);

            yield return StartCoroutine(SmoothReturnToNormal(_returnMotion));

            Message.Publish(new M_SlowMotionEnded());
            _isSlowMotionActive = false;
        }

        private IEnumerator SmoothReturnToNormal(float duration)
        {
            float startScale = Time.timeScale;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float time = elapsed / duration;
                Time.timeScale = Mathf.Lerp(startScale, _maxValue, time);
                Time.fixedDeltaTime = _multiplier * Time.timeScale;
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            Time.timeScale = _maxValue;
            Time.fixedDeltaTime = _multiplier;
        }
    }
}