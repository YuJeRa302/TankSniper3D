using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Scripts.Game
{
    public class DroneScopeView : MonoBehaviour
    {
        private Button _sniperScopeButton;

        public void Initialize(Button sniperScopeButton)
        {
            gameObject.SetActive(false);
            _sniperScopeButton = sniperScopeButton;
        }
    }
}