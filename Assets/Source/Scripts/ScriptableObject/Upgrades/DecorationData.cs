using UnityEngine;

namespace Assets.Source.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Decoration(Decal/Pattern)", menuName = "Create Decoration", order = 51)]
    public class DecorationData : ObjectData
    {
        [SerializeField] private Sprite _sprite;
        [SerializeField] private Texture _texture;

        public Sprite Sprite => _sprite;
        public Texture Texture => _texture;
    }
}