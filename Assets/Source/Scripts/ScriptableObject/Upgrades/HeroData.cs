using Assets.Source.Scripts.Views;
using UnityEngine;

namespace Assets.Source.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Hero", menuName = "Create Hero", order = 51)]
    public class HeroData : ObjectData
    {
        [SerializeField] private HeroView _heroView;
        [SerializeField] private Sprite _icon;
        [SerializeField] private int _starCount;

        public HeroView HeroView => _heroView;
        public Sprite Sprite => _icon;
        public int StarCount => _starCount;
    }
}