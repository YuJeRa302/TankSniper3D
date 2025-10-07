namespace Assets.Source.Scripts.Game
{
    public struct M_Aiming
    {
        private readonly bool _isAiming;

        public M_Aiming(bool isAiming)
        {
            _isAiming = isAiming;
        }

        public readonly bool IsAiming => _isAiming;
    }
}