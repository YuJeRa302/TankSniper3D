using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Scripts.Services;
using UnityEngine;

namespace Assets.Source.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New SelectionButton", menuName = "Create SelectionButton", order = 51)]
    public class SelectionButtonData : ScriptableObject
    {
        [SerializeField] private string _name;
        [SerializeField] private Sprite _icon;
        [SerializeField] private TypeCard _typeCard;
        [SerializeReference] private IUseButtonStrategy _useButtonStrategy;

        public IUseButtonStrategy UseButtonStrategy => _useButtonStrategy;
        public TypeCard TypeCard => _typeCard;
        public Sprite Icon => _icon;
        public string Name => _name;
    }
}