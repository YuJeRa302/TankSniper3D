using Assets.Source.Scripts.Upgrades;
using System;

namespace Assets.Source.Scripts.Services
{
    public interface IUseButtonStrategy
    {
        public void ClickButton(SelectionButtonView selectionButtonView);
        public event Action<SelectionButtonView> DecalButtonClicked;
        public event Action<SelectionButtonView> PatternButtonClicked;
        public event Action<SelectionButtonView> HeroButtonClicked;
        public event Action<SelectionButtonView> TankButtonClicked;
    }
}