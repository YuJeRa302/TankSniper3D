using UnityEngine;

namespace Assets.Source.Scripts.Upgrades
{
    public class BaseView : MonoBehaviour
    {
        protected void ChangeSetActiveObjects(GameObject canvasObject, GameObject sceneObject, bool state)
        {
            canvasObject.SetActive(state);
            sceneObject.gameObject.SetActive(state);
        }
    }
}