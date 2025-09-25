using UnityEngine;

namespace Assets.Source.Scripts.ScriptableObjects
{
    public class DecorationData : ObjectData
    {
        [SerializeField] private Sprite _sprite;
        [SerializeField] private Texture _texture;

        public Sprite Sprite => _sprite;
        public Texture Texture => _texture;
    }
}