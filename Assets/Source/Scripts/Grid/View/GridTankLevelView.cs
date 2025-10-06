using TMPro;
using UnityEngine;

namespace Assets.Source.Scripts.Grid
{
    public class GridTankLevelView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _tmpText;

        public void SetLevelValue(int value)
        {
            _tmpText.text = value.ToString();
        }
    }
}