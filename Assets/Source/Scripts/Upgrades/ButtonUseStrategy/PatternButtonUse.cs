using Assets.Source.Game.Scripts.Enums;

namespace Assets.Source.Scripts.Upgrades
{
    public class PatternButtonUse : BaseButtonUse
    {
        public override TypeCard TypeCard => TypeCard.Pattern;

        public override void ClickButton(SelectionButtonView selectionButtonView)
        {
            if (TryGetAction(TypeCard))
                return;

            ClickPatternButton(selectionButtonView);
        }
    }
}