using UniRx;
using UnityEngine;

namespace Assets.Source.Scripts.Game
{
    public class DroneHealth : MonoBehaviour
    {
        public static readonly IMessageBroker Message = new MessageBroker();

        public void TakeDamage()
        {
            Message.Publish(new M_DeathDrone());
        }
    }
}