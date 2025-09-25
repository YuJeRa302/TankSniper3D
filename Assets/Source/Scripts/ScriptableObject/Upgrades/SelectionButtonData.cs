using Assets.Source.Scripts.Services;
using UnityEngine;

namespace Assets.Source.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New SelectionButton", menuName = "Create SelectionButton", order = 51)]
    public class SelectionButtonData : ScriptableObject
    {
        [SerializeField] private Sprite _icon;
        [SerializeReference] private IUseButtonStrategy _useButtonStrategy;

        public IUseButtonStrategy UseButtonStrategy => _useButtonStrategy;
        public Sprite Icon => _icon;
    }
}