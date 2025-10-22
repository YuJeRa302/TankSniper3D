using Assets.Source.Game.Scripts.States;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New ConfigData", menuName = "Create ConfigData", order = 51)]
    public class ConfigData : ScriptableObject
    {
        [SerializeField] private int _money;
        [SerializeField] private int _currentPlayerTankId;
        [SerializeField] private int _currentBiomId;
        [SerializeField] private int _currentLevelId;
        [SerializeField] private int _currentLevel;
        [SerializeField] private float _ambientVolume;
        [SerializeField] private float _sfxVolumeVolume;
        [SerializeField] private bool _isMuted;
        [SerializeField] private bool _isVibro;
        [SerializeField] private int _currentGridTankCost;
        [SerializeField] private int _countBuyedGridTank;
        [Space(20)]
        [SerializeField] private List<LevelState> _levelStates;
        [Space(20)]
        [SerializeField] private List<TankState> _tankStates;
        [Space(20)]
        [SerializeField] private List<HeroState> _heroStates;
        [Space(20)]
        [SerializeField] private List<DecorationState> _decorationStates;
        [Space(20)]
        [SerializeField] private List<GridTankState> _gridTankStates;

        public int Money => _money;
        public int CurrentPlayerTankId => _currentPlayerTankId;
        public int CurrentBiomId => _currentBiomId;
        public int CurrentLevelId => _currentLevelId;
        public int CurrentLevel => _currentLevel;
        public int CurrentGridTankCost => _currentGridTankCost;
        public int CountBuyedGridTank => _countBuyedGridTank;
        public float AmbientVolume => _ambientVolume;
        public float SfxVolumeVolume => _sfxVolumeVolume;
        public bool IsMuted => _isMuted;
        public bool IsVibro => _isVibro;
        public List<LevelState> LevelStates => _levelStates;
        public List<TankState> TankStates => _tankStates;
        public List<HeroState> HeroStates => _heroStates;
        public List<DecorationState> DecorationStates => _decorationStates;
        public List<GridTankState> GridTankStates => _gridTankStates;
    }
}