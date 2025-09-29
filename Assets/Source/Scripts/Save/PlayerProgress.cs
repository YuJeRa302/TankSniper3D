using Assets.Source.Scripts.Services;

namespace Assets.Source.Scripts.Saves
{
    public class PlayerProgress
    {
        public int Money;
        public int Score;
        public float AmbientVolume;
        public float SfxVolume;
        public bool IsMuted;
        public bool IsGamePause = false;
        public int CurrentPlayerTankId;
        public DecorationService DecorationService;
        public HeroService HeroService;
        public TankService TankService;
        public GridService GridService;

        public PlayerProgress()
        {
            DecorationService = new();
            HeroService = new();
            TankService = new();
            GridService = new();
        }
    }
}