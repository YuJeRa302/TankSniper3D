using UniRx;
using UnityEngine;

namespace Assets.Source.Scripts.Game
{
    public abstract class GameTabView : MonoBehaviour
    {
        public static readonly IMessageBroker Message = new MessageBroker();

        protected virtual void Open()
        {
            gameObject.SetActive(true);
            MessageBroker.Default.Publish(new M_OpenPanel());
        }

        protected virtual void Close()
        {
            gameObject.SetActive(false);
            MessageBroker.Default.Publish(new M_ClosePanel());
        }
    }
}