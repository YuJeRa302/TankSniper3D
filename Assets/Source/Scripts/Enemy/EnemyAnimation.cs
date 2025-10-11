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

        public void SetIdleAnimation() => _animator.Play(EnemyAnimationName.IdleAnimation);
        public void SetMoveAnimation() => _animator.Play(EnemyAnimationName.MoveAnimation);
        public void SetAttackAnimation() => _animator.Play(EnemyAnimationName.AttackAnimation);
        public void SetReloadAnimation() => _animator.Play(EnemyAnimationName.ReloadAnimation);
        public void SetDeathAnimation() => _animator.Play(EnemyAnimationName.DeathAnimation);
    }
}