namespace Assets.Source.Scripts.Game
{
    public struct M_CreateDrone
    {
        private readonly DroneView _droneView;

        public M_CreateDrone(DroneView droneView)
        {
            _droneView = droneView;
        }

        public readonly DroneView DroneView => _droneView;
    }
}