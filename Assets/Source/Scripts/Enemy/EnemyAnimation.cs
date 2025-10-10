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

        public void SetIdleAnimation() => _animator.SetTrigger(EnemyAnimationName.IdleAnimation);
        public void SetMoveAnimation() => _animator.SetTrigger(EnemyAnimationName.MoveAnimation);
        public void SetAttackAnimation() => _animator.SetTrigger(EnemyAnimationName.AttackAnimation);
        public void SetReloadAnimation() => _animator.SetTrigger(EnemyAnimationName.ReloadAnimation);
        public void SetDeathAnimation() => _animator.SetTrigger(EnemyAnimationName.DeathAnimation);
    }
}