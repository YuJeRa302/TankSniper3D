using Assets.Source.Game.Scripts.Enums;

namespace Assets.Source.Scripts.Upgrades
{
    public class DecalButtonUse : BaseButtonUse
    {
        public override TypeCard TypeCard => TypeCard.Decal;

        public override void ClickButton(SelectionButtonView selectionButtonView, TypeCard typeCard)
        {
            if (TryGetAction(typeCard))
                return;

            ClickDecalButton(selectionButtonView);
        }
    }
}