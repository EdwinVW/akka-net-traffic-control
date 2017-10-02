namespace Messages
{
    public class RegisterSpeedingViolation
    {
        public string VehicleId { get; private set; }
        public string RoadId { get; private set; }
        public double ViolationInKmh { get; private set; }

        public RegisterSpeedingViolation(string vehicleId, string roadId, double violationInKmh)
        {
            VehicleId = vehicleId;
            RoadId = roadId;
            ViolationInKmh = violationInKmh;
        }
    }
}
