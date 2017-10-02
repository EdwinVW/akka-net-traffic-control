namespace Messages
{
    public class SimulatePassingCar
    {
        public string VehicleId { get; private set; }

        public SimulatePassingCar(string vehicleId)
        {
            VehicleId = vehicleId;
        }
    }
}
