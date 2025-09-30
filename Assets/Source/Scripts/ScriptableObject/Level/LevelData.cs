using UnityEngine;

namespace Assets.Source.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New LevelData", menuName = "Create LevelData", order = 51)]
    public class LevelData : ScriptableObject
    {
        [SerializeField] private int _id;
        [SerializeField] private string _nameScene;
        [SerializeField] private Sprite _icon;

        public int Id => _id;
        public string NameScene => _nameScene;
        public Sprite Icon => _icon;
    }
}