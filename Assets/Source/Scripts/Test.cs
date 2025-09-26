using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Scripts.ScriptableObjects;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private Transform _heroSpawnPoint;
    [SerializeField] private HeroData _heroData;
    [SerializeField] private TypeHeroSpawn _typeHeroSpawn;

    private void Awake()
    {
        var hero = Instantiate(_heroData.HeroView, _heroSpawnPoint);
        hero.Initialize(_heroData, _typeHeroSpawn);
    }
}
