using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Scripts.Services;
using UnityEngine;

namespace Assets.Source.Scripts.ScriptableObjects
{
    public class ObjectData : ScriptableObject, IIdData
    {
        [SerializeField] private int _id;
        [SerializeField] private TypeCard _typeCard;

        public int Id => _id;
        public TypeCard TypeCard => _typeCard;
    }
}