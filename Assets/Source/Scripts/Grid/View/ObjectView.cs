using UnityEngine;

namespace Assets.Source.Scripts.Grid
{
    public abstract class ObjectView : MonoBehaviour
    {
        public int Level { get; private set; }

        public virtual void Construct(int level)
        {
            Level = level;
        }
    }
}