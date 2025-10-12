namespace Assets.Source.Game.Scripts.Enemy
{
    public class MoveSoldierStrategy : StandartEnemyStrategy
    {
        public override int ReloadCountForPositionChanged => 2;
        public override int CountPositionChange => 2;
    }
}