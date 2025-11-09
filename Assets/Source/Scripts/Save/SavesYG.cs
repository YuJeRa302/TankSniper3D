using Assets.Source.Game.Scripts.States;
using System.Collections.Generic;

namespace YG
{
    public partial class SavesYG
    {
        public float AmbientVolume;
        public float SfxVolumeVolume;
        public bool IsMuted;
        public bool IsVibro;
        public int Score;
        public int Level;
        public int Money;
        public int CurrentGridTankCost;
        public int CountBuyedGridTank;
        public int CurrentGridTankLevel;
        public bool HasSave = false;
        public int CurrentPlayerTankId;
        public int CurrentBiomId;
        public int CurrentLevelId;
        public int CurrentLevel;
        public List<TankState> TankStates;
        public List<DecorationState> DecorationStates;
        public List<LevelState> LevelStates;
        public List<GridTankState> GridTankStates;
        public List<HeroState> HeroStates;

        public SavesYG()
        {
        }
    }
}