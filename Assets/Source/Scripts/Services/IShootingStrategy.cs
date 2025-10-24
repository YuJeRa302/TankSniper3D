using Assets.Source.Scripts.ScriptableObjects;
using UnityEngine;

namespace Assets.Source.Scripts.Services
{
    public interface IShootingStrategy
    {
        public void Construct(ProjectileData projectileData, Transform shotPoint);
        public void ShootWithoutEnergy(bool isVibroEnabled);
        public void ShootWithEnergy(bool isVibroEnabled);
    }
}