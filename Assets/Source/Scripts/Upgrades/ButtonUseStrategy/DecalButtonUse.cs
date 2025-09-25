using Assets.Source.Game.Scripts.Enums;

namespace Assets.Source.Scripts.Upgrades
{
    public class DecalButtonUse : BaseButtonUse
    {
        public override TypeCard TypeCard => TypeCard.Decal;

        public override void ClickButton(SelectionButtonView selectionButtonView)
        {
            if (TryGetAction(TypeCard))
                return;

            ClickDecalButton(selectionButtonView);
        }
    }
}