namespace Assets.Source.Scripts.Game
{
    public struct M_SoundStateChanged
    {
        private readonly bool _isMuted;

        public M_SoundStateChanged(bool isMuted)
        {
            _isMuted = isMuted;
        }

        public readonly bool IsMuted => _isMuted;
    }
}