using Assets.Source.Scripts.ScriptableObjects;
using Assets.Source.Scripts.Sound;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Scripts.Services
{
    public interface IShootingStrategy
    {
        public void Construct(ProjectileData projectileData, AudioPlayer audioPlayer, List<Transform> shotPoints);
        public void ShootWithoutEnergy(bool isVibroEnabled);
        public void ShootWithEnergy(bool isVibroEnabled);
    }
}