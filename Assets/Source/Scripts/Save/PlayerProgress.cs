using Assets.Source.Scripts.Services;

namespace Assets.Source.Scripts.Saves
{
    public class PlayerProgress
    {
        public int Money;
        public int Score;
        public int CurrentGridTankCost;
        public int CountBuyedGridTank;
        public float AmbientVolume;
        public float SfxVolume;
        public bool IsMuted;
        public bool IsVibro;
        public bool IsGamePause = false;
        public int CurrentPlayerTankId;
        public int CurrentBiomId;
        public int CurrentLevelId;
        public int CurrentLevel;
        public DecorationService DecorationService;
        public HeroService HeroService;
        public TankService TankService;
        public GridService GridService;
        public LevelService LevelService;

        public PlayerProgress()
        {
            DecorationService = new();
            HeroService = new();
            TankService = new();
            GridService = new();
            LevelService = new();
        }
    }
}