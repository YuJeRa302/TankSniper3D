using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Scripts.Services;
using System;

namespace Assets.Source.Scripts.Upgrades
{
    public abstract class BaseButtonUse : IUseButtonStrategy
    {
        public abstract TypeCard TypeCard { get; }

        public event Action<SelectionButtonView> DecalButtonClicked;
        public event Action<SelectionButtonView> PatternButtonClicked;
        public event Action<SelectionButtonView> HeroButtonClicked;
        public event Action<SelectionButtonView> TankButtonClicked;

        public virtual void ClickButton(SelectionButtonView selectionButtonView, TypeCard typeCard)
        {
        }

        public bool TryGetAction(TypeCard typeCard)
        {
            return typeCard != TypeCard;
        }

        protected void ClickDecalButton(SelectionButtonView selectionButtonView)
        {
            DecalButtonClicked?.Invoke(selectionButtonView);
        }

        protected void ClickPatternButton(SelectionButtonView selectionButtonView)
        {
            PatternButtonClicked?.Invoke(selectionButtonView);
        }

        protected void ClickHeroButton(SelectionButtonView selectionButtonView)
        {
            HeroButtonClicked?.Invoke(selectionButtonView);
        }

        protected void ClickTankButton(SelectionButtonView selectionButtonView)
        {
            TankButtonClicked?.Invoke(selectionButtonView);
        }
    }
}