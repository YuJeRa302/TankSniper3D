using Assets.Source.Scripts.Game;
using UnityEngine;

namespace Assets.Source.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New GameData", menuName = "Create GameData", order = 51)]
    public class GameData : ScriptableObject
    {
        [SerializeField] private BiomsConfig _biomsConfig;
        [SerializeField] private SniperScopeView _sniperScopeViewPrefab;
        [SerializeField] private DroneScopeView _droneScopeViewPrefab;

        public BiomsConfig BiomsConfig => _biomsConfig;
        public SniperScopeView SniperScopeView => _sniperScopeViewPrefab;
        public DroneScopeView DroneScopeView => _droneScopeViewPrefab;
    }
}