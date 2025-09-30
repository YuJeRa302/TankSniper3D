using Assets.Source.Game.Scripts.States;
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
        [Space(20)]
        [SerializeField] private LevelState[] _levelStates;
        [Space(20)]
        [SerializeField] private TankState[] _tankStates;
        [Space(20)]
        [SerializeField] private HeroState[] _heroStates;
        [Space(20)]
        [SerializeField] private DecorationState[] _decorationStates;
        [Space(20)]
        [SerializeField] private GridTankState[] _gridTankStates;

        public int Money => _money;
        public int CurrentPlayerTankId => _currentPlayerTankId;
        public int CurrentBiomId => _currentBiomId;
        public int CurrentLevelId => _currentLevelId;
        public int CurrentLevel => _currentLevel;
        public float AmbientVolume => _ambientVolume;
        public float SfxVolumeVolume => _sfxVolumeVolume;
        public bool IsMuted => _isMuted;
        public LevelState[] LevelStates => _levelStates;
        public TankState[] TankStates => _tankStates;
        public HeroState[] HeroStates => _heroStates;
        public DecorationState[] DecorationStates => _decorationStates;
        public GridTankState[] GridTankStates => _gridTankStates;
    }
}