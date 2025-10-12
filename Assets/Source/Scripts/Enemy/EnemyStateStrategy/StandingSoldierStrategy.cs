namespace Assets.Source.Game.Scripts.Enemy
{
    public class StandingSoldierStrategy : StandartEnemyStrategy
    {
        public override int ReloadCountForPositionChanged => 0;
        public override int CountPositionChange => 0;
    }
}