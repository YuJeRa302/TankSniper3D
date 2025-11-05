using Assets.Source.Scripts.ScriptableObjects;
using Assets.Source.Scripts.Sound;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Scripts.Services
{
    public interface IAnimationShootingStrategy
    {
        public void Construct(ProjectileData projectileData, AudioPlayer audioPlayer, List<Transform> shotPoints);
        public void Shoot();
    }
}