using Assets.Source.Game.Scripts.Enums;

namespace Assets.Source.Scripts.Upgrades
{
    public class TankButtonUse : BaseButtonUse
    {
        public override TypeCard TypeCard => TypeCard.Tank;

        public override void ClickButton(SelectionButtonView selectionButtonView, TypeCard typeCard)
        {
            if (TryGetAction(typeCard))
                return;

            ClickTankButton(selectionButtonView);
        }
    }
}