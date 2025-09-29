using Assets.Source.Game.Scripts.Enums;

namespace Assets.Source.Scripts.Upgrades
{
    public class HeroButtonUse : BaseButtonUse
    {
        public override TypeCard TypeCard => TypeCard.Hero;

        public override void ClickButton(SelectionButtonView selectionButtonView, TypeCard typeCard)
        {
            if (TryGetAction(typeCard))
                return;

            ClickHeroButton(selectionButtonView);
        }
    }
}