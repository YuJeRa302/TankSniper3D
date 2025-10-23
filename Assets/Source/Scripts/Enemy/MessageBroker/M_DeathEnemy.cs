namespace Assets.Source.Game.Scripts.Enemy
{
    public struct M_DeathEnemy
    {
        private readonly int _moneyReward;

        public M_DeathEnemy(int moneyReward)
        {
            _moneyReward = moneyReward;
        }

        public readonly int MoneyReward => _moneyReward;
    }
}