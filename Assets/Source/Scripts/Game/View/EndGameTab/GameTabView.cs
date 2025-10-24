using UniRx;
using UnityEngine;

namespace Assets.Source.Scripts.Game
{
    public abstract class GameTabView : MonoBehaviour
    {
        public static readonly IMessageBroker Message = new MessageBroker();

        public virtual void Open()
        {
            gameObject.SetActive(true);
            MessageBroker.Default.Publish(new M_OpenPanel());
        }

        public virtual void Close()
        {
            gameObject.SetActive(false);
            MessageBroker.Default.Publish(new M_ClosePanel());
        }
    }
}