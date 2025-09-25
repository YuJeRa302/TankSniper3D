using TMPro;
using UnityEngine;

namespace Assets.Source.Scripts.Grid
{
    public class ItemLevelView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _tmpText;

        private Camera _camera;

        //private void Start()
        //{
        //    _camera = Camera.main;
        //}

        //private void FixedUpdate()
        //{
        //    Vector3 direction = _camera.transform.position - transform.position;

        //    transform.rotation = Quaternion.LookRotation(direction);
        //    transform.rotation = Quaternion.LookRotation(-transform.forward, _camera.transform.up);
        //}

        public void SetLevelValue(int value)
        {
            _tmpText.text = value.ToString();
        }
    }
}