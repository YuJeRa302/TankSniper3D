using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Scripts.Upgrades;
using System;

namespace Assets.Source.Scripts.Services
{
    public interface IUseButtonStrategy
    {
        public void ClickButton(SelectionButtonView selectionButtonView, TypeCard typeCard);
        public event Action<SelectionButtonView> DecalButtonClicked;
        public event Action<SelectionButtonView> PatternButtonClicked;
        public event Action<SelectionButtonView> HeroButtonClicked;
        public event Action<SelectionButtonView> TankButtonClicked;
    }
}