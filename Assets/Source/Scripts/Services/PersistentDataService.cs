
using Assets.Source.Scripts.Saves;

namespace Assets.Source.Scripts.Services
{
    public class PersistentDataService
    {
        public PlayerProgress PlayerProgress;

        public PersistentDataService()
        {
            if (PlayerProgress == null)
                PlayerProgress = new();
        }

        public bool TrySpendMoney(int value)
        {
            if (PlayerProgress.Money >= value)
            {
                PlayerProgress.Money -= value;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}