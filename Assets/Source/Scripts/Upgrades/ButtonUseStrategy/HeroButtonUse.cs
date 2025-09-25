using Assets.Source.Game.Scripts.Enums;

namespace Assets.Source.Scripts.Upgrades
{
    public class HeroButtonUse : BaseButtonUse
    {
        public override TypeCard TypeCard => TypeCard.Hero;

        public override void ClickButton(SelectionButtonView selectionButtonView)
        {
            if (TryGetAction(TypeCard))
                return;

            ClickHeroButton(selectionButtonView);
        }
    }
}