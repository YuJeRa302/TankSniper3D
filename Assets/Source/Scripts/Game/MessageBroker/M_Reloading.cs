namespace Assets.Source.Scripts.Game
{
    public struct M_Reloading
    {
        private readonly float _reloadTime;

        public M_Reloading(float reloadTime)
        {
            _reloadTime = reloadTime;
        }

        public readonly float ReloadTime => _reloadTime;
    }
}