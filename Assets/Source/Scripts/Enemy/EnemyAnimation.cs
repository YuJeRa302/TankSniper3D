using UnityEngine;

namespace Assets.Source.Game.Scripts.Enemy
{
    public class EnemyAnimation
    {
        private readonly Animator _animator;

        public EnemyAnimation(Animator animator)
        {
            _animator = animator;
        }

        public void SetIdleAnimation()
        {
            if (_animator != null)
                _animator.Play(EnemyAnimationName.IdleAnimation);
        }

        public void SetMoveAnimation()
        {
            if (_animator != null)
                _animator.Play(EnemyAnimationName.MoveAnimation);
        }

        public void SetAttackAnimation()
        {
            if (_animator != null)
                _animator.Play(EnemyAnimationName.AttackAnimation);
        }

        public void SetReloadAnimation()
        {
            if (_animator != null)
                _animator.Play(EnemyAnimationName.ReloadAnimation);
        }

        public void SetDeathAnimation()
        {
            if (_animator != null)
                _animator.Play(EnemyAnimationName.DeathAnimation);
        }
    }
}