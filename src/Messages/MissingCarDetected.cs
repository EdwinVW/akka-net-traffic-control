namespace Messages
{
    public class MissingCarDetected
    {
        public string VehicleId { get; private set; }

        public MissingCarDetected(string vehicleId)
        {
            VehicleId = vehicleId;
        }
    }
}