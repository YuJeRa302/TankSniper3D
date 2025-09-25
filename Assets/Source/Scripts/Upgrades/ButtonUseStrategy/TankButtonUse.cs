using Assets.Source.Game.Scripts.Enums;

namespace Assets.Source.Scripts.Upgrades
{
    public class TankButtonUse : BaseButtonUse
    {
        public override TypeCard TypeCard => TypeCard.Tank;

        public override void ClickButton(SelectionButtonView selectionButtonView)
        {
            if (TryGetAction(TypeCard))
                return;

            ClickTankButton(selectionButtonView);
        }
    }
}