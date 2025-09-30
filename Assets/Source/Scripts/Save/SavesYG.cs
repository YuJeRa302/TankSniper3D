using Assets.Source.Game.Scripts.States;
using System.Collections.Generic;

namespace YG
{
    public partial class SavesYG
    {
        public int Coins;
        public float AmbientVolume;
        public float SfxVolumeVolume;
        public bool IsMuted;
        public int Score;
        public bool HasSave = false;

        public SavesYG()
        {
        }
    }
}