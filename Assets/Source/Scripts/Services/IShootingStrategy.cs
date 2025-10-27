using Assets.Source.Scripts.ScriptableObjects;
using Assets.Source.Scripts.Sound;
using UnityEngine;

namespace Assets.Source.Scripts.Services
{
    public interface IShootingStrategy
    {
        public void Construct(ProjectileData projectileData, AudioPlayer audioPlayer, Transform shotPoint);
        public void ShootWithoutEnergy(bool isVibroEnabled);
        public void ShootWithEnergy(bool isVibroEnabled);
    }
}